using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Razor;
using Chirp.Razor.Tests;
using Xunit;

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
        
        // Act - create a new author
        var newAuthor = new Author { Name = "Heðin", Email = "hedin@example.com" };
        await repo.CreateAuthor(newAuthor);

        // Assert - find by name
        var foundByName = await repo.FindByName("Heðin");
        Assert.NotNull(foundByName);
        Assert.Equal("hedin@example.com", foundByName.Email);

        // Assert - find by email
        var foundByEmail = await repo.FindByEmail("hedin@example.com");
        Assert.NotNull(foundByEmail);
        Assert.Equal("Heðin", foundByEmail.Name);
    }

    [Fact]
    public async Task FindByName()
    {
        // Act
        var result = await repo.FindByName("Nonexistent");

        // Assert
        Assert.Null(result);
    }
}