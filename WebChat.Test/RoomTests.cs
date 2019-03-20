using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Qoden.Util;
using WebChat.Database;
using WebChat.DtoModels;
using Xunit;
using Xunit.Abstractions;

namespace WebChat.Test
{
    [Collection(ApiCollectionFixture.Name)]
    public class RoomTests: IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _context;
        private ITestOutputHelper _output;
        
        public RoomTests(ApiFixture fixture, ITestOutputHelper output)
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
        
        [Fact]
        public async void UserCanCreatePrivateRoom()
        {
            await _context.AuthorizeAsJFoster();
            
            var request = new Credential()
            {
                Name = "Drenor",
                Password = "1"
            };
            
            var response = await _context.Client.PostAsJsonAsync("chat/create", request);
            
            response.StatusCode.Should().BeEquivalentTo(200);

            var room = await _context.Db.Rooms.FirstOrDefaultAsync(r => r.Name == request.Name);
            room.Should().NotBeNull();

            _context.Db.Rooms.Remove(room);
            room.IsPublic.Should().BeFalse();

            await _context.Db.SaveChangesAsync();
        }
        
        [Fact]
        public async void UserCanCreatePublicRoom()
        {
            await _context.AuthorizeAsJFoster();
            
            var request = new Credential()
            {
                Name = "Drenor"
            };
            
            var response = await _context.Client.PostAsJsonAsync("chat/create", request);
            
            response.StatusCode.Should().BeEquivalentTo(200);

            var room = await _context.Db.Rooms.FirstOrDefaultAsync(r => r.Name == request.Name);
            room.Should().NotBeNull();

            _context.Db.Rooms.Remove(room);
            room.IsPublic.Should().BeTrue();

            await _context.Db.SaveChangesAsync();
        }
        
        [Fact]
        public async void UserCanGetListOfPublicRooms()
        {
            await _context.AuthorizeAsJFoster();
            
            var response = await _context.Client.GetAsync("chat/rooms");
            
            response.StatusCode.Should().BeEquivalentTo(200);

            var rooms = await response.Content.ReadAsAsync<List<RoomInfoResponse>>();
            
            var roomsExpected = await _context.Db.Rooms.Where(r => r.IsPublic).ToListAsync();

            rooms.Count.Should().Be(roomsExpected.Count);
            roomsExpected.ForEach(r => rooms.Any(rT => rT.Name == r.Name).Should().BeTrue());
        }
        
        [Fact]
        public async void UserCanGetListOfUsersOfRoom()
        {
            await _context.AuthorizeAsJFoster();
            await _context.JoinDevelopersRoom();
            await _context.AuthorizeAsAShishkin();
            await _context.JoinDevelopersRoom();
            
            var response = await _context.Client.GetAsync("chat/Developers/users");
            
            response.StatusCode.Should().BeEquivalentTo(200);

            var users = await response.Content.ReadAsAsync<List<string>>();
            users.Count.Should().Be(2);
            users.Contains("JFoster").Should().BeTrue();
            users.Contains("AShishkin").Should().BeTrue();

            _context.Db.Users.ForEach(u => u.CurrentRoomId = null);

            await _context.Db.SaveChangesAsync();
        }
    }
}