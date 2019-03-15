using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using WebChat.Database.Model;
using WebChat.Repositories;
using WebChat.Services;

namespace WebChat
{
    [Authorize]
    public class ChatHub: Hub
    {
        private static ConcurrentDictionary<string, int> usersConnections = new ConcurrentDictionary<string, int>();
        private static Dictionary<string, string> usersRooms = new Dictionary<string, string>();
        private string LoginName => Context.User.Identity.Name;
        private string Room => usersRooms[LoginName];

        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoomService _roomService;

        public ChatHub(IMessageRepository messageRepository, IUserRepository userRepository, IRoomService roomService)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _roomService = roomService;
        }
        
        public override async Task OnConnectedAsync()
        {   
            var user = await _userRepository.GetUser(LoginName);          
            usersRooms[LoginName] = user.CurrentRoom.Name;
            
            await Groups.AddToGroupAsync(Context.ConnectionId, Room);
            usersConnections.AddOrUpdate(LoginName, k => 1, (k, v) => ++v);

            usersConnections.TryGetValue(LoginName, out var count);

            var history = (await _messageRepository.GetAllMessages(user.CurrentRoom)).OrderBy(m => m.SendingTime);
            
            foreach (var m in history)
            {
                    await Clients.Caller.SendAsync(
                        m.Type.ToString().ToLower(), 
                        m.User.Nickname,
                        m.MessageValue,
                        m.SendingTime.ToString("HH:mm:ss"));
            }
            
            if (count == 1)
            {
                var message = await BuildMessage($"{LoginName} has joined the room.", MessageType.Info);
                
                await _messageRepository.AddMessage(message);
                
                await Clients.Group(Room).SendAsync(
                    "info", 
                    message.User.Nickname,
                    message.MessageValue, 
                    message.SendingTime.ToString("HH:mm:ss"));
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            usersConnections[LoginName] -= 1;
            if (usersConnections[LoginName] == 0)
            {
                var message = await BuildMessage($"{LoginName} has left the room.", MessageType.Info);
                
                await Clients.Group(Room).SendAsync(
                    "info", 
                    message.User.Nickname,
                    message.MessageValue, 
                    message.SendingTime.ToString("HH:mm:ss"));

                await _messageRepository.AddMessage(message);
                
                var user = await _userRepository.GetUser(LoginName);
                await _roomService.LeaveRoom(LoginName);
            }

            await base.OnDisconnectedAsync(exception);
        }
        
        public async Task SendMessage(string message)
        {
            var mes = await BuildMessage(message, MessageType.Broadcast);
            
            await _messageRepository.AddMessage(mes);
            
            await Clients.Group(Room).SendAsync(
                "broadcast", 
                LoginName, 
                mes.MessageValue, 
                mes.SendingTime.ToString("HH:mm:ss"));
        }

        private async Task<Message> BuildMessage(string message, MessageType type)
        {
            var user = await _userRepository.GetUser(LoginName);
            var room = user.CurrentRoom;
            
            return new Message()
            {
                MessageValue = message,
                Room = room,
                SendingTime = DateTime.Now,
                UserId = user.Id,
                User = user,
                RoomId = room.Id,
                Type = type
            };
        }
    }
}