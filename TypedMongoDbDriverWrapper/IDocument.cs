using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TypedMongoDbDriverWrapper
{
    public interface IDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
