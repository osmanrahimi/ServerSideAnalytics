using System;
using System.IO;
using System.Threading.Tasks;
using ServerSideAnalytics.SqLite;
using Xunit;

namespace TestSqLite
{
    public class SqLiteTest
    {
        [Fact]
        public async Task TestSqLiteAsync()
        {
            await TestBase.StoreTests.RunAll(async () =>
            {
                const string filePath = "test.db";

                var db = new SqLiteAnalyticStore(filePath);
                await db.PurgeRequestAsync();
                await db.PurgeGeoIpAsync();
                return db;
            });
        }
    }
}
