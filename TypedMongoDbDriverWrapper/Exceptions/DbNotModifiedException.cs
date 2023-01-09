namespace TypedMongoDbDriverWrapper.Exceptions
{
    public class DbNotModifiedException : Exception
    {
        public DbNotModifiedException(long count, long expectedCount, string methodName) : 
            base($"Db was not modified. Expected Changes ${expectedCount}, actual {count} in method {methodName}") { }
    }
}
