namespace TypedMongoDbDriverWrapper
{
    public interface ICollectionProvider
    {
        public ICollection<IDocumentCollection> GetAll();
    }
}
