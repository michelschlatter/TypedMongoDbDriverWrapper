using MongoDB.Bson;

namespace TypedMongoDbDriverWrapper
{
    public interface IRepository<T> where T : IDocument
    {
        public Task<IList<T>> GetAllAsync(IList<string> ids);

        public Task<IList<T>> GetAllAsync();
        public Task<T?> GetOneAsync(string id);
        public Task<T> GetSingleOrThrowAsync(string id);
        public Task InsertOneAsync(T item);
        public Task InsertManyAsync(IList<T> items);
        public Task ReplaceOneAsync(T item);
        public Task DeleteManyAsync(IList<T> items);
        public Task DeleteManyAsync(IList<ObjectId> ids);
        public Task DeleteManyAsync(IList<string> ids);
        public Task DeleteAsync(T item);
        public Task DeleteAsync(string id);
        public Task<long> CountAllAsync();
        public bool IsValidNotEmptyId(string id);
        public bool IsNotEmptyId(ObjectId id);
        public bool IsEmptyId(ObjectId id);
        public ObjectId GetEmptyId();
        public ObjectId GenerateId();
    }
}
