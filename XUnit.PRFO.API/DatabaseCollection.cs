using Xunit;

namespace XUnit.PRFO.API
{
    [CollectionDefinition(nameof(DatabaseCollection))]
    public class DatabaseCollection : ICollectionFixture<BaseAppTestFixture>
    {
    }
}
