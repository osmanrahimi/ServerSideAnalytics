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

        Task<long> CountUniqueIndentitiesAsync(DateTime day);

        Task<long> CountUniqueIndentitiesAsync(DateTime from, DateTime to);

        Task<long> CountAsync(DateTime from, DateTime to);

        Task<IEnumerable<IPAddress>> IpAddressesAsync(DateTime day);

        Task<IEnumerable<IPAddress>> IpAddressesAsync(DateTime from, DateTime to);

        Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity);

        Task StoreGeoIpRangeAsync(IPAddress from, IPAddress to, CountryCode countryCode);

        Task<CountryCode> ResolveCountryCodeAsync(IPAddress address);

        Task PurgeRequestAsync();

        Task PurgeGeoIpAsync();
    }
}