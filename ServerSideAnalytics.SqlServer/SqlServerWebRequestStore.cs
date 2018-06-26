using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqlServer
{
    public class SqlServerWebRequestStore : IWebRequestStore<EntityFrameworkWebRequest>
    {
        private readonly string _connectionString;

        protected SqlServerWebRequestStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public EntityFrameworkWebRequest GetNew() => new EntityFrameworkWebRequest();

        public async Task AddAsync(EntityFrameworkWebRequest request)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                await db.AddAsync(request);
            }
        }

        public Task<long> CountUniqueAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return CountUniqueAsync(from, to);
        }

        public async Task<long> CountUniqueAsync(DateTime from, DateTime to)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to).GroupBy(x => x.Identity).CountAsync();
            }
        }

        public async Task<long> CountAsync(DateTime from, DateTime to)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to).CountAsync();
            }
        }

        public Task<IEnumerable<string>> IpAddresses(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return IpAddressesAsync(from, to);
        }

        public async Task<IEnumerable<string>> IpAddressesAsync(DateTime from, DateTime to)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to).Select(x => x.Identity)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<IWebRequest>> RequestByIdentityAsync(string identity)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.Requests.Where(x => x.Identity == identity).ToListAsync();
            }
        }
    }
}