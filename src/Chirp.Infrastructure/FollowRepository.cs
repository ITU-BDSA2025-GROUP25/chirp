using Chirp.Core;

namespace Chirp.Infrastructure;

using Microsoft.EntityFrameworkCore;

public class FollowRepository : IFollowRepository
{
    // Database context used to access follow relationships
    private readonly ChirpDbContext _dbContext;

    public FollowRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Follow(string follower, string followee)
    {
        // Only add a follow if it does not already exist
        if (!_dbContext.Follows.Any(f => f.Follower == follower && f.Followee == followee))
        {
            // Create and save a new follow relationship
            _dbContext.Follows.Add(new Follow
            {
                Follower = follower,
                Followee = followee
            });
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task Unfollow(string follower, string followee)
    {
        // Find the follow relationship
        var rel = await _dbContext.Follows
            .FirstOrDefaultAsync(f => f.Follower == follower && f.Followee == followee);

        // Remove it if it exists
        if (rel != null)
        {
            _dbContext.Follows.Remove(rel);
            await _dbContext.SaveChangesAsync();
        }
    }

    public Task<bool> IsFollowing(string follower, string followee)
    {
        // Check if the follower is already following the followee
        return _dbContext.Follows.AnyAsync(f =>
            f.Follower == follower && f.Followee == followee);
    }

    public Task<List<string>> GetFollowing(string username)
    {
        // Get all users that the user follows
        return _dbContext.Follows
            .Where(f => f.Follower == username)
            .Select(f => f.Followee)
            .ToListAsync();
    }
}