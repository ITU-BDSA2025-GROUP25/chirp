using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class ChirpDbContext : DbContext
{
    public DbSet<Cheep> cheeps { get; set; }
    public DbSet<Author> authors { get; set; }

    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
        
    }
}