
namespace TypedMongoDbDriverWrapper
{
    public interface IDocumentCollection
    {
        /// <summary>
        /// The Type of the Collection
        /// </summary>
        public Type DocumentType { get; init; }
        
        /// <summary>
        /// The Name of the Collection
        /// </summary>
        public string CollectionName { get; init; } 
    }
}
