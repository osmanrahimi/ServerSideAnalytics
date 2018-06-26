using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ServerSideAnalytics.Mongo
{
    public class MongoRequestStore : IWebRequestStore<MongoWebRequest>
    {
        private readonly IMongoCollection<MongoWebRequest> _mongoCollection;

        public MongoRequestStore()
        {
            _mongoCollection = (new MongoClient()).GetDatabase("default").GetCollection<MongoWebRequest>("serverSideAnalytics");
        }

        public MongoRequestStore(string collectionName)
        {
            _mongoCollection = (new MongoClient()).GetDatabase("default").GetCollection<MongoWebRequest>(collectionName);
        }

        public MongoRequestStore(string connectionString, string collectionName)
        {
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(connectionString);
            _mongoCollection = client.GetDatabase(url.DatabaseName ?? "default").GetCollection<MongoWebRequest>(collectionName);
        }

        public MongoWebRequest GetNew() => new MongoWebRequest();

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

        public Task<IEnumerable<string>> IpAddresses(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return IpAddresses(from, to);
        }

        public async Task<IEnumerable<string>> IpAddresses(DateTime from, DateTime to)
        {
            var identities = await _mongoCollection.DistinctAsync(x => x.RemoteIpAddress, x => x.Timestamp >= from && x.Timestamp <= to);
            return identities.ToEnumerable();
        }

        public async Task<IEnumerable<IWebRequest>> RequestByIdentity(string identity)
        {
            var identities = await _mongoCollection.FindAsync(x => x.Identity == identity);
            return identities.ToEnumerable();
        }

        public async Task<IEnumerable<IWebRequest>> QueryAsync(Expression<Func<MongoWebRequest, bool>> filter)
        {
            return (await _mongoCollection.FindAsync(filter)).ToEnumerable();
        }

        public async Task<IEnumerable<T>> DistinctAsync<T>(Expression<Func<MongoWebRequest, T>> field, Expression<Func<MongoWebRequest, bool>> filter)
        {
            return (await _mongoCollection.DistinctAsync(field,filter)).ToEnumerable();
        }
    }
}