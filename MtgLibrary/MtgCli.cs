using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommandLine;

namespace MtgCli
{
    enum Entity
    {
        player,
        land,
        card
    }
    enum Action 
    {
        add,
        delete
    }

    enum CardColor
    {
        white,
        black,
        red,
        blue,
        green
    }

    [Verb("search", HelpText = "Search for cards in the local library or the MTG online API")]
    class SearchOptions
    {
        [Option(Default=false, HelpText="Search online instead of the local library")]
        public bool Online { get; set; }
        [Option('n', "name", Required=true, HelpText="Name of the card (partial name allowed)")]
        public string Name { get; set; }

        public static async Task<int> Process(SearchOptions opts)
        {
            List<MtgData.Card> result = await MtgApiClient.CardService.AsyncSearchCardByName(opts.Name);
            result.ForEach(Console.WriteLine);
            return 0;
        }
    }

    [Verb("add")]
    class AddOptions
    {
        [Option("player", SetName="player")]
        public bool IsPlayer { get; set; }

        [Option("name", SetName="player", Required=true, HelpText="Player's name to add")]
        public string PlayerName { get; set; }

        public static async Task<int> Process(AddOptions opts, MtgData.DataAccess db)
        {
            return await db.AsyncAddPlayer(new MtgData.Player {Name = opts.PlayerName});
        }
    }

    [Verb("get")]
    class GetOptions
    {
        [Option('k', "key", Required=true, HelpText="Setting key")]
        public string Key { get; set; }
        
        public static async Task<int> Process(GetOptions opts)
        {
            var value = await MtgLibrary.Settings.AsyncGetSetting(opts.Key) ?? "Not Set";
            Console.WriteLine($"{opts.Key}: {value}");
            return 0;
        }
    }

    [Verb("exit")]
    class ExitOptions
    {
        // Creating empty class to match the verb, there may be other ways to do this
    }

    public class Runner
    {
        private MtgData.DataAccess db;
        private MtgData.Player currentPlayer { get; set; }

        public Runner()
        {
            db = new MtgData.DataAccess();
        }
        public Task<int> run(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<SearchOptions, AddOptions, GetOptions, ExitOptions>(args)
            .MapResult(
                (SearchOptions opts) => SearchOptions.Process(opts),
                (AddOptions opts) => AddOptions.Process(opts, db),
                (GetOptions opts) => GetOptions.Process(opts),
                (ExitOptions opts) => Task.FromResult(-1),
                errors => Task.FromResult(-2)
            );
        }
    }
}
