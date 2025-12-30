using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;

public interface ICheepService
{
    Task<List<CheepDTO>> GetCheeps(int page = 1, string? username = null);
    Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1);
    Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1);
    Task PostCheep(CheepDTO cheep);

    Task LikeCheep(int cheepId, string username);
    Task UnlikeCheep(int cheepId, string username);
    
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IFollowRepository _followRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly ChirpDbContext _dbContext;

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
        var cheeps = await _cheepRepository.ReadCheep(page, PageSize);

        await ApplyDisplayNames(cheeps);
        await AddLikesToCheep(cheeps, username);

        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1)
    {
        var cheeps = await _cheepRepository.ReadCheepByAuthor(author, page, PageSize);

        await ApplyDisplayNames(cheeps);
        await AddLikesToCheep(cheeps);

        return cheeps;
    }

    public async Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1)
    {
        var following = await _followRepository.GetFollowing(username);

        // Always include current user email key
        if (!following.Contains(username))
            following.Add(username);
        
        var me = await _dbContext.Users
            .Where(u => u.Email == username)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrWhiteSpace(me) && !following.Contains(me))
            following.Add(me);

        // Avoid duplicates
        following = following.Distinct().ToList();

        var cheeps = await _cheepRepository.GetTimelineByAuthors(following, page);

        await ApplyDisplayNames(cheeps);
        await AddLikesToCheep(cheeps, username);

        return cheeps;
    }

    public async Task PostCheep(CheepDTO cheep)
    {
        await _cheepRepository.CreateCheep(cheep);
    }

    public async Task LikeCheep(int cheepId, string username)
    {
        await _likeRepository.LikeCheep(cheepId, username);
    }

    public async Task UnlikeCheep(int cheepId, string username)
    {
        await _likeRepository.UnlikeCheep(cheepId, username);
    }

    private async Task ApplyDisplayNames(List<CheepDTO> cheeps)
    {
        if (cheeps.Count == 0) return;

        var keys = cheeps
            .Select(c => c.AuthorKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        if (keys.Count == 0) return;

        // keys are emails in your current setup
        var map = await _dbContext.Users
            .Where(u => keys.Contains(u.Email))
            .Select(u => new { u.Email, u.DisplayName })
            .ToDictionaryAsync(x => x.Email!, x => x.DisplayName);

        foreach (var cheep in cheeps)
        {
            if (map.TryGetValue(cheep.AuthorKey, out var display) && !string.IsNullOrWhiteSpace(display))
            {
                cheep.AuthorDisplayName = display;
            }
            else
            {
                cheep.AuthorDisplayName = cheep.AuthorKey; // fallback
            }
        }
    }

    private async Task AddLikesToCheep(List<CheepDTO> cheeps, string? username = null)
    {
        foreach (var cheep in cheeps)
        {
            cheep.LikeCount = await _likeRepository.CountLikes(cheep.CheepId);

            if (!string.IsNullOrEmpty(username))
                cheep.HasLiked = await _likeRepository.HasLiked(cheep.CheepId, username);
        }
    }
}

