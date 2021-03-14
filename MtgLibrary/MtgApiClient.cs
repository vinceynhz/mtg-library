using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using MtgData;

namespace MtgApiClient
{
    public static class CardService
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<List<Card>> AsyncSearchCardByName(string name)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var stringUrl = "https://api.magicthegathering.io/v1/cards?name=" + name;
            var streamTask = client.GetStreamAsync(stringUrl);

            var result = await JsonSerializer.DeserializeAsync<CardResponse>(await streamTask);
            return result.Cards;
        }
    }

    class CardResponse
    {
        [JsonPropertyName("cards")]
        public List<Card> Cards { get; set; }
    }
}