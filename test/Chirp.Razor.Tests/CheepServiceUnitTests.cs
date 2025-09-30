using Xunit;
using System.Collections.Generic;

namespace Chirp.Razor.Tests;

public class CheepServiceUnitTests
{
    [Fact]
    public void GetCheeps_ReturnsListOfCheeps()
    {
        // Arrange
        var service = new CheepService();
        
        // Act
        var result = service.GetCheeps(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<CheepViewModel>>(result);
    }
    
    [Fact]
    public void GetCheeps_RespectsPageSize()
    {
        // Arrange
        var service = new CheepService();
        
        // Act
        var result = service.GetCheeps(1);
        
        // Assert
        Assert.True(result.Count <= 32, "Page should contain maximum 32 cheeps");
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public void GetCheeps_AcceptsPageParameter(int page)
    {
        // Arrange
        var service = new CheepService();
        
        // Act
        var result = service.GetCheeps(page);
        
        // Assert
        Assert.NotNull(result);
        // Even if there's no data for that page, should return empty list, not null
    }
    
    [Fact]
    public void GetCheepsFromAuthor_ReturnsOnlyAuthorsCheeps()
    {
        // Arrange
        var service = new CheepService();
        string author = "Helge";
        
        // Act
        var result = service.GetCheepsFromAuthor(author, 1);
        
        // Assert
        if (result.Count > 0)  // Only check if we have results
        {
            Assert.All(result, cheep => Assert.Equal(author, cheep.Author));
        }
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    [InlineData("NonExistentAuthor")]
    public void GetCheepsFromAuthor_HandlesVariousAuthors(string author)
    {
        // Arrange
        var service = new CheepService();
        
        // Act
        var result = service.GetCheepsFromAuthor(author, 1);
        
        // Assert
        Assert.NotNull(result);
        Assert.All(result, cheep => Assert.Equal(author, cheep.Author));
    }
}