using Maddalena;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerSideAnalytics.Mongo
{
    internal class MongoGeoIpRange
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public long FromUp { get; set; }
        public long FromDown { get; set; }

        public long ToUp { get; set; }
        public long ToDown { get; set; }

        public CountryCode CountryCode { get; set; }
    }
}
