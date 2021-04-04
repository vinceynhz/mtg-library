using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MtgLibrary;
using CommandLine;

namespace MtgCli
{
    enum ReturnCode
    {
        OK = 0,
        WRONG_PARAMETER = -1,
        DATA_ERROR = -2,
        NO_RESULT = -3,
    }

    abstract class Options
    {
        protected async Task<(ReturnCode, MtgData.Player)> ResolvePlayer(
            MtgData.DataAccess db,
            MtgData.Player globalPlayer,
            int playerId
        )
        {
            if (null == globalPlayer && playerId == 0)
            {
                Console.WriteLine("No player selected.");
                Console.WriteLine("Use `--id {id}` parameter, or");
                Console.WriteLine("Set global player with `set --player {id}`");
                return (ReturnCode.WRONG_PARAMETER, null);
            }
            MtgData.Player currentPlayer = globalPlayer ?? await db.AsyncGetPlayerById(playerId);
            if (null == currentPlayer)
            {
                Console.WriteLine($"Player with id {playerId} not found");
                return (ReturnCode.NO_RESULT, null);
            }
            return (ReturnCode.OK, currentPlayer);
        }
        protected async Task<ReturnCode> PlayerUpdate(
            MtgData.DataAccess db,
            MtgData.Player globalPlayer,
            int playerId,
            Action<MtgData.Player> update
        )
        {
            (ReturnCode rc, MtgData.Player player) currentPlayer = await this.ResolvePlayer(db, globalPlayer, playerId);
            if (currentPlayer.rc != ReturnCode.OK)
            {
                return currentPlayer.rc;
            }
            update(currentPlayer.player);
            if (!await db.AsyncUpdatePlayer(currentPlayer.player))
            {
                Console.WriteLine("Error updating player in database");
                return ReturnCode.DATA_ERROR;
            }
            Console.WriteLine("Player information updated");
            return ReturnCode.OK;
        }
    }

    [Verb("search", HelpText = "Search for cards in the MTG online API")]
    class SearchOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the card (partial name allowed)")]
        public string Name { get; set; }

        [Option('t', "type", HelpText = "Type of the card (partial name allowed)")]
        public string Type { get; set; }

        [Option('s', "set", HelpText = "Set of the card (3 characters)")]
        public string SetCode { get; set; }

