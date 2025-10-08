using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO newCheep);
    Task<List<CheepDTO>> ReadCheep(String authorName);
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
        Cheep cheep = new() {Text = newCheep.text, Author = newCheep.author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CheepDTO>> ReadCheep(String authorName)
    {
        var query = from cheep in _dbContext.cheeps
            where cheep.Author.Name == authorName
            select new CheepDTO(){text = cheep.Text, author = cheep.Author,};

        var result = await query.ToListAsync();
        return result;
        
    }

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        Cheep cheep = new() {Text = alteredCheep.text, Author = alteredCheep.author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync(); 
    }
}