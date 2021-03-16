using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using MtgData;

namespace MtgLibrary
{
    public static class CardService
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<List<MtgData.Card>> AsyncSearchCard(string name, string type, string set)
        {
            var baseUrl = await Settings.AsyncGet(Settings.API_MTG_URL_PROPERTY);
            var cardPath = await Settings.AsyncGet(Settings.API_MTG_CARD_PATH_PROPERTY);
            var url = baseUrl + cardPath + "?name=" + name;

            if (null != type && type.Length > 0)
            {
                url += "&type=" + type;
            }

            if (null != set && set.Length > 0)
            {
                url += "&set=" + set;
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var streamTask = client.GetStreamAsync(url);
                var result = await JsonSerializer.DeserializeAsync<CardResponse>(await streamTask);
                result.Cards.Sort(delegate (Card x, Card y)
                {
                    if (null == x.Name && null == y.Name) return 0;
                    else if (null == x.Name) return -1;
                    else if (null == y.Name) return 1;
                    else return x.Name.CompareTo(y.Name);
                });
                return result.Cards;
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine($"Error executing request: {exception.Message}");
                Console.WriteLine($"Requested URL: {url}");
            }
            catch (TaskCanceledException exception)
            {
                Console.WriteLine($"Timeout executing request: {exception.Message}");
                Console.WriteLine($"Requested URL: {url}");
            }
            return null;
        }
    }

    class CardResponse
    {
        [JsonPropertyName("cards")]
        public List<MtgData.Card> Cards { get; set; }
    }
}