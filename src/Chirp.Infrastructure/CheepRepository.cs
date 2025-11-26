using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO newCheep);
    Task<List<CheepDTO>> ReadCheep(int  page, int limit);
    Task<List<CheepDTO>> ReadCheepByAuthor(String authorName, int page, int limit);
    Task UpdateCheep(CheepDTO alteredCheep);
    Task FollowUser(string follower, string followee);
    Task UnfollowUser(string follower, string followee);
    Task<bool> IsFollowing(string follower, string followee);
    Task<List<CheepDTO>> GetPrivateTimeline(string username, int page);
    Task<List<string>> GetFollowing(string username);


}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _dbContext;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepository(ChirpDbContext dbContext, IAuthorRepository authorRepository)
    {
        _dbContext = dbContext;
        _authorRepository = authorRepository;
    }

    public async Task CreateCheep(CheepDTO newCheep)
    {
        // Server-side validation (defense in depth)
        if (string.IsNullOrWhiteSpace(newCheep.Message))
        {
            throw new ArgumentException("Cheep message cannot be empty");
        }
    
        if (newCheep.Message.Length > 160)
        {
            throw new ArgumentException("Cheep cannot exceed 160 characters");
        }
        // Find or create author
        var author = await _authorRepository.FindByName(newCheep.Author.Name);
        if (author == null)
        {
            author = await _authorRepository.CreateAuthor(new Author
            {
                Name = newCheep.Author.Name,
                Email = newCheep.Author.Email
            });
        }

        var cheep = new Cheep
        {
            Text = newCheep.Message,
            Author = author,
            TimeStamp = DateTime.Now
        };

        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task FollowUser(string follower, string followee)
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

    public async Task UnfollowUser(string follower, string followee)
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
        return _dbContext.Follows
            .AnyAsync(f => f.Follower == follower && f.Followee == followee);
    }

    public async Task<List<CheepDTO>> GetPrivateTimeline(string username, int page)
    {
        const int pageSize = 32;

        var followees = _dbContext.Follows
            .Where(f => f.Follower == username)
            .Select(f => f.Followee);

        return await _dbContext.Cheeps
            .Where(c => followees.Contains(c.Author.Name))
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO
            {
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("g"),
                Author = new Author
                {
                    Name = c.Author.Name,
                    Email = c.Author.Email
                }
            })
            .ToListAsync();
    }

    public Task<List<string>> GetFollowing(string username)
    {
        return _dbContext.Follows
            .Where(f => f.Follower == username)
            .Select(f => f.Followee)
            .ToListAsync();
    }


    
   //Reads all cheeps based on current page and limit of 32
	public async Task<List<CheepDTO>> ReadCheep(int page, int limit)
	{
    int offset = (page - 1) * limit;

    var query = from cheep in _dbContext.Cheeps
        orderby cheep.TimeStamp descending
        select new CheepDTO()
        {
            Message = cheep.Text,
            Author = cheep.Author,
            Timestamp = cheep.TimeStamp.ToString("g")
        };

    var result = await query
        .Skip(offset)
        .Take(limit)
        .ToListAsync();

    return result;
	}

	//sorts the messages by author and page number for example http://localhost:5273/Jacqualine Gilcoine?page=2 
    public async Task<List<CheepDTO>> ReadCheepByAuthor(string authorName, int page, int limit)
{
    int offset = (page - 1) * limit;

    var query = from cheep in _dbContext.Cheeps
        where cheep.Author.Name == authorName
        orderby cheep.TimeStamp descending
        select new CheepDTO()
        {
            Message = cheep.Text,
            Author = cheep.Author,
            Timestamp = cheep.TimeStamp.ToString("g")
        };

    var result = await query
        .Skip(offset)
        .Take(limit)
        .ToListAsync();

    return result;
}

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        Cheep cheep = new() {Text = alteredCheep.Message, Author = alteredCheep.Author};
        var result = await _dbContext.Cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync(); 
    }
}