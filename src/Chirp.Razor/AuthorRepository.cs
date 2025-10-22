using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface IAuthorRepository
{
    Task<Author?> FindByName(string name);
    Task<Author?> FindByEmail(string email);
    Task<Author> CreateAuthor(Author newAuthor);
}

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _dbContext;

    public AuthorRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Author?> FindByName(string name)
    {
        return await _dbContext.Authors
            .Include(a => a.Cheeps)
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<Author?> FindByEmail(string email)
    {
        return await _dbContext.Authors
            .Include(a => a.Cheeps)
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Author> CreateAuthor(Author newAuthor)
    {
        // check if author exists to avoid duplicate 
        var existing = await _dbContext.Authors
            .FirstOrDefaultAsync(a => a.Email == newAuthor.Email);

        if (existing != null)
            return existing;

        _dbContext.Authors.Add(newAuthor);
        await _dbContext.SaveChangesAsync();

        return newAuthor;
    }
}