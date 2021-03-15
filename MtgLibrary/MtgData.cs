using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MtgData
{
    public class Card
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
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

        override
        public string ToString()
        {
            return $"Player({this.Id})<{this.Name} {{W:{this.WhiteLands}}} {{K:{this.BlackLands}}} {{R:{this.RedLands}}} {{B:{this.BlueLands}}} {{G:{this.GreenLands}}}>";
        }
    }

    public class DataAccess
    {
        private readonly LiteDB.LiteDatabase db;
        public DataAccess()
        {
            db = new LiteDB.LiteDatabase("mtg.db");
        }

        public async Task<Player> AsyncAddPlayer(Player player)
        {
            int added = await Task.Run(() => this.AddPlayer(player));
            return player;
        }

        public async Task<IEnumerable<Player>> AsyncGetPlayers()
        {
            return await Task.Run(() => this.GetPlayers());
        }

        public int AddPlayer(Player player)
        {
            var players = db.GetCollection<Player>("player");
            int id = players.Insert(player).AsInt32;
            return id;
        }

        public IEnumerable<Player> GetPlayers()
        {
            return db.GetCollection<Player>("player").FindAll();
        }
        ~DataAccess()
        {
            db.Dispose();
        }
    }
}