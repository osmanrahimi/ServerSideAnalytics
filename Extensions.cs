using System;
using Maddalena;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ServerSideAnalytics
{
    public static class Extensions
    {
        public static string UserIdentity(this HttpContext context)
        {
            var user = context.User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(user))
            {
                return context.Request.Cookies.ContainsKey("ai_user")
                    ? context.Request.Cookies["ai_user"]
                    : context.Connection.Id;
            }

            return user;
        }

        public static IApplicationBuilder UseServerSideAnalytics<T>(this IApplicationBuilder app, IWebRequestStore<T> repository, IContextFilter filter=null, IGeoIpResolver geoIp = null) where T : IWebRequest
        {
            app.Use(async (context, next) =>
            {
                if (filter?.IsRelevant(context) ?? false)
                {
                    var req = repository.GetNew();
                    req.Timestamp = DateTime.Now;

                    req.Identity = UserIdentity(context);
                    req.RemoteIpAddress = context.Connection.RemoteIpAddress.ToString();
                    req.Method = context.Request.Method;
                    req.UserAgent = context.Request.Headers["User-Agent"];
                    req.Path = context.Request.Path.Value;

                    req.Country = geoIp?.GetCountry(context.Connection.RemoteIpAddress) ?? CountryCode.World;

                    await repository.AddAsync(req);
                }
                await next.Invoke();
            });
            return app;
        }
    }
}
