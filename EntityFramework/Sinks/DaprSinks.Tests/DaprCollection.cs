namespace DaprSinks.Tests
{
    [CollectionDefinition(Name)]
    public class DaprCollection : ICollectionFixture<DaprFixture>
    {
        public const string Name = "dapr collection";
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}