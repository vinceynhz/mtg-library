using System.Linq;
using System.Text.Json.Serialization;

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
        public static readonly Card DefaultCard = new Card()
        {
            MtgId = string.Empty,
            Name = string.Empty,
            Type = string.Empty,
            ManaCost = string.Empty,
            ImageUrl = string.Empty,
            SetCode = string.Empty,
            Text = string.Empty,
            Flavor = string.Empty,
            Power = string.Empty,
            Toughness = string.Empty
        };

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
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("flavor")]
        public string Flavor { get; set; }
        [JsonPropertyName("power")]
        public string Power { get; set; }
        [JsonPropertyName("toughness")]
        public string Toughness { get; set; }
        [JsonPropertyName("loyalty")]
        public string Loyalty { get; set; }

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

        public string PowerToughnessLoyalty
        {
            get => this.GetPowerToughnessLoyalty();
        }

        private string GetPowerToughnessLoyalty()
        {
            if (!string.IsNullOrEmpty(Power) || !string.IsNullOrEmpty(Toughness))
            {
                return $"{Power}/{Toughness}";
            }
            else if (!string.IsNullOrEmpty(Loyalty))
            {
                return Loyalty;
            }
            return string.Empty;
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
}