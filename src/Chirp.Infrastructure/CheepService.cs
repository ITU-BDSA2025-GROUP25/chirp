using Chirp.Razor;

public interface ICheepService
{
    Task<List<CheepDTO>> GetCheeps(int page = 1);
    Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1);
    Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IFollowRepository _followRepository;

    private const int PageSize = 32;

    public CheepService(
        ICheepRepository cheepRepository,
        IFollowRepository followRepository)
    {
        _cheepRepository = cheepRepository;
        _followRepository = followRepository;
    }

    public async Task<List<CheepDTO>> GetCheeps(int page = 1)
    {
        return await _cheepRepository.ReadCheep(page, PageSize);
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1)
    {
        return await _cheepRepository.ReadCheepByAuthor(author, page, PageSize);
    }

    public async Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1)
    {
        var following = await _followRepository.GetFollowing(username);
        return await _cheepRepository.GetTimelineByAuthors(following, page);
    }
}