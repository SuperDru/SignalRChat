using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat.Database.Model
{
    public class RoomCredential
    {
        [Key]
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        
        public byte[] Salt { get; set; }
        public string HashedPassword { get; set; }
    }
}