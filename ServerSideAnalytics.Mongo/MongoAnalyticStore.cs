using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;

namespace ServerSideAnalytics.Mongo
{
    public class MongoAnalyticStore : IAnalyticStore
    {
        private static readonly IMapper Mapper;
        private readonly IMongoCollection<MongoWebRequest> _mongoCollection;

        static MongoAnalyticStore()
        {
             var config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<WebRequest, MongoWebRequest>();
                 cfg.CreateMap<MongoWebRequest, WebRequest>();
             });

            Mapper = config.CreateMapper();
        }

        public MongoAnalyticStore()
        {
            _mongoCollection = (new MongoClient()).GetDatabase("default").GetCollection<MongoWebRequest>("serverSideAnalytics");
        }

        public MongoAnalyticStore(string collectionName)
        {
            _mongoCollection = (new MongoClient()).GetDatabase("default").GetCollection<MongoWebRequest>(collectionName);
        }

        public MongoAnalyticStore(string connectionString, string collectionName)
        {
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(connectionString);
            _mongoCollection = client.GetDatabase(url.DatabaseName ?? "default").GetCollection<MongoWebRequest>(collectionName);
        }

        public Task AddAsync(WebRequest request)
        {
            return _mongoCollection.InsertOneAsync(Mapper.Map<MongoWebRequest>(request));
        }

        public Task AddAsync(MongoWebRequest request) => _mongoCollection.InsertOneAsync(request);

        public Task<long> CountUniqueAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return CountUniqueAsync(from, to);
        }

        public async Task<long> CountUniqueAsync(DateTime from, DateTime to)
        {
            var identities = await _mongoCollection.DistinctAsync(x => x.Identity, x => x.Timestamp >= from && x.Timestamp <= to);
            return identities.ToEnumerable().Count();
        }

        public Task<long> CountAsync(DateTime from, DateTime to)
        {
            return _mongoCollection.CountAsync(x => x.Timestamp >= from && x.Timestamp <= to);
        }

        public Task<IEnumerable<string>> IpAddressesAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return IpAddressesAsync(from, to);
        }

        public async Task<IEnumerable<string>> IpAddressesAsync(DateTime from, DateTime to)
        {
            var identities = await _mongoCollection.DistinctAsync(x => x.RemoteIpAddress, x => x.Timestamp >= from && x.Timestamp <= to);
            return identities.ToEnumerable();
        }

        public async Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity)
        {
            var identities = await _mongoCollection.FindAsync(x => x.Identity == identity);
            return identities.ToEnumerable().Select( x => Mapper.Map<WebRequest>(x));
        }

        public async Task<IEnumerable<MongoWebRequest>> QueryAsync(Expression<Func<MongoWebRequest, bool>> filter)
        {
            return (await _mongoCollection.FindAsync(filter)).ToEnumerable();
        }

        public async Task<IEnumerable<T>> DistinctAsync<T>(Expression<Func<MongoWebRequest, T>> field, Expression<Func<MongoWebRequest, bool>> filter)
        {
            return (await _mongoCollection.DistinctAsync(field,filter)).ToEnumerable();
        }

    }
}