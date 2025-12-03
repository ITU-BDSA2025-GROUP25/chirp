using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO newCheep);
    Task<List<CheepDTO>> ReadCheep(int page, int limit);
    Task<List<CheepDTO>> ReadCheepByAuthor(string authorName, int page, int limit);
    Task UpdateCheep(CheepDTO alteredCheep);
    Task<List<CheepDTO>> GetTimelineByAuthors(List<string> authors, int page);
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
        if (string.IsNullOrWhiteSpace(newCheep.Message))
            throw new ArgumentException("Cheep message cannot be empty");

        if (newCheep.Message.Length > 160)
            throw new ArgumentException("Cheep cannot exceed 160 characters");

        var author = await _authorRepository.FindByName(newCheep.AuthorName);
        if (author == null)
        {
            author = await _authorRepository.CreateAuthor(new Author
            {
                Name = newCheep.AuthorName,
                Email = "unknown@email.com"
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

    public async Task<List<CheepDTO>> ReadCheep(int page, int limit)
    {
        int offset = (page - 1) * limit;

        return await _dbContext.Cheeps
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("g"),
                AuthorName = c.Author.Name
            })
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> ReadCheepByAuthor(string authorName, int page, int limit)
    {
        int offset = (page - 1) * limit;

        return await _dbContext.Cheeps
            .Where(c => c.Author.Name == authorName)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("g"),
                AuthorName = c.Author.Name
            })
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> GetTimelineByAuthors(List<string> authors, int page)
    {
        const int pageSize = 32;

        return await _dbContext.Cheeps
            .Where(c => authors.Contains(c.Author.Name))
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("g"),
                AuthorName = c.Author.Name
            })
            .ToListAsync();
    }

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        var cheep = await _dbContext.Cheeps.FindAsync(alteredCheep.CheepId);

        if (cheep == null)
            throw new Exception("Cheep not found");

        cheep.Text = alteredCheep.Message;
        await _dbContext.SaveChangesAsync();
    }
}
