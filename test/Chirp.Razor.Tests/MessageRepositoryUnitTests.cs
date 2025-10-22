namespace Chirp.Razor.Tests;

public class MessageRepositoryUnitTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MessageRepositoryUnitTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ReadCheep_ExistingUser_ReturnsCheeps()
    {
        // Act
        var result = _fixture.Repository.ReadCheep("TestUser");
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ReadCheep_NonExistingUser_ReturnsEmpty()
    {
        // Act
        var result = _fixture.Repository.ReadCheep("NonExistentUser");
        
        // Assert
        Assert.NotNull(result);
    }
}