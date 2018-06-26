using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ServerSideAnalytics
{
    public class InMemoryContextFilter : IContextFilter
    {
        public string[] ExcludedFolders { get; set; }

        public string[] ExcludedExtensions { get; set; }

        public bool FilterLocalNetwork { get; set; }

        public bool FilterLoopback { get; set; }

        public bool IsRelevant(HttpContext context)
        {
            if ((ExcludedFolders?.Any(x => context.Request.Path.Value.StartsWith(x)) ?? false) ||
                (ExcludedExtensions?.Any(x => context.Request.Path.Value.EndsWith(x)) ?? false)) return false;

            var ipAddress = context.Connection.RemoteIpAddress;

            if (FilterLocalNetwork)
            {
                var bytes = ipAddress.GetAddressBytes();

                if (bytes.Length == 4)
                {
                    if (bytes[0] == 10) return false;
                    if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return false;
                    if (bytes[0] == 192 && bytes[1] == 168) return false;
                }
            }

            if (FilterLoopback) return !IPAddress.IsLoopback(ipAddress);

            return true;
        }
    }
}