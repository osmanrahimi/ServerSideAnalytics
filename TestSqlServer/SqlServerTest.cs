using System;
using System.Threading.Tasks;
using ServerSideAnalytics.SqlServer;
using Xunit;

namespace TestSqlServer
{
    public class SqlServerTest
    {
        [Fact]
        public async Task TestSqlServerAsync()
        {
            await TestBase.StoreTests.RunAll(async () =>
            {
                const string filePath = "test.db";

                var db = new SqlServerAnalyticStore("Server=localhost;Database=test;Trusted_Connection=True");
                await db.PurgeRequestAsync();
                await db.PurgeGeoIpAsync();
                return db;
            });
        }
    }
}
