using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    // Create a new cheep
    Task CreateCheep(CheepDTO newCheep);

    // Read paginated public cheeps
    Task<List<CheepDTO>> ReadCheep(int page, int limit);

    // Read paginated cheeps by a specific author
    Task<List<CheepDTO>> ReadCheepByAuthor(string authorName, int page, int limit);

    // Update an existing cheep
    Task UpdateCheep(CheepDTO alteredCheep);

    // Get timeline cheeps from a list of authors
    Task<List<CheepDTO>> GetTimelineByAuthors(List<string> authors, int page);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _dbContext;

    // Repository for author lookup and creation
    private readonly IAuthorRepository _authorRepository;

    public CheepRepository(ChirpDbContext dbContext, IAuthorRepository authorRepository)
    {
        _dbContext = dbContext;
        _authorRepository = authorRepository;
    }

    public async Task CreateCheep(CheepDTO newCheep)
    {
        // Validate cheep message
        if (string.IsNullOrWhiteSpace(newCheep.Message))
            throw new ArgumentException("Cheep message cannot be empty");

        if (newCheep.Message.Length > 160)
            throw new ArgumentException("Cheep cannot exceed 160 characters");

        // Find or create the author
        var author = await _authorRepository.FindByName(newCheep.AuthorKey);
        if (author == null)
        {
            author = await _authorRepository.CreateAuthor(new Author
            {
                Name = newCheep.AuthorKey,
                Email = newCheep.AuthorKey
            });
        }

        // Create cheep entity
        var cheep = new Cheep
        {
            Text = newCheep.Message,
            Author = author,
            TimeStamp = DateTime.Now
        };

        // Save cheep to database
        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CheepDTO>> ReadCheep(int page, int limit)
    {
        // Calculate pagination offset
        int offset = (page - 1) * limit;

        // Fetch paginated cheeps
        return await _dbContext.Cheeps
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("g"),
                // Internal author key (email/name)
                AuthorKey = c.Author.Name,
                AuthorDisplayName = c.Author.Name
            })
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> ReadCheepByAuthor(string authorName, int page, int limit)
    {
        // Calculate pagination offset
        int offset = (page - 1) * limit;

        // Fetch cheeps from one author
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
                AuthorKey = c.Author.Name,
                AuthorDisplayName = c.Author.Name
            })
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> GetTimelineByAuthors(List<string> authors, int page)
    {
        const int pageSize = 32;

        // Fetch cheeps from followed authors
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
                AuthorKey = c.Author.Name,
                AuthorDisplayName = c.Author.Name
            })
            .ToListAsync();
    }

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        // Find cheep by id
        var cheep = await _dbContext.Cheeps.FindAsync(alteredCheep.CheepId);

        // Throw if cheep does not exist
        if (cheep == null)
            throw new Exception("Cheep not found");

        // Update cheep text
        cheep.Text = alteredCheep.Message;
        await _dbContext.SaveChangesAsync();
    }
}
