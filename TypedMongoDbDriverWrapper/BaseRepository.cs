using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TypedMongoDbDriverWrapper.Exceptions;

namespace TypedMongoDbDriverWrapper
{
    public abstract class BaseRepository<T> : IRepository<T> where T : IDocument
    {
        protected readonly IDbContext DbContext;

        protected BaseRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<long> CountAllAsync()
        {
            return await DbContext.GetCollection<T>().CountDocumentsAsync(x => true);
        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            return await (await DbContext.GetCollection<T>().FindAsync(x => true)).ToListAsync();
        }

        public virtual async Task<IList<T>> GetAllAsync(IList<string> ids)
        {
            List<ObjectId> objectIds = ids.Select(ObjectId.Parse).ToList();
            return await (await DbContext.GetCollection<T>().FindAsync(x => objectIds.Contains(x.Id))).ToListAsync();
        }

        public virtual async Task<T?> GetOneAsync(string id)
        {
            return (await DbContext.GetCollection<T>()
                    .FindAsync(Builders<T>.Filter.Eq(x => x.Id, ObjectId.Parse(id))))
                .FirstOrDefault();
        }
        public virtual async Task<T> GetSingleOrThrowAsync(string id)
        {
            var document = await DbContext.GetCollection<T>()
                .AsQueryable()
                .Where(x => x.Id == ObjectId.Parse(id))
                .FirstOrDefaultAsync();

            if (document == null) throw new ResourceNotFoundException(typeof(T).Name, id);
            return document;
        }

        public virtual async Task InsertOneAsync(T item)
        {
            await DbContext.GetCollection<T>().InsertOneAsync(item);
        }

        public virtual async Task InsertManyAsync(IList<T> items)
        {
            await DbContext.GetCollection<T>().InsertManyAsync(items);
        }

        public virtual async Task ReplaceOneAsync(T item)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, item.Id);
            var collection = DbContext.GetCollection<T>();
            var result = await collection.ReplaceOneAsync(filter, item);
            if (result.ModifiedCount != 1)
            {
                throw new DbNotModifiedException(result.ModifiedCount, 1, nameof(ReplaceOneAsync));
            }
        }

        public virtual async Task DeleteManyAsync(IList<T> items)
        {
            var ids = items.Select(x => x.Id);
            var filter = Builders<T>.Filter.In(x => x.Id, ids);
            var result = await DbContext.GetCollection<T>().DeleteManyAsync(filter);
            if (result.DeletedCount != items.Count)
            {
                throw new DbNotModifiedException(result.DeletedCount, items.Count, nameof(DeleteManyAsync));
            }
        }

        public virtual async Task DeleteManyAsync(IList<string> ids)
        {
            await DeleteManyAsync(ids.Select(ObjectId.Parse).ToList());
        }

        public virtual async Task DeleteManyAsync(IList<ObjectId> ids)
        {
            var filter = Builders<T>.Filter.In(x => x.Id, ids);
            var result = await DbContext.GetCollection<T>().DeleteManyAsync(filter);
            if (result.DeletedCount != ids.Count)
            {
                throw new DbNotModifiedException(result.DeletedCount, ids.Count, nameof(DeleteManyAsync));
            }
        }

        public virtual async Task DeleteAsync(T item)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, item.Id);
            var result = await DbContext.GetCollection<T>().DeleteOneAsync(filter);
            if (result.DeletedCount != 1)
            {
                throw new DbNotModifiedException(result.DeletedCount, 1, nameof(DeleteAsync));
            }
        }

        public virtual async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var result = await DbContext.GetCollection<T>().DeleteOneAsync(filter);
            if (result.DeletedCount != 1)
            {
                throw new DbNotModifiedException(result.DeletedCount, 1, nameof(DeleteAsync));
            }
        }

        public bool IsValidNotEmptyId(string id)
        {
            return ObjectId.TryParse(id, out ObjectId objectId) && !objectId.Equals(ObjectId.Empty);
        }

        public bool IsNotEmptyId(ObjectId id)
        {
            return !id.Equals(ObjectId.Empty);
        }

        public bool IsEmptyId(ObjectId id)
        {
            return id.Equals(ObjectId.Empty);
        }

        public ObjectId GetEmptyId()
        {
            return ObjectId.Empty;
        }

        public ObjectId GenerateId()
        {
            return ObjectId.GenerateNewId();
        }
    }
}
