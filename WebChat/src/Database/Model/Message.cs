using System;

namespace WebChat.Database.Model
{
    public enum MessageType
    {
        Info = 1,
        Broadcast = 2,
        Important = 3
    }
    
    public class Message
    {
        public string MessageValue { get; set; }
        public DateTime SendingTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public MessageType Type { get; set; }
    }
}