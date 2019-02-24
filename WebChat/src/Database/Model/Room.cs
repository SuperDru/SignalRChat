using System;

namespace WebChat.Database.Model
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}