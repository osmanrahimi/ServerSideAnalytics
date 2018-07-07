using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSideAnalytics
{
    public class FluidAnalyticBuilder
    {
        private readonly IAnalyticStore _store;
        private List<Func<HttpContext, bool>> _exclude;

        internal FluidAnalyticBuilder(IAnalyticStore store)
        {
            _store = store;
        }

        internal async Task Run(HttpContext context, Func<Task> next)
        {
            if (_exclude?.Any(x => x(context)) ?? false)
            {
                await next.Invoke();
                return;
            }

            var req = new WebRequest
            {
                Timestamp = DateTime.Now,
                Identity = context.UserIdentity(),
                RemoteIpAddress = context.Connection.RemoteIpAddress,
                Method = context.Request.Method,
                UserAgent = context.Request.Headers["User-Agent"],
                Path = context.Request.Path.Value,
                IsWebSocket = context.WebSockets.IsWebSocketRequest,
                CountryCode = await _store.ResolveCountryCodeAsync(context.Connection.RemoteIpAddress)
            };

            await _store.StoreWebRequestAsync(req);
            await next.Invoke();
        }

        public FluidAnalyticBuilder Exclude(Func<HttpContext, bool> filter)
        {
            if(_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public FluidAnalyticBuilder Exclude(IPAddress ip) => Exclude(x => Equals(x.Connection.RemoteIpAddress, ip));

        public FluidAnalyticBuilder LimitToPath(string path) => Exclude(x => !Equals(x.Request.Path.StartsWithSegments(path)));

        public FluidAnalyticBuilder ExcludePath(params string[] paths)
        {
            return Exclude(x => paths.Any(path => x.Request.Path.StartsWithSegments(path)));
        }

        public FluidAnalyticBuilder ExcludeExtension(params string[] extensions)
        {
            return Exclude(x => extensions.Any(ext => x.Request.Path.Value.EndsWith(ext)));
        }

        public FluidAnalyticBuilder ExcludeLoopBack() => Exclude(x => IPAddress.IsLoopback(x.Connection.RemoteIpAddress));

        public FluidAnalyticBuilder ExcludeIp(IPAddress address) => Exclude(x => x.Connection.RemoteIpAddress.Equals(address));
    }
}