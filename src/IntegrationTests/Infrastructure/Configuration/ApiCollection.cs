using IntegrationTests.Infrastructure.Fixtures;
using Xunit;

namespace IntegrationTests.Infrastructure.Configuration
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    [CollectionDefinition(Constants.Api)]
    public class ApiCollection : ICollectionFixture<TestHostFixture>
    {

    }
}