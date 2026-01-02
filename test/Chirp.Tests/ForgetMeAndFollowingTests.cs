using System;
using System.Linq;
using System.Threading.Tasks;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Razor;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Tests;

public class ForgetMeAndFollowingTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ChirpDbContext _context;

    public ForgetMeAndFollowingTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.Context;
    }

    [Fact]
    public async Task DeleteUserData_IntegrationTest()
    {
        // Arrange
        var testEmail = "integration@example.com";
        
        var author = new Author { Name = "Integration Test", Email = testEmail };
        _context.Authors.Add(author);
        
        var cheep = new Cheep 
        { 
            Text = "Integration test cheep", 
            TimeStamp = DateTime.Now,
            Author = author 
        };
        _context.Cheeps.Add(cheep);
        
        var follow = new Follow 
        { 
            Follower = testEmail, 
            Followee = "helge@example.com" 
        };
        _context.Follows.Add(follow);
        
        var like = new Like 
        { 
            CheepId = cheep.CheepId, 
            Username = testEmail 
        };
        _context.Likes.Add(like);
        
        await _context.SaveChangesAsync();

        // Act
        var userFollows = _context.Follows
            .Where(f => f.Follower == testEmail || f.Followee == testEmail);
        _context.Follows.RemoveRange(userFollows);

        var userLikes = _context.Likes.Where(l => l.Username == testEmail);
        _context.Likes.RemoveRange(userLikes);

        author.Name = "Deleted user";
        author.Email = $"deleted-integration@anon.invalid";

        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(0, await _context.Follows
            .CountAsync(f => f.Follower == testEmail || f.Followee == testEmail));
        
        Assert.Equal(0, await _context.Likes
            .CountAsync(l => l.Username == testEmail));
        
        Assert.Equal("Deleted user", author.Name);
        Assert.Contains("@anon.invalid", author.Email);
        
        Assert.NotNull(await _context.Cheeps.FindAsync(cheep.CheepId));
    }

    [Fact]
    public async Task GetFollowingList_IntegrationTest()
    {
        var testEmail = "followingtest@example.com";
        
        var follows = new[]
        {
            new Follow { Follower = testEmail, Followee = "helge@example.com" },
            new Follow { Follower = testEmail, Followee = "adrian@example.com" },
            new Follow { Follower = "test@example.com", Followee = testEmail }
        };
        _context.Follows.AddRange(follows);
        await _context.SaveChangesAsync();

        var following = await _context.Follows
            .Where(f => f.Follower == testEmail)
            .Join(_context.Authors,
                f => f.Followee,
                a => a.Email,
                (f, a) => new { a.Name, a.Email })
            .OrderBy(x => x.Name)
            .ToListAsync();

        Assert.Equal(2, following.Count);
        Assert.Equal("Adrian", following[0].Name);
        Assert.Equal("Helge", following[1].Name);
    }

    [Fact]
    public async Task ForgetMe_AnonymizesAuthorData_ButKeepsCheeps()
    {
        var testEmail = "anonymize-test@example.com";
        var testName = "Test User";
        
        var author = new Author { Name = testName, Email = testEmail };
        _context.Authors.Add(author);
        
        var cheep = new Cheep 
        { 
            Text = "This cheep should remain", 
            TimeStamp = DateTime.Now,
            Author = author 
        };
        _context.Cheeps.Add(cheep);
        await _context.SaveChangesAsync();

        // Act - Anonymize
        author.Name = "Deleted user";
        author.Email = $"deleted-{Guid.NewGuid():N}@anon.invalid";
        await _context.SaveChangesAsync();

        // Assert
        var updatedCheep = await _context.Cheeps
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.CheepId == cheep.CheepId);
            
        Assert.NotNull(updatedCheep);
        Assert.Equal("Deleted user", updatedCheep.Author.Name);
        Assert.Contains("@anon.invalid", updatedCheep.Author.Email);
    }

    [Fact]
    public async Task ForgetMe_RemovesAllFollowRelationships()
    {
        var testEmail = "follow-removal-test@example.com";
        
        var follows = new[]
        {
            new Follow { Follower = testEmail, Followee = "helge@example.com" },
            new Follow { Follower = "adrian@example.com", Followee = testEmail },
            new Follow { Follower = testEmail, Followee = "test@example.com" }
        };
        _context.Follows.AddRange(follows);
        await _context.SaveChangesAsync();

        // Act
        var userFollows = await _context.Follows
            .Where(f => f.Follower == testEmail || f.Followee == testEmail)
            .ToListAsync();
        _context.Follows.RemoveRange(userFollows);
        await _context.SaveChangesAsync();

        // Assert
        var remainingFollows = await _context.Follows
            .Where(f => f.Follower == testEmail || f.Followee == testEmail)
            .CountAsync();
        Assert.Equal(0, remainingFollows);
    }

    [Fact]
    public async Task ForgetMe_RemovesAllUserLikes()
    {
        var testEmail = "like-removal-test@example.com";
        
        var author = new Author { Name = "Test User", Email = testEmail };
        _context.Authors.Add(author);
        
        var cheep = new Cheep 
        { 
            Text = "Test cheep for likes", 
            TimeStamp = DateTime.Now,
            Author = author 
        };
        _context.Cheeps.Add(cheep);
        await _context.SaveChangesAsync();

        var like = new Like { CheepId = cheep.CheepId, Username = testEmail };
        _context.Likes.Add(like);
        await _context.SaveChangesAsync();

        // Act
        var userLikes = await _context.Likes
            .Where(l => l.Username == testEmail)
            .ToListAsync();
        _context.Likes.RemoveRange(userLikes);
        await _context.SaveChangesAsync();

        // Assert
        var remainingLikes = await _context.Likes
            .Where(l => l.Username == testEmail)
            .CountAsync();
        Assert.Equal(0, remainingLikes);
    }

    [Fact]
    public async Task ForgetMe_OnlyAffectsTargetUser_NotOthers()
    {
        var user1Email = "user1@example.com";
        var user2Email = "user2@example.com";
        
        var author1 = new Author { Name = "User One", Email = user1Email };
        var author2 = new Author { Name = "User Two", Email = user2Email };
        _context.Authors.AddRange(author1, author2);
        
        var follow = new Follow { Follower = user1Email, Followee = user2Email };
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        // Act - Only anonymize user1
        author1.Name = "Deleted user";
        author1.Email = $"deleted-user1@anon.invalid";
        
        var user1Follows = await _context.Follows
            .Where(f => f.Follower == user1Email || f.Followee == user1Email)
            .ToListAsync();
        _context.Follows.RemoveRange(user1Follows);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser1 = await _context.Authors
            .FirstOrDefaultAsync(a => a.AuthorId == author1.AuthorId);
        Assert.Equal("Deleted user", updatedUser1.Name);
        
        var unchangedUser2 = await _context.Authors
            .FirstOrDefaultAsync(a => a.AuthorId == author2.AuthorId);
        Assert.Equal("User Two", unchangedUser2.Name);
        Assert.Equal(user2Email, unchangedUser2.Email);
    }
}