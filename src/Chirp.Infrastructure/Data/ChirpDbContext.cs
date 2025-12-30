using Chirp.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;
public class ChirpDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    public DbSet<Follow> Follows { get; set; }

    public DbSet<Like> Likes { get; set; }


    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
    }
}