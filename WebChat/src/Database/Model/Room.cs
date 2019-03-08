using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat.Database.Model
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        public byte[] Salt { get; set; }
        public string Password { get; set; }
    }
}