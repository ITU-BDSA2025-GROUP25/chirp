using Chirp.Core;
using Chirp.Infrastructure; 
using Microsoft.EntityFrameworkCore;

// Repository responsible for handling likes in Cheep.db
public class LikeRepository : ILikeRepository
{
    // Database context
    private readonly ChirpDbContext _db;

    // Constructor that receives the database context via dependency injection
    public LikeRepository(ChirpDbContext db)
    {
        _db = db;
    }

    // Adds a like to a cheep if the user hasn't already liked it
    public async Task LikeCheep(int cheepId, string username)
    {
        // Check if a like from this user for this cheep already exists
        if (!_db.Likes.Any(l => l.CheepId == cheepId && l.Username == username))
        {
            // Create and add a new like
            _db.Likes.Add(new Like { CheepId = cheepId, Username = username });

            // Save changes to Cheep.db
            await _db.SaveChangesAsync();
        }
    }

    // Removes a like from a cheep if it exists
    public async Task UnlikeCheep(int cheepId, string username)
    {
        // Find the like matching this cheep and user
        var like = await _db.Likes.FirstOrDefaultAsync(l =>
            l.CheepId == cheepId && l.Username == username);

        // If the like exists, remove it
        if (like != null)
        {
            _db.Likes.Remove(like);
            await _db.SaveChangesAsync();
        }
    }

    // Checks whether a specific user has liked a specific cheep
    public Task<bool> HasLiked(int cheepId, string username)
    {
        return _db.Likes.AnyAsync(l =>
            l.CheepId == cheepId && l.Username == username);
    }

    // Counts how many likes a specific cheep has
    public Task<int> CountLikes(int cheepId)
    {
        return _db.Likes.CountAsync(l => l.CheepId == cheepId);
    }
}