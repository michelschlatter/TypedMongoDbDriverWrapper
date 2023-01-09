using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace TypedMongoDbDriverWrapper
{
    public class DbContext : MongoClient, IDbContext
    {
        private static readonly List<string> InitializedDbs = new();
        private readonly IMongoDatabase _database;
        private readonly List<IDocumentCollection> _collections;
        private readonly IDictionary<Type, object> _collectionCache = new Dictionary<Type, object>();
        public string DbName { get; }

        public DbContext(string dbName, string connectionString, ICollectionProvider collectionProvider) : base(new MongoUrl(connectionString))
        {
            DbName = dbName;
            _collections = collectionProvider.GetAll().ToList();

            if(InitializedDbs.Contains(dbName)) throw new Exception($"MongoDbClient for DB {dbName} already initialized. Use Singleton.");
            InitializedDbs.Add(dbName);
            _database = GetDatabase(dbName);
        }

        public IList<Type> GetCollectionTypes()
        {
            return _collections.Select(c => c.DocumentType).ToList();
        }

        public IList<string> GetCollectionNames()
        {
            return _collections.Select(c => c.CollectionName).ToList();
        }

        public async Task DeleteCollectionAsync<T>() where T : IDocument
        {
            await _database.DropCollectionAsync(GetCollectionNameByType<T>());
        }

        public async Task<bool> CheckCollectionExistsAsync<T>() where T : IDocument
        {
            var collections = await _database.ListCollectionNamesAsync();
            var collectionNames = await collections.ToListAsync();
            return collectionNames != null && collectionNames.Any() && collectionNames.Contains(GetCollectionName<T>());
        }

        public IMongoCollection<T> GetCollection<T>() where T : IDocument
        {
            Type collectionType = typeof(T);
            if (_collectionCache.ContainsKey(collectionType)) return _collectionCache[collectionType] as IMongoCollection<T> ?? throw new Exception("Collection is not found");
            var collection = _database.GetCollection<T>(GetCollectionName<T>());
            _collectionCache.Add(new KeyValuePair<Type, object>(collectionType, collection));
            return collection;
        }

        public string GetCollectionName<T>() where T : IDocument
        {
            return GetCollectionNameByType<T>();
        }

        /// <summary>
        /// Call this Method before instancing the DbContext
        /// </summary>
        /// <param name="serializers"></param>
        public static void RegisterSerializers(ICollection<IBsonSerialization> serializers)
        {
            foreach (var serializer in serializers)
            {
                BsonSerializer.RegisterSerializer(serializer.Type, serializer.Serializer);
            }
        }
    
        public async Task<IClientSessionHandle> StartTransactionSessionAsync()
        {
            return await StartSessionAsync();
        }

        public IClientSessionHandle StartTransectionSessionSync()
        {
            return StartSession();
        }

        public async Task CreateCollectionsAsync()
        {
            var existingCollections = (await _database.ListCollectionNamesAsync()).ToList();
            foreach (var collectionName in GetCollectionNames().Distinct())
            {
                if (!existingCollections.Contains(collectionName))
                {
                    await _database.CreateCollectionAsync(collectionName);
                }
            }
        }

        public async Task CreateIndicesAsync(IIndexFactory indexFactory)
        {
            await indexFactory.CreateIndicesAsync(this);
        }

        private string GetCollectionNameByType<T>()
        {
           IDocumentCollection? collection = _collections.FirstOrDefault(c => c.DocumentType == typeof(T));
           var baseClass = typeof(T).BaseType;
           while (baseClass != null && collection == null)
           {
               collection = _collections.FirstOrDefault(c => c.DocumentType == baseClass);
               baseClass = baseClass.BaseType;
           }
           if (collection == null) throw new Exception($"Collection of Type {typeof(T).FullName} not found");
           return collection.CollectionName;
        }
    }
}
