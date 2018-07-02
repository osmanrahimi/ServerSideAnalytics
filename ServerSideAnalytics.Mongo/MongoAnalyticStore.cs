using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Maddalena;
using MongoDB.Driver;

namespace ServerSideAnalytics.Mongo
{
    public class MongoAnalyticStore : IAnalyticStore
    {
        private static readonly IMapper Mapper;
        private readonly IMongoCollection<MongoWebRequest> _requestCollection;
        private readonly IMongoCollection<MongoGeoIpRange> _geoIpCollection;

        static MongoAnalyticStore()
        {
             var config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<WebRequest, MongoWebRequest>()
                     .ForMember(dest => dest.RemoteIpAddress, x => x.MapFrom(req => req.RemoteIpAddress.ToString()));

                 cfg.CreateMap<MongoWebRequest, WebRequest>()
                     .ForMember(dest => dest.RemoteIpAddress, x => x.MapFrom(req => IPAddress.Parse(req.RemoteIpAddress)));
             });

            Mapper = config.CreateMapper();
        }

        public MongoAnalyticStore() : this("mongodb://localhost/default", "SsaRequests", "SsaGeoIp")
        {
        }

        public MongoAnalyticStore(string requestCollectionName, string geoIpCollectionName) : this("mongodb://localhost/default", requestCollectionName, geoIpCollectionName)
        {
            
        }

        public MongoAnalyticStore(string connectionString, string requestCollectionName, string geoIpCollectionName)
        {
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(connectionString);
            _requestCollection = client.GetDatabase(url.DatabaseName ?? "default").GetCollection<MongoWebRequest>(requestCollectionName);
            _geoIpCollection = client.GetDatabase(url.DatabaseName ?? "default").GetCollection<MongoGeoIpRange>(geoIpCollectionName);
        }

        public Task StoreWebRequestAsync(WebRequest request)
        {
            return _requestCollection.InsertOneAsync(Mapper.Map<MongoWebRequest>(request));
        }

        public Task<long> CountUniqueAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return CountUniqueAsync(from, to);
        }

        public async Task<long> CountUniqueAsync(DateTime from, DateTime to)
        {
            var identities = await _requestCollection.DistinctAsync(x => x.Identity, x => x.Timestamp >= from && x.Timestamp <= to);
            return identities.ToEnumerable().Count();
        }

        public Task<long> CountAsync(DateTime from, DateTime to)
        {
            return _requestCollection.CountDocumentsAsync(x => x.Timestamp >= from && x.Timestamp <= to);
        }

        public Task<IEnumerable<string>> IpAddressesAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return IpAddressesAsync(from, to);
        }

        public async Task<IEnumerable<string>> IpAddressesAsync(DateTime from, DateTime to)
        {
            var identities = await _requestCollection.DistinctAsync(x => x.RemoteIpAddress, x => x.Timestamp >= from && x.Timestamp <= to);
            return identities.ToEnumerable();
        }

        public async Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity)
        {
            var identities = await _requestCollection.FindAsync(x => x.Identity == identity);
            return identities.ToEnumerable().Select( x => Mapper.Map<WebRequest>(x));
        }

        public Task StoreGeoIpRangeAsync(IPAddress from, IPAddress to, CountryCode countryCode)
        {
            var bytesFrom = from.GetAddressBytes();
            var bytesTo = to.GetAddressBytes();

            Array.Resize(ref bytesFrom, 16);
            Array.Resize(ref bytesTo, 16);

            return _geoIpCollection.InsertOneAsync(new MongoGeoIpRange
            {
                FromDown = BitConverter.ToInt64(bytesFrom,0),
                FromUp = BitConverter.ToInt64(bytesFrom,8),

                ToDown = BitConverter.ToInt64(bytesTo,0),
                ToUp = BitConverter.ToInt64(bytesTo,8),
                CountryCode = countryCode
            });
        }

        public async Task<CountryCode> ResolveCountryCodeAsync(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            Array.Resize(ref bytes, 16);

            var down = BitConverter.ToInt64(bytes, 0);
            var up = BitConverter.ToInt64(bytes, 8);

            var found = await _geoIpCollection.FindAsync(x => x.FromDown <= down && x.ToDown >= down && x.FromUp <= up && x.ToUp >= up);
            return (await found.FirstOrDefaultAsync())?.CountryCode ?? CountryCode.World;
        }
    }
}