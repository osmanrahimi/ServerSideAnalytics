using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ServerSideAnalytics
{
    public static class Extensions
    {
        public static string UserIdentity(this HttpContext context)
        {
            var user = context.User?.Identity?.Name;

            return string.IsNullOrWhiteSpace(user)
                ? (context.Request.Cookies.ContainsKey("ai_user")
                    ? context.Request.Cookies["ai_user"]
                    : context.Connection.Id)
                : user;
        }

        public static FluidAnalyticBuilder UseServerSideAnalytics(this IApplicationBuilder app, IAnalyticStore repository)
        {
            var builder = new FluidAnalyticBuilder(repository);
            app.Use(builder.Run);
            return builder;
        }
    }
}
