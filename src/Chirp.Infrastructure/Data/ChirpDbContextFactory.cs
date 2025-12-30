using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Chirp.Infrastructure
{
    public class ChirpDbContextFactory : IDesignTimeDbContextFactory<ChirpDbContext>
    {
        public ChirpDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Chirp.Web"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var options = new DbContextOptionsBuilder<ChirpDbContext>()
                .UseSqlite(config.GetConnectionString("DefaultConnection"))
                .Options;

            return new ChirpDbContext(options);
        }
    }
}