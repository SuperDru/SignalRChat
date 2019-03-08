using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebChat.Database.Model;
using WebChat.Helpers;

namespace WebChat.Database
{
    public class ChatDbContext: DbContext
    {
        public ChatDbContext(DbContextOptions options)
            : base(options) {}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.ApplySnakeCase(builder);

            builder.Entity<Message>().HasKey(m => new {m.SendingTime, m.UserId});
        }
    }
}