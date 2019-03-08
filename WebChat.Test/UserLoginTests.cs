using System;
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
    public class UserLoginTests: IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _context;
        private ITestOutputHelper _output;
        
        public UserLoginTests(ApiFixture fixture, ITestOutputHelper output)
        {
            _context = fixture;
            _output = output;
        }
        
        [Fact]
        public async void UserCanLogin()
        {
            var request = new Credential()
            {
                Name = "AShishkin",
                Password = "12"
            };
            
            var response = await _context.Client.PostAsJsonAsync("login", request);
            
            response.StatusCode.Should().BeEquivalentTo(200);
            response.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.Should().NotBeNull();
        }
        
        [Theory]
        [InlineData("AShishkin", "12345")]
        [InlineData("AShishkind", "54321")]
        [InlineData("AShishkinf", "12")]
        public async Task UserWithInvalidCredentialsCannotLogin(string nickname, string password)
        {
            var request = new Credential()
            {
                Name = nickname,
                Password = password
            };
            
            var response = await _context.Client.PostAsJsonAsync("login", request);
            var body = await response.Content.ReadAsStringAsync();
            
            response.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.Should().BeNull();
            body.Should().Contain("\"message\":\"Invalid name or password\"");
            _output.WriteLine(response.StatusCode.ToString());
            response.StatusCode.Should().BeEquivalentTo(400);
        }
    }
}