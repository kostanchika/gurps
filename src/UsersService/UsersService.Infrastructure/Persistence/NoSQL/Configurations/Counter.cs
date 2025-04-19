using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersService.Infrastructure.Persistence.NoSQL.Configurations
{
    public class Counter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Value { get; set; }
    }
}
