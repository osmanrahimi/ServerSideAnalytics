using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSideAnalytics.Extensions
{
    public static class Extensions
    {
        public static async Task<double> DailyAverage(this IAnalyticStore analyticStore,
            DateTime from, DateTime to)
        {
            return (await analyticStore.CountAsync(from, to)) / (to - from).TotalDays;
        }

        public static async Task<IEnumerable<(DateTime Day, long Served)>> DailyEvera
            (this IAnalyticStore analyticStore, DateTime from, DateTime to)
        {
            return (await analyticStore.(from, to)) / (to - from).TotalDays;
        }
    }
}
