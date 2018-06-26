using System.Net;
using Maddalena;

namespace ServerSideAnalytics
{
    public interface IGeoIpResolver
    {
        CountryCode GetCountry(IPAddress address);
    }
}
