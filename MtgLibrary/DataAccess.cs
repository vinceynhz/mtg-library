using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MtgData
{
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