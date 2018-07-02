using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Maddalena;

namespace ServerSideAnalytics
{
    public interface IAnalyticStore
    {
        Task StoreWebRequestAsync(WebRequest request);

        Task<long> CountUniqueAsync(DateTime day);

        Task<long> CountUniqueAsync(DateTime from, DateTime to);

        Task<long> CountAsync(DateTime from, DateTime to);

        Task<IEnumerable<string>> IpAddressesAsync(DateTime day);

        Task<IEnumerable<string>> IpAddressesAsync(DateTime from, DateTime to);

        Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity);

        Task StoreGeoIpRangeAsync(IPAddress from, IPAddress to, CountryCode countryCode);

        Task<CountryCode> ResolveCountryCodeAsync(IPAddress address);
    }
}