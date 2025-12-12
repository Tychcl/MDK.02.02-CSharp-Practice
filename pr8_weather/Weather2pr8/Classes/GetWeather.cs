using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Weather2pr8.Classes
{
    public class GetWeather
    {
        private readonly static string ApiKey = "demo_yandex_weather_api_key_ca6d09349ba0";
        private readonly static string url = "https://api.weather.yandex.ru/v2/forecast";

        public static async Task<Classes.Response> Get(float lat, float lon)
        {
            Response response = new Response();
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{url}?lat={lat}&lon={lon}&lang=ru_RU&limit=7".Replace(',', '.')))
                {
                    request.Headers.Add("X-Yandex-Weather-Key", ApiKey);
                    using (var r = await client.SendAsync(request))
                    {
                        response = JsonConvert.DeserializeObject<Response>(await r.Content.ReadAsStringAsync());
                    }
                }
            }
            return response;
        }
    }
}
