using Microsoft.EntityFrameworkCore;

namespace MyChat.Razor.Models
{
    public class ChatDBContext : DbContext
    {
        // Constructor: must call base constructor with options
        public ChatDBContext(DbContextOptions<ChatDBContext> options)
            : base(options)
        {
        }

        // DbSets = tables in the database
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
    }
}