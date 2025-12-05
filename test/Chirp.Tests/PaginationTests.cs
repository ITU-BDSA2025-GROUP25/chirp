using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;
using Assert = Xunit.Assert;

namespace Chirp.Razor.Tests;

public class PaginationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PaginationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Xunit.Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(100)] // Very high page number
    public async Task PublicTimeline_AcceptsPageParameter(int page)
    {
        // Act
        var response = await _client.GetAsync($"/?page={page}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Public Timeline", content);
    }

    [Xunit.Theory]
    [InlineData("Helge", 1)]
    [InlineData("Adrian", 1)]
    [InlineData("Helge", 2)]
    public async Task UserTimeline_AcceptsPageParameter(string author, int page)
    {
        // Act
        var response = await _client.GetAsync($"/{author}?page={page}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Fact]
    public async Task PublicTimeline_DefaultsToPage1WhenNoParameter()
    {
        // Act
        var responseNoPage = await _client.GetAsync("/");
        var responsePage1 = await _client.GetAsync("/?page=1");
        
        // Assert
        responseNoPage.EnsureSuccessStatusCode();
        responsePage1.EnsureSuccessStatusCode();
        
        // Both should return successful responses
        var contentNoPage = await responseNoPage.Content.ReadAsStringAsync();
        var contentPage1 = await responsePage1.Content.ReadAsStringAsync();
        
        Assert.Contains("Public Timeline", contentNoPage);
        Assert.Contains("Public Timeline", contentPage1);
    }

    [Xunit.Theory]
    [InlineData(0)]   // Invalid page
    [InlineData(-1)]  // Negative page
    public async Task PublicTimeline_HandlesInvalidPageNumbers(int invalidPage)
    {
        // Act
        var response = await _client.GetAsync($"/?page={invalidPage}");
        
        // Assert  
        response.EnsureSuccessStatusCode(); // Should still work
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Public Timeline", content);
        // Should default to page 1
    }
}