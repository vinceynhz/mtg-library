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
            return "name: " + this.Name + "\n" +
                    "type: " + this.Type + "\n" +
                    "set: " + this.SetCode + "\n";
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
    }

    public class DataAccess
    {
        private readonly LiteDB.LiteDatabase db;
        public DataAccess()
        {
            db = new LiteDB.LiteDatabase("mtg.db");
        }

        public async Task<int> AsyncAddPlayer(Player player)
        {
            await Task.Run(() => this.AddPlayer(player));
            return 0;
        }

        public void AddPlayer(Player player)
        {
            var players = db.GetCollection<MtgData.Player>("player");
            players.Insert(player);
        }
        ~DataAccess()
        {
            db.Dispose();
        }
    }
}