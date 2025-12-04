using Microsoft.EntityFrameworkCore;

public class LikeRepository : ILikeRepository
{
    private readonly ChirpDbContext _db;

    public LikeRepository(ChirpDbContext db)
    {
        _db = db;
    }

    public async Task LikeCheep(int cheepId, string username)
    {
        if (!_db.Likes.Any(l => l.CheepId == cheepId && l.Username == username))
        {
            _db.Likes.Add(new Like { CheepId = cheepId, Username = username });
            await _db.SaveChangesAsync();
        }
    }

    public async Task UnlikeCheep(int cheepId, string username)
    {
        var like = await _db.Likes.FirstOrDefaultAsync(l =>
            l.CheepId == cheepId && l.Username == username);

        if (like != null)
        {
            _db.Likes.Remove(like);
            await _db.SaveChangesAsync();
        }
    }

    public Task<bool> HasLiked(int cheepId, string username)
    {
        return _db.Likes.AnyAsync(l =>
            l.CheepId == cheepId && l.Username == username);
    }

    public Task<int> CountLikes(int cheepId)
    {
        return _db.Likes.CountAsync(l => l.CheepId == cheepId);
    }
}