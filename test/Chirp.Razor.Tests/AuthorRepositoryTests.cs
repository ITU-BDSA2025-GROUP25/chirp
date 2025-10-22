using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Razor;
using Xunit;

public class AuthorRepositoryTests
{
    [Fact]
    public async Task CreateAndFindAuthor_WorksCorrectly()
    {
        // Arrange - making an temporary in-memory SQLite database
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var builder = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite(connection);

        using var context = new ChirpDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // applies database schema

        var repo = new AuthorRepository(context);

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
    public async Task FindByName_ReturnsNull_WhenAuthorDoesNotExist()
    {
        // Arrange
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var builder = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite(connection);

        using var context = new ChirpDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();

        var repo = new AuthorRepository(context);

        // Act
        var result = await repo.FindByName("Nonexistent");

        // Assert
        Assert.Null(result);
    }
}