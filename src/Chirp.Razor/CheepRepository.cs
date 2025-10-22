using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO newCheep);
    Task<List<CheepDTO>> ReadCheep(int  page, int limit);
    Task<List<CheepDTO>> ReadCheepByAuthor(String authorName, int page, int limit);
    Task UpdateCheep(CheepDTO alteredCheep);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _dbContext;

    public CheepRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreateCheep(CheepDTO newCheep)
    {
        Cheep cheep = new() {Text = newCheep.Message, Author = newCheep.Author};
        var result = await _dbContext.Cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync();
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