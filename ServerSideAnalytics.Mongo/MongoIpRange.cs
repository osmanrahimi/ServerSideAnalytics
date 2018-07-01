using Maddalena;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ServerSideAnalytics
{
    public class MongoIpRange : IIpRange
    {
        [BsonId]
        public string Id { get; set; }

        public CountryCode Country { get; set; }

        public long TopFrom { get; set; }
        public long TopTo { get; set; }

        public long DownFrom { get; set; }
        public long DownTo { get; set; }
    }
}
