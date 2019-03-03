using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebChat.Database;
using WebChat.Database.Model;
using WebChat.Repositories;

namespace WebChat.Services
{
    public interface IRoomService
    {
        Task<bool> CheckPassword(Room room, string password);
        Task JoinRoom(User user, Room room);
        Task LeaveRoom(User user, Room room);
        Task CreateRoom(User user, Room room);
        Task RemoveRoom(User user, Room room);

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
        public async Task<bool> CheckPassword(Room room, string password)
        {
            var cred = await _dbContext.RoomCredentials.FirstOrDefaultAsync(c => c.RoomId == room.Id);

            var hashedPassword = PasswordGenerator.HashPassword(password, cred.Salt);

            return hashedPassword == cred.HashedPassword;
        }

        public async Task JoinRoom(User user, Room room)
        {
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

        public async Task CreateRoom(User user, Room room)
        {
            throw new System.NotImplementedException();
        }

        public async Task RemoveRoom(User user, Room room)
        {
            throw new System.NotImplementedException();
        }
    }
}