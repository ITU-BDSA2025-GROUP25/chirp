using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Razor;
using Chirp.Tests;
using Xunit;
using Assert = Xunit.Assert;

namespace Chirp.Tests;

// Collection Definition - ADD THIS AS A SEPARATE CLASS
[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection("Database collection")]
public class AuthorRepositoryTests
{
    private readonly DatabaseFixture fix;
    private readonly AuthorRepository repo;

    public AuthorRepositoryTests(DatabaseFixture fixture)
    {
        fix = fixture;
        repo = new AuthorRepository(fix.Context);
    }
    [Fact]
    public async Task CreateAndFindAuthor_WorksCorrectly()
    {
        
        // create a new author
        var newAuthor = new Author { Name = "Heðin", Email = "hedin@example.com" };
        await repo.CreateAuthor(newAuthor);

        // find by name
        var foundByName = await repo.FindByName("Heðin");
        Assert.NotNull(foundByName);
        Assert.Equal("hedin@example.com", foundByName.Email);

        // find by email
        var foundByEmail = await repo.FindByEmail("hedin@example.com");
        Assert.NotNull(foundByEmail);
        Assert.Equal("Heðin", foundByEmail.Name);
    }

    [Fact]
    public async Task FindByName()
    {
        var result = await repo.FindByName("Nonexistent");

        Assert.Null(result);
    }
}