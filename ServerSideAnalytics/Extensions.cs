using Microsoft.AspNetCore.Builder;

namespace ServerSideAnalytics
{
    public static class Extensions
    {
        public static FluidAnalyticBuilder UseServerSideAnalytics(this IApplicationBuilder app, IAnalyticStore repository)
        {
            var builder = new FluidAnalyticBuilder(app,repository);
            app.Use(builder.Run);
            return builder;
        }
    }
}
