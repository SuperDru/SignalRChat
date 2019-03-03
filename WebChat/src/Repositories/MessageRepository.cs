using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebChat.Database;
using WebChat.Database.Model;

namespace WebChat.Repositories
{
    public interface IMessageRepository
    {
        Task<ICollection<Message>> GetMessagesAfter(Room room, DateTime date);
        Task<ICollection<Message>> GetMessagesBefore(Room room, DateTime date);
        Task<ICollection<Message>> GetMessagesBetween(Room room, DateTime date1, DateTime date2);
        Task<ICollection<Message>> GetMessagesAtDay(Room room, DateTime date);
        Task<ICollection<Message>> GetTodayMessages(Room room);
        Task<ICollection<Message>> GetAllMessages(Room room);
        Task AddMessage(Message message);
    }
    
    public class MessageRepository: IMessageRepository
    {
        private readonly ChatDbContext _db;
        
        public MessageRepository(ChatDbContext context)
        {
            _db = context;
        }
        
        public async Task<ICollection<Message>> GetMessagesAfter(Room room, DateTime date)
        {
            return await _db.Messages
                .Include(m => m.User)
                .Where(m => m.Room.Id == room.Id && m.SendingTime > date).ToListAsync();
        }

        public async Task<ICollection<Message>> GetMessagesBefore(Room room, DateTime date)
        {
            return await _db.Messages
                .Include(m => m.User)
                .Where(m => m.Room.Id == room.Id && m.SendingTime < date).ToListAsync();
        }
        
        public async Task<ICollection<Message>> GetMessagesBetween(Room room, DateTime date1, DateTime date2)
        {
            var after = date1 < date2 ? date1 : date2;
            var before = date1 > date2 ? date1 : date2;
            
            return await _db.Messages
                .Include(m => m.User)
                .Where(m => m.Room.Id == room.Id && m.SendingTime > after && m.SendingTime < before).ToListAsync();
        }

        public async Task<ICollection<Message>> GetMessagesAtDay(Room room, DateTime date)
        {
            return await _db.Messages
                .Include(m => m.User)
                .Where(m => m.Room.Id == room.Id && m.SendingTime.Day == date.Day).ToListAsync();
        }

        public async Task<ICollection<Message>> GetTodayMessages(Room room)
        {
            return await _db.Messages
                .Include(m => m.User)
                .Where(m => m.Room.Id == room.Id && m.SendingTime.Day == DateTime.Now.Day).ToListAsync();
        }

        public async Task<ICollection<Message>> GetAllMessages(Room room)
        {
            return await _db.Messages
                .Include(m => m.User)
                .Where(m => m.Room.Id == room.Id).ToListAsync();
        }

        public async Task AddMessage(Message message)
        {
            await _db.Messages.AddAsync(message);

            await _db.SaveChangesAsync();
        }
    }
}