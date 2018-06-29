using Maddalena;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerSideAnalytics.IpInfo
{
    public class IpInfoGeoIp
    {
        public async static Task<CountryCode> GetCountry(IPAddress ip)
        {
            try
            {
                var ipstr = "104.28.19.81";
                var response = await (new HttpClient()).GetStringAsync($"https://ipinfo.io/{ipstr}/json");
                
                var obj = JsonConvert.DeserializeObject(response) as JObject;
                return (CountryCode) Enum.Parse(typeof(CountryCode), obj["country"].ToString());
            }
            catch (Exception)
            {
                return CountryCode.World;
            }
        }
    }
}
