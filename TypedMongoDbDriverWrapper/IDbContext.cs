using MongoDB.Driver;

namespace TypedMongoDbDriverWrapper
{
    public interface IDbContext
    {
        public IMongoCollection<T> GetCollection<T>() where T : IDocument;
        public Task<bool> CheckCollectionExistsAsync<T>() where T : IDocument;
        public Task DeleteCollectionAsync<T>() where T : IDocument;
        public Task<IClientSessionHandle> StartTransactionSessionAsync();
        public IClientSessionHandle StartTransectionSessionSync();
        public IList<Type> GetCollectionTypes();
        public IList<string> GetCollectionNames();
        public string GetCollectionName<T>() where T : IDocument;
        public Task CreateCollectionsAsync();
        
    }
}
