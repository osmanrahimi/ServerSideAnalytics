using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Maddalena;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ServerSideAnalytics
{
    public class FluidAnalyticBuilder<T> where T : IWebRequest
    {
        private readonly IApplicationBuilder _app;
        private readonly IWebRequestStore<T> _repository;
        private List<Func<HttpContext, bool>> _exclude;
        private Func<IPAddress, CountryCode> _geoResolve;

        private static string UserIdentity(HttpContext context)
        {
            var user = context.User?.Identity?.Name;

            return string.IsNullOrWhiteSpace(user)
                ? (context.Request.Cookies.ContainsKey("ai_user")
                    ? context.Request.Cookies["ai_user"]
                    : context.Connection.Id)
                : user;
        }

        internal FluidAnalyticBuilder(IApplicationBuilder app, IWebRequestStore<T> repository)
        {
            _app = app;
            _repository = repository;
        }

        internal async Task Run(HttpContext context, Func<Task> next)
        {
            if (_exclude?.Any(x => x(context)) ?? false)
            {
                await next.Invoke();
                return;
            }

            var req = _repository.GetNew();
            req.Timestamp = DateTime.Now;

            req.Identity = UserIdentity(context);
            req.RemoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            req.Method = context.Request.Method;
            req.UserAgent = context.Request.Headers["User-Agent"];
            req.Path = context.Request.Path.Value;

            req.Country = _geoResolve?.Invoke(context.Connection.RemoteIpAddress) ?? CountryCode.World;

            await _repository.AddAsync(req);
            await next.Invoke();
        }

        public FluidAnalyticBuilder<T> Exclude(Func<HttpContext, bool> filter)
        {
            if(_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public FluidAnalyticBuilder<T> Exclude(IPAddress ip) => Exclude(x => Equals(x.Connection.RemoteIpAddress, ip));

        public FluidAnalyticBuilder<T> ExcludePath(string path) => Exclude(x => Equals(x.Request.Path.StartsWithSegments(path)));

        public FluidAnalyticBuilder<T> LimitToPath(string path) => Exclude(x => !Equals(x.Request.Path.StartsWithSegments(path)));

        public FluidAnalyticBuilder<T> ExcludeExtension(string extension) => Exclude(x => x.Request.Path.Value?.EndsWith(extension) ?? false);

        public FluidAnalyticBuilder<T> ExcludeLoopBack() => Exclude(x => IPAddress.IsLoopback(x.Connection.RemoteIpAddress));

        public FluidAnalyticBuilder<T> UseGeoIp(Func<IPAddress, CountryCode> geoResolve)
        {
            _geoResolve = geoResolve;
            return this;
        }

        public IApplicationBuilder ApplicationBuilder() => _app;
    }
}