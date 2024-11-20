using System.Collections.Generic;
using System.IO;
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
                LoggingService.Instance.Error($"Error executing request: {exception.Message}");
                LoggingService.Instance.Error($"Requested URL: {url}");
            }
            catch (TaskCanceledException exception)
            {
                LoggingService.Instance.Error($"Timeout executing request: {exception.Message}");
                LoggingService.Instance.Error($"Requested URL: {url}");
            }
            return null;
        }

        public static async Task<Stream> AsyncLoadImage(string url)
        {
            try
            {
                LoggingService.Instance.Info($"Loading image: {url}");
                Stream stream = await client.GetStreamAsync(url);
                MemoryStream memoryStream = await Task.Run(() => ToMemoryStream(stream));
                return memoryStream;
            }
            catch (HttpRequestException exception)
            {
                LoggingService.Instance.Error($"Error executing request: {exception.Message}");
                LoggingService.Instance.Error($"Requested URL: {url}");
            }
            catch (TaskCanceledException exception)
            {
                LoggingService.Instance.Error($"Timeout executing request: {exception.Message}");
                LoggingService.Instance.Error($"Requested URL: {url}");
            }
            return null;
        }

        private static MemoryStream ToMemoryStream(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }

    class CardResponse
    {
        [JsonPropertyName("cards")]
        public List<MtgData.Card> Cards { get; set; }
    }
}