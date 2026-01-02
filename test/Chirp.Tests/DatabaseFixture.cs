using System;
using Chirp.Core;
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
            
        Context = new ChirpDbContext(options);
        Context.Database.EnsureCreated(); 
        
        IAuthorRepository authorRepository = new AuthorRepository(Context);
        Repository = new CheepRepository(Context, authorRepository);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        
        //insert data
        var authors = new[]
        {
            new Author { Name = "Helge", Email = "helge@example.com" },
            new Author { Name = "Adrian", Email = "adrian@example.com" },
            new Author { Name = "TestUser", Email = "test@example.com" }
        };
        Context.Authors.AddRange(authors);

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
        Context.Cheeps.AddRange(cheeps);
        
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}