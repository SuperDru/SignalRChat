using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebChat.Database;
using WebChat.Database.Model;

namespace WebChat.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUser(string nickname);
        Task<Room> GetUserRoom(string nickname);
    }
    
    public class UserRepository: IUserRepository
    {
        private ChatDbContext _db;

        public UserRepository(ChatDbContext context)
        {
            _db = context;
        }
        
        public async Task<User> GetUser(string nickname)
        {
            //var users = _db.Users.ToList();
            return await _db.Users
                .Include(u => u.CurrentRoom)
                .FirstOrDefaultAsync(u => u.Nickname == nickname);
        }

        public async Task<Room> GetUserRoom(string nickname)
        {
           // var users = _db.Users.ToList();
            var user = await _db.Users
                .Include(u => u.CurrentRoom)
                .FirstOrDefaultAsync(u => u.Nickname == nickname); 
            return user.CurrentRoom;
        }
    }
}