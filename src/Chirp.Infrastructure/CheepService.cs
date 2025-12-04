using Chirp.Razor;

public interface ICheepService
{
    Task<List<CheepDTO>> GetCheeps(int page = 1);
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

    private const int PageSize = 32;

    public CheepService(
        ICheepRepository cheepRepository,
        IFollowRepository followRepository,
        ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _followRepository = followRepository;
        _likeRepository = likeRepository;
    }

    //Public cheeps
    public async Task<List<CheepDTO>> GetCheeps(int page = 1)
    {
        var cheeps = await _cheepRepository.ReadCheep(page, PageSize);

        await AddLikesToCheep(cheeps);

        return cheeps;
    }

    //Cheeps by author
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1)
    {
        var cheeps = await _cheepRepository.ReadCheepByAuthor(author, page, PageSize);

        await AddLikesToCheep(cheeps);

        return cheeps;
    }
    
    //cheeps by privateTimeLine
    public async Task<List<CheepDTO>> GetPrivateTimeline(string username, int page = 1)
    {
        var following = await _followRepository.GetFollowing(username);
        var cheeps = await _cheepRepository.GetTimelineByAuthors(following, page);

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
