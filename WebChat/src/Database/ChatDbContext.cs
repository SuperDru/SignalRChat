using Microsoft.EntityFrameworkCore;
using WebChat.Database.Model;
using WebChat.Helpers;

namespace WebChat.Database
{
    public class ChatDbContext: DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<RoomCredential> RoomCredentials { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.ApplySnakeCase(builder);

            builder.Entity<Message>().HasKey(m => new {m.SendingTime, m.UserId});
            
            builder.Entity<User>().HasIndex(u => u.Nickname).IsUnique();
            builder.Entity<Room>().HasIndex(r => r.Name).IsUnique();
        }
    }
}