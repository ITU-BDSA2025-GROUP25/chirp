using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface IAuthorRepository
{
    Task<Author?> FindByName(string name);
    Task<Author?> FindByEmail(string email);
    Task<Author> CreateAuthor(Author newAuthor);
    Task<Author?> GetAuthorByName(string name);
    Task DeleteAuthor(int authorId);
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

    public async Task<Author?> GetAuthorByName(string name)
    {
        return await _dbContext.Authors
            .Include(a => a.Cheeps)
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task DeleteAuthor(int authorId)
    {
        var author = await _dbContext.Authors
            .Include(a => a.Cheeps)
            .FirstOrDefaultAsync(a => a.AuthorId == authorId);

        if (author == null)
            return;

        // Remove all cheeps by this author
        _dbContext.Cheeps.RemoveRange(author.Cheeps);

        // Remove the author
        _dbContext.Authors.Remove(author);

        await _dbContext.SaveChangesAsync();
    }
}
