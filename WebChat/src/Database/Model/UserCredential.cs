using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat.Database.Model
{
    public class UserCredential
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        public byte[] Salt { get; set; }
        public string HashedPassword { get; set; }
    }
}