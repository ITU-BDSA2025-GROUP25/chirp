using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class DatabaseFixture : IDisposable
{
    public ChirpDbContext Context { get; }
    public ICheepRepository Repository { get; }
    private readonly SqliteConnection _connection;

    public DatabaseFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite(_connection)
            .Options;
            
        Context = new ChirpDbContext(options);
        Context.Database.EnsureCreated();
        Repository = new CheepRepository(Context);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add your test data here
        var authors = new[]
        {
            new Author { name = "Helge", email = "helge@example.com" },
            new Author { name = "Adrian", email = "adrian@example.com" },
            new Author { name = "TestUser", email = "test@example.com" }
        };
        Context.authors.AddRange(authors);

        var cheeps = new[]
        {
            new Cheep { author = authors[0], text = "Hello from Helge!", timeStamp = DateTime.UtcNow.AddMinutes(-10) },
            new Cheep { author = authors[1], text = "Hello from Adrian!", timeStamp = DateTime.UtcNow.AddMinutes(-5) },
            new Cheep { author = authors[2], text = "Test message", timeStamp = DateTime.UtcNow.AddMinutes(3) }
        };
        Context.cheeps.AddRange(cheeps);
        
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}