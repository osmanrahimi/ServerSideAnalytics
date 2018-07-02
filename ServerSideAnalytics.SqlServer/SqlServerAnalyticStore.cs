using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqlServer
{
    public class SqlServerAnalyticStore : IAnalyticStore
    {
        private static readonly IMapper Mapper;
        private readonly string _connectionString;

        public SqlServerAnalyticStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        static SqlServerAnalyticStore()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WebRequest, SqlServerWebRequest>();
                cfg.CreateMap<SqlServerWebRequest, WebRequest>();
            });

            Mapper = config.CreateMapper();
        }

        public async Task AddAsync(WebRequest request)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                await db.WebRequest.AddAsync(Mapper.Map<SqlServerWebRequest>(request));
                await db.SaveChangesAsync();
            }
        }

        public async Task AddAsync(SqlServerWebRequest request)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                await db.WebRequest.AddAsync(request);
                await db.SaveChangesAsync();
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
                return await db.WebRequest.Where(x => x.Timestamp >= from && x.Timestamp <= to).GroupBy(x => x.Identity).CountAsync();
            }
        }

        public async Task<long> CountAsync(DateTime from, DateTime to)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Timestamp >= from && x.Timestamp <= to).CountAsync();
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
                return await db.WebRequest.Where(x => x.Timestamp >= from && x.Timestamp <= to).Select(x => x.Identity)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<SqlServerWebRequest>> RequestByIdentityAsync(string identity)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Identity == identity).ToListAsync();
            }
        }
    }
}