using Chirp.Core;

namespace Chirp.Infrastructure;

using Microsoft.EntityFrameworkCore;

public class FollowRepository : IFollowRepository
{
    private readonly ChirpDbContext _dbContext;

    public FollowRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Follow(string follower, string followee)
    {
        if (!_dbContext.Follows.Any(f => f.Follower == follower && f.Followee == followee))
        {
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
        var rel = await _dbContext.Follows
            .FirstOrDefaultAsync(f => f.Follower == follower && f.Followee == followee);

        if (rel != null)
        {
            _dbContext.Follows.Remove(rel);
            await _dbContext.SaveChangesAsync();
        }
    }

    public Task<bool> IsFollowing(string follower, string followee)
    {
        return _dbContext.Follows.AnyAsync(f =>
            f.Follower == follower && f.Followee == followee);
    }

    public Task<List<string>> GetFollowing(string username)
    {
        return _dbContext.Follows
            .Where(f => f.Follower == username)
            .Select(f => f.Followee)
            .ToListAsync();
    }
}
