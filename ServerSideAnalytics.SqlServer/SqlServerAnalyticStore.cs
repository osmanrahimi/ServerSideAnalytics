using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Maddalena;
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
                cfg.CreateMap<WebRequest, SqlServerWebRequest>()
                    .ForMember(dest => dest.RemoteIpAddress, x => x.MapFrom(req => req.RemoteIpAddress.ToString()));

                cfg.CreateMap<SqlServerWebRequest, WebRequest>()
                    .ForMember(dest => dest.RemoteIpAddress, x => x.MapFrom(req => IPAddress.Parse(req.RemoteIpAddress)));
            });

            config.AssertConfigurationIsValid();

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

        public Task<IEnumerable<string>> IpAddressesAsync(DateTime day)
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

        public async Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity)
        {
            using (var db = new SqlServerContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Identity == identity).Select( x=> Mapper.Map<WebRequest>(x)).ToListAsync();
            }
        }

        public async Task StoreGeoIpRangeAsync(IPAddress from, IPAddress to, CountryCode countryCode)
        {
            var bytesFrom = from.GetAddressBytes();
            var bytesTo = to.GetAddressBytes();

            Array.Resize(ref bytesFrom, 16);
            Array.Resize(ref bytesTo, 16);

            using (var db = new SqlServerContext(_connectionString))
            {
                await db.GeoIpRange.AddAsync(new SqlServerGeoIpRange
                {
                    FromDown = BitConverter.ToInt64(bytesFrom, 0),
                    FromUp = BitConverter.ToInt64(bytesFrom, 8),

                    ToDown = BitConverter.ToInt64(bytesTo, 0),
                    ToUp = BitConverter.ToInt64(bytesTo, 8),
                    CountryCode = countryCode
                });
            }
        }
    }
}