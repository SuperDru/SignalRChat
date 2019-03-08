using Xunit;

namespace WebChat.Test
{
    [CollectionDefinition(Name)]
    public class ApiCollectionFixture : ICollectionFixture<ApiFixture>
    {
        public const string Name = "ApiCollectionFixture";
    }
}