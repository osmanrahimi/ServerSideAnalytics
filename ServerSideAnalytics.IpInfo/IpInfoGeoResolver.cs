using System;
using System.Net;
using Maddalena;
using Newtonsoft.Json;

namespace ServerSideAnalytics.IpInfo
{
    class record
    {
        public string ip;
        public string city;
        public string region;
        public string country;
        public string loc;
        public string postal;
        public string phone;
        public string org;
    }

    public class IpInfoGeoResolver 
    {
        public static CountryCode GetCountry(string ipAddress)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<record>((new WebClient()).DownloadString($"https://ipinfo.io/{ipAddress}/json"));
                return (CountryCode)Enum.Parse(typeof(CountryCode), obj.country);
            }
            catch (Exception e)
            {
                return CountryCode.World;
            }
        }

        public static CountryCode GetCountry(IPAddress remoteIpAddress) => GetCountry(remoteIpAddress.ToString());
    }
}
