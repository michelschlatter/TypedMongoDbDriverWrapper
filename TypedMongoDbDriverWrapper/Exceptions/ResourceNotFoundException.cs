namespace TypedMongoDbDriverWrapper.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
            public ResourceNotFoundException(string resourceName, string resourceId)
                : base($"The resource {resourceName} with the id {resourceId} was not found") { }
        
    }
}
