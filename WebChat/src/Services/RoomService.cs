using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;
using WebChat.Database;
using WebChat.Database.Model;
using WebChat.DtoModels;
using WebChat.Repositories;

namespace WebChat.Services
{
    public interface IRoomService
    {
        Task JoinRoom(string userName, Credential credentials);
        Task LeaveRoom(User user, Room room);
        Task CreateRoom(string userName, Credential credentials);

        Task<Room> GetRoom(string name);
    }
    
    public class RoomService: IRoomService
    {
        private readonly ChatDbContext _dbContext;
        private readonly IUserRepository _rep;
    
        public RoomService(ChatDbContext dbContext, IUserRepository rep)
        {
            _dbContext = dbContext;
            _rep = rep;
        }
        private static bool CheckPassword(Room room, string password) =>
            PasswordGenerator.HashPassword(password, room.Salt) == room.Password;

        public async Task JoinRoom(string userName, Credential credentials)
        {
            var user = await _rep.GetUser(userName);
            var room = await GetRoom(credentials.Name);

            Check.Value(room, "room credentials").NotNull(ErrorMessages.CredentialsMsg);

            var check = CheckPassword(room, credentials.Password);
            
            Check.Value(check, "room credentials").IsTrue(ErrorMessages.CredentialsMsg);
            
            user.CurrentRoom = room;

            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task LeaveRoom(User user, Room room)
        {
            user.CurrentRoom = null;
            user.CurrentRoomId = null;
            
            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Room> GetRoom(string name)
        {
            return await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task CreateRoom(string userName, Credential credentials)
        {
            var user = await _rep.GetUser(userName);
            var room = await GetRoom(credentials.Name);

            Check.Value(room, "room creation").IsNull(ErrorMessages.RoomWithNameExistsMsg(credentials.Name));

            var salt = PasswordGenerator.GenerateSalt();
            var password = PasswordGenerator.HashPassword(credentials.Password, salt);
            
            room = new Room()
            {
                Name = credentials.Name,
                CreatedAt = DateTime.Now,
                Salt = salt,
                Password = password,
                UserId = user.Id
            };

            await _dbContext.Rooms.AddAsync(room);

            await _dbContext.SaveChangesAsync();
        }
    }
}