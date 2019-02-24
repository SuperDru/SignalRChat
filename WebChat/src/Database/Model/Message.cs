using System;

namespace WebChat.Database.Model
{
    public class Message
    {
        public string MessageValue { get; set; }
        public DateTime SendingTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }
    }
}