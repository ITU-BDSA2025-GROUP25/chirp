using System;
using Chirp.Razor;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

//USE THIS DATABASE FOR IN-MEMORY TESTS

/* Insert this at the start of the class :
 
    private readonly DatabaseFixture fix;
        private readonly AuthorRepository repo;

        public AuthorRepositoryTests(DatabaseFixture fixture)
        {
            fix = fixture;
            repo = new AuthorRepository(fix.Context);
        }
*/

public class DatabaseFixture : IDisposable
{
    public ChirpDbContext Context { get; }
    public ICheepRepository Repository { get; }
    private readonly SqliteConnection _connection;

    public DatabaseFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlite(_connection).Options;
            
        Context = new ChirpDbContext(options); // Remove 'using' - we need to keep it alive
        Context.Database.EnsureCreated(); // Use synchronous version, remove Async
        
        // You need to provide IAuthorRepository - create a mock or real one
        IAuthorRepository authorRepository = new AuthorRepository(Context); // Assuming you have this
        Repository = new CheepRepository(Context, authorRepository);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add your test data here - using proper C# property names (PascalCase)
        var authors = new[]
        {
            new Author { Name = "Helge", Email = "helge@example.com" }, // PascalCase
            new Author { Name = "Adrian", Email = "adrian@example.com" }, // PascalCase
            new Author { Name = "TestUser", Email = "test@example.com" } // PascalCase
        };
        Context.Authors.AddRange(authors); // PascalCase

        var cheeps = new[]
        {
            new Cheep { 
                Author = authors[0], 
                Text = "Hello from Helge!", 
                TimeStamp = DateTime.UtcNow.AddMinutes(-10) 
            },
            new Cheep { 
                Author = authors[1], 
                Text = "Hello from Adrian!", 
                TimeStamp = DateTime.UtcNow.AddMinutes(-5) 
            },
            new Cheep { 
                Author = authors[2], 
                Text = "Test message", 
                TimeStamp = DateTime.UtcNow 
            }
        };
        Context.Cheeps.AddRange(cheeps); // PascalCase
        
        Context.SaveChanges(); // Save all test data
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}