        public async Task<int> Process(Action<List<MtgData.Card>> consumer)
        {
            ReturnCode rc = ReturnCode.OK;
            Console.WriteLine("Searching online...");
            List<MtgData.Card> result = await MtgLibrary.CardService.AsyncSearchCard(this.Name, this.Type, this.SetCode);
            if (result == null)
            {
                rc = ReturnCode.DATA_ERROR;
            }
            else if (result.Count == 0)
            {
                Console.WriteLine($"No results found for name {this.Name}");
                rc = ReturnCode.NO_RESULT;
            }
            else
            {
                for (int i = 0; i < result.Count; i++)
                {
                    Console.WriteLine($"{i:00000} - {result[i]}");
                }
                Console.WriteLine($"{result.Count} results found");
                consumer(result);
            }
            return (int)rc;
        }
    }

    [Verb("add", HelpText = "Add elements to the library (players, lands, cards)")]
    class AddOptions : Options
    {
        // PLAYER options
        [Option("player", SetName = "player", HelpText = "See options with [PLAYER]")]
        public bool IsPlayer { get; set; }

        [Option('n', "name", SetName = "player", Required = true, HelpText = "[PLAYER] Player's name")]
        public string PlayerName { get; set; }

        // LAND options
        [Option("land", SetName = "land", HelpText = "See options with [LAND]")]
        public bool IsLand { get; set; }

        [Option('c', "color", SetName = "land", Required = true, HelpText = "[LAND] Color of the land to add to a player")]
        public MtgData.CardColor Color { get; set; }

        [Option("number", SetName = "land", Default = 1, HelpText = "[LAND] Number of lands to add to a player")]
        public int Number { get; set; }

        // CARD options
        [Option("card", SetName = "card", HelpText = "See optins with [CARD]")]
        public bool IsCard { get; set; }

        [Option('x', "index", SetName = "card", Required = true, HelpText = "[CARD] Index of the card to add from search results")]
        public int ResultIndex { get; set; }

        // COMMON options
        [Option('i', "id", HelpText = "[LAND|CARD] Id of the player to add the card to, if not set global player will be used")]
        public int PlayerId { get; set; }

        public async Task<int> Process(
            MtgData.DataAccess db,
            List<MtgData.Card> searchResults,
            MtgData.Player globalPlayer
        )
        {
            ReturnCode rc;
            if (this.IsPlayer)
            {
                rc = await this.AddPlayer(db);
            }
            else if (this.IsLand)
            {
                rc = await this.PlayerUpdate(
                        db,
                        globalPlayer,
                        this.PlayerId,
                        currentPlayer => currentPlayer.addLand(this.Color, this.Number)
                    );
            }
            else if (this.IsCard)
            {
                rc = await this.AddCard(db, searchResults, globalPlayer);
            }
            else
            {
                Console.WriteLine("Unsupported option selected");
                rc = ReturnCode.WRONG_PARAMETER;
            }
            return (int)rc;
        }

        private async Task<ReturnCode> AddPlayer(MtgData.DataAccess db)
        {
            var added = await db.AsyncAddPlayer(new MtgData.Player { Name = this.PlayerName });
            if (null != added)
            {
                Console.WriteLine("New player added sucessfully");
                Console.WriteLine(added);
                return ReturnCode.OK;
            }
            Console.WriteLine("Error adding player");
            return ReturnCode.DATA_ERROR;
        }

        private async Task<ReturnCode> AddCard(
            MtgData.DataAccess db,
            List<MtgData.Card> searchResults,
            MtgData.Player globalPlayer
        )
        {
            // First check we have results to work with
            if (null == searchResults || searchResults.Count == 0)
            {
                Console.WriteLine("No recent search results available");
                return ReturnCode.NO_RESULT;
            }
            // Second check if the index of the card passed exists in the searchResults
            if (this.ResultIndex >= searchResults.Count)
            {
                Console.WriteLine($"Index passed {this.ResultIndex} exceeds results {searchResults.Count - 1}");
                return ReturnCode.WRONG_PARAMETER;
            }
            // Third check if the player exists
            (ReturnCode rc, MtgData.Player player) currentPlayer = await this.ResolvePlayer(db, globalPlayer, this.PlayerId);
            if (currentPlayer.rc != ReturnCode.OK)
            {
                return currentPlayer.rc;
            }
            // Use id from the current player, whether from session or from db
            this.PlayerId = currentPlayer.player.Id;
            MtgData.Card card = searchResults[this.ResultIndex];
            var added = await db.AsyncAddPlayerCard(this.PlayerId, card);
            if (null != added)
            {
                Console.WriteLine("New card added sucessfully");
                Console.WriteLine(added);
                return ReturnCode.OK;
            }
            Console.WriteLine("Error adding card to player");
            return ReturnCode.DATA_ERROR;
        }
    }

    [Verb("get", HelpText = "Get elements from the library (players, cards)")]
    class GetOptions : Options
    {
        // PLAYER options
        [Option("player", SetName = "player", HelpText = "See options with [PLAYER]")]
        public bool IsPlayer { get; set; }

        // SETTING options
        [Option("setting", SetName = "setting", HelpText = "See options with [SETTING]")]
        public bool IsSetting { get; set; }

        [Option('k', "key", SetName = "setting", HelpText = "Key of the setting to retrieve, if none set all current settings keys will be retrieved")]
        public string Key { get; set; }

        // RESULT options
        [Option("results", SetName = "result", HelpText = "Show last results from search")]
        public bool IsResult { get; set; }

        // CARD options
        [Option("card", SetName = "card", HelpText = "See optins with [CARD]")]
        public bool IsCard { get; set; }

        [Option('n', "name", SetName = "card", HelpText = "[CARD] Name of the card (partial name allowed)")]
        public string Name { get; set; }

        [Option('t', "type", SetName = "card", HelpText = "[CARD] Type of the card (partial name allowed)")]
        public string Type { get; set; }

        [Option('s', "set", SetName = "card", HelpText = "[CARD] Set of the card (3 characters)")]
        public string SetCode { get; set; }

        // COMMON options
        [Option('i', "id", HelpText = "[PLAYER|CARD] Id of the player to get, if none set all [PLAYER|CARD] info will be retrieved")]
        public int PlayerId { get; set; }

        public async Task<int> Process(
            MtgData.DataAccess db,
            List<MtgData.Card> searchResults,
            MtgData.Player globalPlayer
        )
        {
            ReturnCode rc = ReturnCode.OK;
            if (this.IsPlayer)
            {
                rc = await this.GetPlayer(db);
            }
            else if (this.IsSetting)
            {
                rc = await this.GetSetting();
            }
            else if (this.IsResult)
            {
                rc = await this.GetResults(searchResults);
            }
            else if (this.IsCard)
            {
                rc = await this.GetCard(db, globalPlayer);
            }
            else
            {
                Console.WriteLine("Unsupported option selected");
                rc = ReturnCode.WRONG_PARAMETER;
            }
            return (int)rc;
        }

        private async Task<ReturnCode> GetPlayer(MtgData.DataAccess db)
        {
            ReturnCode rc = ReturnCode.OK;
            if (this.PlayerId == 0)
            {
                // No player id defined, get all from db
                var players = await db.AsyncGetPlayers();
                if (players.Count == 0)
                {
                    Console.WriteLine("No players found");
                    rc = ReturnCode.NO_RESULT;
                }
                else
                {
                    players.ForEach(Console.WriteLine);
                }
            }
            else
            {
                // Get player from db
                var player = await db.AsyncGetPlayerById(this.PlayerId);
                if (null == player)
                {
                    Console.WriteLine($"Player with id {this.PlayerId} not found");
                    rc = ReturnCode.NO_RESULT;
                }
                else
                {
                    Console.WriteLine(player);
                }
            }
            return rc;
        }

        private async Task<ReturnCode> GetSetting()
        {
            ReturnCode rc = ReturnCode.OK;
            if (null == this.Key)
            {
                string[] settings = await MtgLibrary.Settings.AsyncGetAllKeys();
                if (null == settings || settings.Length == 0)
                {
                    Console.WriteLine("No settings defined");
                    rc = ReturnCode.NO_RESULT;
                }
                else
                {
                    foreach (var key in settings)
                    {
                        Console.WriteLine(key);
                    }
                }
            }
            else
            {
                string value = await MtgLibrary.Settings.AsyncGet(this.Key);
                if (null == value || value.Length == 0)
                {
                    Console.WriteLine($"Setting with key {this.Key} not defined");
                    rc = ReturnCode.NO_RESULT;
                }
                else
                {
                    Console.WriteLine($"{this.Key}={value}");
                }
            }
            return rc;
        }

        private async Task<ReturnCode> GetResults(List<MtgData.Card> searchResults)
        {
            ReturnCode rc = ReturnCode.OK;
            if (null == searchResults || searchResults.Count == 0)
            {
                Console.WriteLine("No recent search results available");
                rc = ReturnCode.NO_RESULT;
            }
            else
            {
                for (int i = 0; i < searchResults.Count; i++)
                {
                    Console.WriteLine($"{i:00000} - {searchResults[i]}");
                }
                Console.WriteLine($"{searchResults.Count} results found");
            }
            return await Task.FromResult(rc);
        }

        private async Task<ReturnCode> GetCard(MtgData.DataAccess db, MtgData.Player globalPlayer)
        {
            (ReturnCode rc, MtgData.Player player) currentPlayer = await this.ResolvePlayer(db, globalPlayer, this.PlayerId);
            if (currentPlayer.rc != ReturnCode.OK)
            {
                return currentPlayer.rc;
            }
            // Use id from the current player, whether from session or from db
            this.PlayerId = currentPlayer.player.Id;
            // Find all cards
            List<MtgData.PlayerCard> result = await db.AsyncFindPlayerCards(this.PlayerId, this.Name, this.Type, this.SetCode);
            if (result.Count == 0)
            {
                Console.WriteLine($"No results found");
                return ReturnCode.NO_RESULT;
            }
            else
            {
                for (int i = 0; i < result.Count; i++)
                {
                    Console.WriteLine($"{i:00000} - {result[i]}");
                }
                Console.WriteLine($"{result.Count} results found");
            }
            return ReturnCode.OK;
        }
    }

    [Verb("set", HelpText = "Set attributes of the current session")]
    class SetOptions : Options
    {
        // PLAYER options
        [Option("player", SetName = "player", HelpText = "See options with [PLAYER]")]
        public bool IsPlayer { get; set; }

        // SETTING options
        [Option("setting", SetName = "setting", HelpText = "See options with [SETTING]")]
        public bool IsSetting { get; set; }

        [Option('k', "key", SetName = "setting", Required = true, HelpText = "Key of the setting")]
        public string Key { get; set; }

        [Option('v', "value", SetName = "setting", Required = true, HelpText = "Value of the setting")]
        public string Value { get; set; }

        // LAND options
        [Option("land", SetName = "land", HelpText = "See options with [LAND]")]
        public bool IsLand { get; set; }

        [Option('c', "color", SetName = "land", Required = true, HelpText = "[LAND] Color of the land to add to a player")]
        public MtgData.CardColor Color { get; set; }

        [Option('n', "number", SetName = "land", Required = true, HelpText = "[LAND] Number of lands to set to a player")]
        public int Number { get; set; }

        // COMMON options

        [Option('i', "id", HelpText = "[PLAYER|LAND] Id of the player")]
        public int PlayerId { get; set; }

        public async Task<int> Process(
            MtgData.DataAccess db,
            MtgData.Player globalPlayer,
            Action<MtgData.Player> consumer
        )
        {
            ReturnCode rc = ReturnCode.OK;
            if (this.IsPlayer)
            {
                rc = await this.SetPlayer(db, consumer);
            }
            else if (this.IsSetting)
            {
                rc = await this.SetSetting();
            }
            else if (this.IsLand)
            {
                rc = await this.PlayerUpdate(
                        db,
                        globalPlayer,
                        this.PlayerId,
                        currentPlayer => currentPlayer.setLand(this.Color, this.Number)
                    );
            }
            else
            {
                Console.WriteLine("Unsupported option selected");
                rc = ReturnCode.WRONG_PARAMETER;
            }
            return (int)rc;
        }

        private async Task<ReturnCode> SetPlayer(MtgData.DataAccess db, Action<MtgData.Player> consumer)
        {
            ReturnCode rc = ReturnCode.OK;
            var player = await db.AsyncGetPlayerById(this.PlayerId);
            if (null == player)
            {
                Console.WriteLine($"Player with id {this.PlayerId} not found");
                rc = ReturnCode.NO_RESULT;
            }
            else
            {
                consumer(player);
                Console.WriteLine(player);
            }
            return rc;
        }

        private async Task<ReturnCode> SetSetting()
        {
            ReturnCode rc = ReturnCode.OK;
            string value = await MtgLibrary.Settings.AsyncAdd(this.Key, this.Value);
            if (null == value || value.Length == 0)
            {
                Console.WriteLine($"Unable to set setting with key {this.Key}");
                rc = ReturnCode.DATA_ERROR;
            }
            else
            {
                Console.WriteLine("Setting saved");
            }
            return rc;
        }
    }

    public class Runner
    {
        private MtgData.DataAccess Db;
        private MtgData.Player GlobalPlayer { get; set; }

        private List<MtgData.Card> SearchResult { get; set; }
        public Runner()
        {
            Db = new MtgData.DataAccess();
        }
        public Task<int> run(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<SearchOptions, AddOptions, GetOptions, SetOptions>(args)
            .MapResult(
                (SearchOptions opts) => opts.Process((cards) => this.SearchResult = cards),
                (AddOptions opts) => opts.Process(this.Db, this.SearchResult, this.GlobalPlayer),
                (GetOptions opts) => opts.Process(this.Db, this.SearchResult, this.GlobalPlayer),
                (SetOptions opts) => opts.Process(this.Db, this.GlobalPlayer, (player) => this.GlobalPlayer = player),
                errors => Task.FromResult((int)ReturnCode.WRONG_PARAMETER)
            );
        }

        public string GetCurrentPlayerName()
        {
            if (this.GlobalPlayer == null)
            {
                return null;
            }
            return this.GlobalPlayer.Name;
        }
    }
}
