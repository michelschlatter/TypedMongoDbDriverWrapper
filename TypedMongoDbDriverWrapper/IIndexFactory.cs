namespace TypedMongoDbDriverWrapper
{
    public interface IIndexFactory
    {
        public Task CreateIndicesAsync(IDbContext dbContext);
    }
}
