using MongoDB.Bson.Serialization;

namespace TypedMongoDbDriverWrapper
{
    public interface IBsonSerialization
    {
        public Type Type { get; init; }
        public IBsonSerializer Serializer {get; init; }
    }
}
