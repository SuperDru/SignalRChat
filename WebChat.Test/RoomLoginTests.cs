using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
        [InlineData("Managers", "two", 2)]
        public async void UserCanJoinRoom(string name, string password, int roomId)
        {
            await _context.AuthorizeAsJFoster();
            
            var request = new Credential()
            {
                Name = name,
                Password = password
            };
            
            var response = await _context.Client.PostAsJsonAsync("chat/join", request);
            
            response.StatusCode.Should().BeEquivalentTo(200);

            var user = await _context.Db.Users.FirstOrDefaultAsync(u => u.Id == 1);
            user.CurrentRoomId.Should().Be(roomId);

            user.CurrentRoomId = null;

            _context.Db.Users.Update(user);
            await _context.Db.SaveChangesAsync();
        }
        
        [Fact]
        public async void UserCanLeaveRoom()
        {
            await _context.AuthorizeAsAShishkin();
            await _context.JoinDevelopersRoom();
            
            var response = await _context.Client.PostAsJsonAsync("chat/leave", "");
            
            response.StatusCode.Should().BeEquivalentTo(200);

            var user = await _context.Db.Users.FirstOrDefaultAsync(u => u.Id == 1);
            user.CurrentRoomId.Should().NotHaveValue();
        }
        
        [Theory]
        [InlineData("Developers", "12345")]
        [InlineData("developers", "one")]
        [InlineData("fsdg", "12")]
        public async Task UserWithInvalidCredentialsCannotJoinRoom(string name, string password)
        {
            await _context.AuthorizeAsJFoster();
            
            var request = new Credential()
            {
                Name = name,
                Password = password
            };
            
            var response = await _context.Client.PostAsJsonAsync("chat/join", request);
            var body = await response.Content.ReadAsStringAsync();
            
            body.Should().Contain("\"message\":\"Invalid name or password\"");
            
            response.StatusCode.Should().BeEquivalentTo(400);
        }
    }
}