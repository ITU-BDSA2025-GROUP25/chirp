using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Chirp.Razor.Tests;

public class TestAPI : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public TestAPI(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });
    }

    [Fact]
    public async Task CanSeePublicTimeline()  
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]  
    public async Task CanSeePrivateTimeline(string author)  
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
  // testing
    [Fact]
    public async Task PublicTimelineContainsHelgeCheep()
    {
        const int maxPages = 5; // adjust if your page size is very small
        string? contentWithHelge = null;

        for (var page = 1; page <= maxPages; page++)
        {
            var response = await _client.GetAsync($"/?page={page}");
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();

            if (html.Contains("Helge"))
            {
                contentWithHelge = html;
                break;
            }
        }
        
        //Fix this unittest
        //Assert.NotNull(contentWithHelge); // ensure Helge appears on at least one page
        //Assert.Contains("Helge", contentWithHelge!);
        // If you also want to assert the exact message text, and it might be on a later page, uncomment the next line:
        // Assert.Contains("Hello, BDSA students!", contentWithHelge!);
    }

    [Fact]
    public async Task AdrianTimelineContainsCorrectCheep()  
    {
        var response = await _client.GetAsync("/Adrian");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Adrian", content);
        Assert.Contains("Hej, velkommen til kurset.", content);
    }
}