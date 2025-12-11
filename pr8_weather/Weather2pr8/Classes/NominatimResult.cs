using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Weather2pr8.Classes
{
    public class NominatimResult
    {
        public string lat { get; set; }
        public string lon { get; set; }

        public static async Task<NominatimResult> Get(string name)
        {
            NominatimResult response = new NominatimResult();
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://nominatim.openstreetmap.org/search?format=jsonv2&q={WebUtility.UrlEncode(name)}"))
                {
                    request.Headers.Add("User-Agent", "Tychkclpr8HELP/1.0");
                    using (var r = await client.SendAsync(request))
                    {
                        string json = await r.Content.ReadAsStringAsync();
                        var list = JsonConvert.DeserializeObject<List<NominatimResult>>(json);
                        if(list != null && list.Count != 0 )
                        {
                            response = list[0];
                        }
                    }
                }
            }
            return response;
        } 
    }
}
