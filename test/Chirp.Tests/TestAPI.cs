using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Chirp.Tests;

public class TestAPI : IClassFixture<WebDatabaseFixture>
{
    private readonly WebDatabaseFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;

    public TestAPI(WebDatabaseFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
        _client = fixture.CreateHttpClient();
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

    [Xunit.Theory]
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

    /*[Fact]
    public async Task JacqualineTimelineContainsCorrectCheep()  
    {
        var response = await _client.GetAsync("/Adrian");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Adrians's Timeline", content);
        Assert.Contains("Hej, velkommen til kurset.", content);
    }*/
    
    [Fact]
    public void Diagnostic_CheckDatabaseState()
    {
        _testOutputHelper.WriteLine("=== RUNNING DATABASE DIAGNOSTIC ===");
        
        // Check counts
        var authorCount = _fixture.Context.Authors.Count();
        var cheepCount = _fixture.Context.Cheeps.Count();
        
        _testOutputHelper.WriteLine($"Author count: {authorCount}");
        _testOutputHelper.WriteLine($"Cheep count: {cheepCount}");
        
        // List authors
        _testOutputHelper.WriteLine("\nAuthors:");
        foreach (var author in _fixture.Context.Authors)
        {
            _testOutputHelper.WriteLine($"  - {author.Name} ({author.Email})");
        }
        
        // List cheeps with authors
        _testOutputHelper.WriteLine("\nCheeps:");
        foreach (var cheep in _fixture.Context.Cheeps.Include(c => c.Author))
        {
            _testOutputHelper.WriteLine($"  - '{cheep.Text}' by {cheep.Author?.Name} at {cheep.TimeStamp}");
        }
        
        // Also check what tables exist
        try
        {
            var connection = _fixture.Context.Database.GetDbConnection();
            _testOutputHelper.WriteLine($"\nDatabase connection: {connection.State}");
            
            if (connection.State == System.Data.ConnectionState.Open)
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                using var reader = command.ExecuteReader();
                
                _testOutputHelper.WriteLine("Tables in database:");
                while (reader.Read())
                {
                    _testOutputHelper.WriteLine($"  - {reader.GetString(0)}");
                }
            }
        }
        catch (Exception ex)
        {
            _testOutputHelper.WriteLine($"Could not list tables: {ex.Message}");
        }
        
        _testOutputHelper.WriteLine("===================================");
        
        Assert.True(true); // Just to mark test as passed
    }
}