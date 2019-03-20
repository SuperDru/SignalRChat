using System;
using System.Collections.Generic;
using System.Linq;
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
        Task LeaveRoom(string userName);
        Task CreateRoom(string userName, Credential credentials);
        Task<List<RoomInfoResponse>> GetPublicRooms();
        Task<List<string>> GetUsersOfRoom(string roomName);
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

        private async Task<Room> GetRoom(string name)
        {
            return await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Name == name);
        }
        
        public async Task JoinRoom(string userName, Credential credentials)
        {
            var user = await _rep.GetUser(userName);
            var room = await GetRoom(credentials.Name);

            Check.Value(room, "room credentials").NotNull(ErrorMessages.CredentialsMsg);

            if (!room.IsPublic)
            {
                var check = CheckPassword(room, credentials.Password);
                Check.Value(check, "room credentials").IsTrue(ErrorMessages.CredentialsMsg);
            }

            user.CurrentRoom = room;

            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task LeaveRoom(string userName)
        {
            var user = await _rep.GetUser(userName);
            
            user.CurrentRoom = null;
            user.CurrentRoomId = null;
            
            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateRoom(string userName, Credential credentials)
        {
            var isPublic = string.IsNullOrEmpty(credentials.Password);
                
            var user = await _rep.GetUser(userName);
            var room = await GetRoom(credentials.Name);

            Check.Value(room, "room creation").IsNull(ErrorMessages.RoomWithNameExistsMsg(credentials.Name));

            var salt = isPublic ? null : PasswordGenerator.GenerateSalt();
            var password = isPublic ? null : PasswordGenerator.HashPassword(credentials.Password, salt);
            
            room = new Room()
            {
                Name = credentials.Name,
                CreatedAt = DateTime.Now,
                Salt = salt,
                Password = password,
                UserId = user.Id,
                IsPublic = isPublic
            };

            await _dbContext.Rooms.AddAsync(room);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<RoomInfoResponse>> GetPublicRooms()
        {
            var rooms = await _dbContext.Rooms.Include(r => r.User).Where(r => r.IsPublic).ToListAsync();

            var numberUsersInRoom = new Dictionary<string, int>();
            
            await _dbContext.Users
                .Include(u => u.CurrentRoom)
                .Where(u => u.CurrentRoomId != null)
                .ForEachAsync(u =>
                {
                    if (!numberUsersInRoom.ContainsKey(u.CurrentRoom.Name))
                        numberUsersInRoom[u.CurrentRoom.Name] = 1;
                    else
                        numberUsersInRoom[u.CurrentRoom.Name] += 1;
                });
            
            return rooms.Select(r => new RoomInfoResponse()
            {
                Name = r.Name,
                RoomCreator = r.User.Nickname,
                NumberOfUsers = numberUsersInRoom.ContainsKey(r.Name) ? numberUsersInRoom[r.Name] : 0
            }).ToList();
        }

        public async Task<List<string>> GetUsersOfRoom(string roomName)
        {
            return await _dbContext.Users
                .Include(u => u.CurrentRoom)
                .Where(u => u.CurrentRoomId != null && u.CurrentRoom.Name == roomName)
                .Select(u => u.Nickname)
                .ToListAsync();
        }
    }
}