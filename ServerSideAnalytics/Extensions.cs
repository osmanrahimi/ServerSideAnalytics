using Microsoft.AspNetCore.Builder;

namespace ServerSideAnalytics
{
    public static class Extensions
    {
        public static FluidAnalyticBuilder<T> UseServerSideAnalytics<T>(this IApplicationBuilder app, IWebRequestStore<T> repository) where T : IWebRequest
        {
            var builder = new FluidAnalyticBuilder<T>(app,repository);
            app.Use(builder.Run);
            return builder;
        }
    }
}
