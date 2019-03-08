using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using WebChat.DtoModels;
using Xunit;
using Xunit.Abstractions;

namespace WebChat.Test
{
    [Collection(ApiCollectionFixture.Name)]
    public class RoomLoginTests: IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _context;
        private ITestOutputHelper _output;
        
        public RoomLoginTests(ApiFixture fixture, ITestOutputHelper output)
        {
            _context = fixture;
            _output = output;
        }
        
        [Theory]
        [InlineData("Developers", "one")]
        [InlineData("Managers", "two")]
        public async void UserCanJoinRoom(string name, string password)
        {
            await _context.AuthorizeAsJFoster();
            
            var request = new Credential()
            {
                Name = name,
                Password = password
            };
            
            var response = await _context.Client.PostAsJsonAsync("chat/join", request);
            var body = await response.Content.ReadAsStringAsync();
            
            response.StatusCode.Should().BeEquivalentTo(200);
            body.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData("Developers", "12345")]
        [InlineData("developers", "one")]
        [InlineData("fsdg", "12")]
        public async Task UserWithInvalidCredentialsCannotJoinRoom(string name, string password)
        {
            var request = new Credential()
            {
                Name = name,
                Password = password
            };
            
            var response = await _context.Client.PostAsJsonAsync("chat/join", request);
            var body = await response.Content.ReadAsStringAsync();
            
            response.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.Should().BeNull();
            body.Should().Contain("\"message\":\"Invalid name or password\"");
            
            response.StatusCode.Should().BeEquivalentTo(400);
        }
    }
}