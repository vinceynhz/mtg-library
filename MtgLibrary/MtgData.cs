using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MtgData
{
    public enum CardColor
    {
        white,
        black,
        red,
        blue,
        green
    }

    public class Card
    {
        [JsonPropertyName("id")]
        public string MtgId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("manaCost")]
        public string ManaCost { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("set")]
        public string SetCode { get; set; }

        public int GetCost(CardColor color)
        {
            if (null == this.ManaCost || this.ManaCost.Length == 0)
            {
                return -1;
            }
            char colorCode = ' ';
            switch (color)
            {
                case CardColor.white:
                    colorCode = 'W';
                    break;
                case CardColor.black:
                    colorCode = 'B';
                    break;
                case CardColor.red:
                    colorCode = 'R';
                    break;
                case CardColor.blue:
                    colorCode = 'U';
                    break;
                case CardColor.green:
                    colorCode = 'G';
                    break;
                default:
                    break;
            }
            return this.ManaCost.Count(c => c == colorCode);
        }

        public int GetAnyCost()
        {
            if (null == this.ManaCost || this.ManaCost.Length == 0)
            {
                return -1;
            }
            return int.Parse(this.ManaCost.Substring(1, 1));
        }

        override
        public string ToString()
        {
            return $"Card<{this.Name} : {this.Type} - {this.SetCode}>";
        }
    }

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WhiteLands { get; set; }
        public int BlackLands { get; set; }
        public int RedLands { get; set; }
        public int BlueLands { get; set; }
        public int GreenLands { get; set; }

        public void addLand(CardColor color, int num)
        {
            switch (color)
            {
                case CardColor.white:
                    this.WhiteLands += num;
                    break;
                case CardColor.black:
                    this.BlackLands += num;
                    break;
                case CardColor.red:
                    this.RedLands += num;
                    break;
                case CardColor.blue:
                    this.BlueLands += num;
                    break;
                case CardColor.green:
                    this.GreenLands += num;
                    break;
                default:
                    break;
            }
        }

        public void setLand(CardColor color, int num)
        {
            switch (color)
            {
                case CardColor.white:
                    this.WhiteLands = num;
                    break;
                case CardColor.black:
                    this.BlackLands = num;
                    break;
                case CardColor.red:
                    this.RedLands = num;
                    break;
                case CardColor.blue:
                    this.BlueLands = num;
                    break;
                case CardColor.green:
                    this.GreenLands = num;
                    break;
                default:
                    break;
            }
        }
        override
        public string ToString()
        {
            return $"Player({this.Id})<{this.Name} {{W:{this.WhiteLands}}} {{B:{this.BlackLands}}} {{R:{this.RedLands}}} {{U:{this.BlueLands}}} {{G:{this.GreenLands}}}>";
        }
    }

    public class PlayerCard : Card
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Count { get; set; }

        public PlayerCard(int playerId, Card card)
        {
            this.PlayerId = playerId;
            this.MtgId = card.MtgId;
            this.Name = card.Name;
            this.Type = card.Type;
            this.ManaCost = card.ManaCost;
            this.ImageUrl = card.ImageUrl;
            this.SetCode = card.SetCode;
            this.Count = 1;
        }

        override
        public string ToString()
        {
            return $"Card({this.Count:00})<{this.Name} : {this.Type} - {this.SetCode}>({this.ManaCost})";
        }
    }
    public class DataAccess
    {
        private const string PLAYER_COLLECTION = "player";
        private const string CARDS_COLLECTION = "cards";
        private readonly LiteDB.LiteDatabase db;
        public DataAccess()
        {
            string location = MtgLibrary.Settings.Get(MtgLibrary.Settings.DB_LOCATION_PROPERTY) ?? "";
            string name = MtgLibrary.Settings.Get(MtgLibrary.Settings.DB_NAME_PROPERTY) ?? "mtg.db";
            db = new LiteDB.LiteDatabase(name + location);
        }

        // PLAYER Methods
        public Task<Player> AsyncAddPlayer(Player player)
        {
            return Task.Run(() => this.AddPlayer(player));
        }

        public Task<List<Player>> AsyncGetPlayers()
        {
            return Task.Run(() => this.GetPlayers());
        }

        public Task<Player> AsyncGetPlayerById(int id)
        {
            return Task.Run(() => this.GetPlayerById(id));
        }

        public Task<bool> AsyncUpdatePlayer(Player player)
        {
            return Task.Run(() => this.UpdatePlayer(player));
        }

        public Player AddPlayer(Player player)
        {
            var players = db.GetCollection<Player>(PLAYER_COLLECTION);
            int id = players.Insert(player).AsInt32;
            return player;
        }

        public List<Player> GetPlayers()
        {
            return db.GetCollection<Player>(PLAYER_COLLECTION).FindAll().ToList();
        }

        public Player GetPlayerById(int id)
        {
            return db.GetCollection<Player>(PLAYER_COLLECTION).FindById(new LiteDB.BsonValue(id));
        }

        public bool UpdatePlayer(Player player)
        {
            return db.GetCollection<Player>(PLAYER_COLLECTION).Update(player);
        }

        // CARD Methods
        public Task<PlayerCard> AsyncAddPlayerCard(int playerId, Card card)
        {
            return Task.Run(() => this.AddPlayerCard(playerId, card));

        }

        public async Task<List<PlayerCard>> AsyncGetPlayerCards(int playerId)
        {
            return await Task.Run(() => this.GetPlayerCards(playerId));
        }

        public Task<List<PlayerCard>> AsyncFindPlayerCards(int playerId, string name, string type, string setCode)
        {
            return Task.Run(() => this.FindPlayerCards(playerId, name, type, setCode));
        }

        public PlayerCard AddPlayerCard(in int playerId, in Card card)
        {
            PlayerCard playerCard = new PlayerCard(playerId, card);
            var cards = db.GetCollection<PlayerCard>(CARDS_COLLECTION);
            int id = cards.Insert(playerCard).AsInt32;
            return playerCard;
        }

        public List<PlayerCard> GetPlayerCards(int playerId)
        {
            return FindPlayerCards(playerId, null, null, null);
        }

        public List<PlayerCard> FindPlayerCards(int playerId, string name, string type, string setCode)
        {
            return db.GetCollection<PlayerCard>(CARDS_COLLECTION)
                                    .Find(c =>
                                          c.PlayerId == playerId
                                        && (name == null || c.Name.Contains(name))
                                        && (type == null || c.Type.Equals(type))
                                        && (setCode == null || c.SetCode.Equals(setCode))
                                    )
                                    .ToList();

        }
        ~DataAccess()
        {
            db.Dispose();
        }
    }
}