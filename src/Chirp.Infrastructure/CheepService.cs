using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;

public interface ICheepService
{
    // Get public cheeps with optional user context for likes
    Task<List<CheepDTO>> GetCheeps(int page = 1, string? username = null);

    // Get cheeps from a specific author
    Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1);

    // Get timeline cheeps from followed users and self
    Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1);

    // Create a new cheep
    Task PostCheep(CheepDTO cheep);

    // Like a cheep
    Task LikeCheep(int cheepId, string username);

    // Remove a like from a cheep
    Task UnlikeCheep(int cheepId, string username);
}

public class CheepService : ICheepService
{
    // Repository for reading and writing cheeps
    private readonly ICheepRepository _cheepRepository;

    // Repository for follow relationships
    private readonly IFollowRepository _followRepository;

    // Repository for likes
    private readonly ILikeRepository _likeRepository;

    // Database context for user lookups
    private readonly ChirpDbContext _dbContext;

    // Number of cheeps per page
    private const int PageSize = 32;

    public CheepService(
        ICheepRepository cheepRepository,
        IFollowRepository followRepository,
        ILikeRepository likeRepository,
        ChirpDbContext dbContext)
    {
        _cheepRepository = cheepRepository;
        _followRepository = followRepository;
        _likeRepository = likeRepository;
        _dbContext = dbContext;
    }

    public async Task<List<CheepDTO>> GetCheeps(int page = 1, string? username = null)
    {
        // Fetch public cheeps
        var cheeps = await _cheepRepository.ReadCheep(page, PageSize);

        // Add display names and like info
        await ApplyDisplayNames(cheeps);
        await AddLikesToCheep(cheeps, username);

        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1)
    {
        // Fetch cheeps by a specific author
        var cheeps = await _cheepRepository.ReadCheepByAuthor(author, page, PageSize);

        // Add display names and like counts
        await ApplyDisplayNames(cheeps);
        await AddLikesToCheep(cheeps);

        return cheeps;
    }

    public async Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1)
    {
        // Get users that the current user follows
        var following = await _followRepository.GetFollowing(username);

        // Always include the current user
        if (!following.Contains(username))
            following.Add(username);

        // Resolve display name for current user
        var me = await _dbContext.Users
            .Where(u => u.Email == username)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync();

        // Include display name if different from email
        if (!string.IsNullOrWhiteSpace(me) && !following.Contains(me))
            following.Add(me);

        // Remove duplicates
        following = following.Distinct().ToList();

        // Fetch timeline cheeps
        var cheeps = await _cheepRepository.GetTimelineByAuthors(following, page);

        // Add display names and like info
        await ApplyDisplayNames(cheeps);
        await AddLikesToCheep(cheeps, username);

        return cheeps;
    }

    public async Task PostCheep(CheepDTO cheep)
    {
        // Save a new cheep
        await _cheepRepository.CreateCheep(cheep);
    }

    public async Task LikeCheep(int cheepId, string username)
    {
        // Add a like to the cheep
        await _likeRepository.LikeCheep(cheepId, username);
    }

    public async Task UnlikeCheep(int cheepId, string username)
    {
        // Remove a like from the cheep
        await _likeRepository.UnlikeCheep(cheepId, username);
    }

    private async Task ApplyDisplayNames(List<CheepDTO> cheeps)
    {
        // Skip if there are no cheeps
        if (cheeps.Count == 0) return;

        // Collect unique author keys
        var keys = cheeps
            .Select(c => c.AuthorKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        if (keys.Count == 0) return;

        // Map emails to display names
        var map = await _dbContext.Users
            .Where(u => keys.Contains(u.Email))
            .Select(u => new { u.Email, u.DisplayName })
            .ToDictionaryAsync(x => x.Email!, x => x.DisplayName);

        // Apply display names to cheeps
        foreach (var cheep in cheeps)
        {
            if (map.TryGetValue(cheep.AuthorKey, out var display) && !string.IsNullOrWhiteSpace(display))
                cheep.AuthorDisplayName = display;
            else
                cheep.AuthorDisplayName = cheep.AuthorKey; // fallback
        }
    }

    private async Task AddLikesToCheep(List<CheepDTO> cheeps, string? username = null)
    {
        foreach (var cheep in cheeps)
        {
            // Add total like count
            cheep.LikeCount = await _likeRepository.CountLikes(cheep.CheepId);

            // Check if the current user has liked the cheep
            if (!string.IsNullOrEmpty(username))
                cheep.HasLiked = await _likeRepository.HasLiked(cheep.CheepId, username);
        }
    }
}
