using Chirp.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ChirpDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
        
    }
}