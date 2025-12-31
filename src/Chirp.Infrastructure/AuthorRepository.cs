using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    // Find an author by name
    Task<Author?> FindByName(string name);

    // Find an author by email
    Task<Author?> FindByEmail(string email);

    // Create a new author if it does not already exist
    Task<Author> CreateAuthor(Author newAuthor);
}

public class AuthorRepository : IAuthorRepository
{
    // Database context for authors
    private readonly ChirpDbContext _dbContext;

    public AuthorRepository(ChirpDbContext dbContext)
    {
        // Store injected database context
        _dbContext = dbContext;
    }

    public async Task<Author?> FindByName(string name)
    {
        // Find author by name and include cheeps
        return await _dbContext.Authors
            .Include(a => a.Cheeps)
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<Author?> FindByEmail(string email)
    {
        // Find author by email and include cheeps
        return await _dbContext.Authors
            .Include(a => a.Cheeps)
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Author> CreateAuthor(Author newAuthor)
    {
        // Check if author exists to avoid duplicates
        var existing = await _dbContext.Authors
            .FirstOrDefaultAsync(a => a.Email == newAuthor.Email);

        if (existing != null)
            return existing;

        // Add and save new author
        _dbContext.Authors.Add(newAuthor);
        await _dbContext.SaveChangesAsync();

        return newAuthor;
    }
}