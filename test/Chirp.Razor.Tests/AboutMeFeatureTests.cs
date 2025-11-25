using System.Threading.Tasks;
using Chirp.Razor;
using Chirp.Razor.Tests;
using Xunit;

namespace Chirp.Razor.Tests;

public class AboutMeFeatureTests
{
    private readonly DatabaseFixture _fixture;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepService _cheepService;

    public AboutMeFeatureTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _authorRepository = new AuthorRepository(_fixture.Context);
        _cheepService = new CheepService(_fixture.Repository);
    }

    [Fact]
    public async Task GetAuthorByName_ReturnsCorrectAuthor()
    {
        // Arrange
        var authorName = "Helge";

        // Act
        var author = await _authorRepository.GetAuthorByName(authorName);

        // Assert
        Assert.NotNull(author);
        Assert.Equal(authorName, author.Name);
        Assert.Equal("helge@example.com", author.Email);
    }

    [Fact]
    public async Task GetAuthorByName_ReturnsNullForNonexistentAuthor()
    {
        // Arrange
        var nonexistentName = "NonExistentUser";

        // Act
        var author = await _authorRepository.GetAuthorByName(nonexistentName);

        // Assert
        Assert.Null(author);
    }

    [Fact]
    public async Task GetAllCheepsFromAuthor_ReturnsAllCheepsForAuthor()
    {
        // Arrange
        var authorName = "Helge";

        // Act
        var cheeps = await _cheepService.GetAllCheepsFromAuthor(authorName);

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
        Assert.All(cheeps, cheep => Assert.Equal(authorName, cheep.Author.Name));
    }

    [Fact]
    public async Task GetAllCheepsFromAuthor_ReturnsEmptyListForAuthorWithNoCheeps()
    {
        // Arrange
        // First create an author with no cheeps
        var newAuthor = new Author 
        { 
            Name = "NewUserWithNoCheeps", 
            Email = "new@example.com" 
        };
        await _authorRepository.CreateAuthor(newAuthor);

        // Act
        var cheeps = await _cheepService.GetAllCheepsFromAuthor("NewUserWithNoCheeps");

        // Assert
        Assert.NotNull(cheeps);
        Assert.Empty(cheeps);
    }

    [Fact]
    public async Task DeleteAuthor_RemovesAuthorAndTheirCheeps()
    {
        // Arrange
        // Create a new author with cheeps
        var testAuthor = new Author 
        { 
            Name = "UserToDelete", 
            Email = "delete@example.com" 
        };
        var createdAuthor = await _authorRepository.CreateAuthor(testAuthor);
        
        // Create a cheep for this author
        var testCheep = new Cheep
        {
            Text = "This will be deleted",
            Author = createdAuthor,
            TimeStamp = DateTime.UtcNow
        };
        _fixture.Context.Cheeps.Add(testCheep);
        await _fixture.Context.SaveChangesAsync();

        // Verify author and cheep exist
        var authorBeforeDelete = await _authorRepository.GetAuthorByName("UserToDelete");
        Assert.NotNull(authorBeforeDelete);
        Assert.NotEmpty(authorBeforeDelete.Cheeps);

        // Act
        await _authorRepository.DeleteAuthor(createdAuthor.AuthorId);

        // Assert
        var authorAfterDelete = await _authorRepository.GetAuthorByName("UserToDelete");
        Assert.Null(authorAfterDelete);

        // Verify cheeps were also deleted
        var cheepsAfterDelete = await _cheepService.GetAllCheepsFromAuthor("UserToDelete");
        Assert.Empty(cheepsAfterDelete);
    }

    [Fact]
    public async Task DeleteAuthor_DoesNotThrowForNonexistentAuthor()
    {
        // Arrange
        var nonexistentAuthorId = 99999;

        // Act & Assert
        // Should not throw exception
        await _authorRepository.DeleteAuthor(nonexistentAuthorId);
    }

    [Fact]
    public async Task ReadAllCheepsByAuthor_ReturnsCheepsInDescendingOrder()
    {
        // Arrange
        var author = new Author 
        { 
            Name = "OrderTestUser", 
            Email = "order@example.com" 
        };
        var createdAuthor = await _authorRepository.CreateAuthor(author);

        // Create multiple cheeps at different times
        var cheep1 = new Cheep
        {
            Text = "First cheep",
            Author = createdAuthor,
            TimeStamp = DateTime.UtcNow.AddMinutes(-10)
        };
        var cheep2 = new Cheep
        {
            Text = "Second cheep",
            Author = createdAuthor,
            TimeStamp = DateTime.UtcNow.AddMinutes(-5)
        };
        var cheep3 = new Cheep
        {
            Text = "Third cheep",
            Author = createdAuthor,
            TimeStamp = DateTime.UtcNow
        };

        _fixture.Context.Cheeps.AddRange(cheep1, cheep2, cheep3);
        await _fixture.Context.SaveChangesAsync();

        // Act
        var cheeps = await _fixture.Repository.ReadAllCheepsByAuthor("OrderTestUser");

        // Assert
        Assert.Equal(3, cheeps.Count);
        // Verify descending order (newest first)
        Assert.Equal("Third cheep", cheeps[0].Message);
        Assert.Equal("Second cheep", cheeps[1].Message);
        Assert.Equal("First cheep", cheeps[2].Message);
    }
}
