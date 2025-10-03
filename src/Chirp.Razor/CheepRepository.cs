using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task CreateCheep(Cheep newCheep);
    Task<List<Cheep>> ReadCheep(String authorName);
    Task UpdateCheep(Cheep alteredCheep);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _dbContext;

    public CheepRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreateCheep(Cheep newCheep)
    {
        Cheep cheep = new() {text = newCheep.text, author = newCheep.author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Cheep>> ReadCheep(String authorName)
    {
        var query = from cheep in _dbContext.cheeps
            where cheep.author.name == authorName
            select cheep;

        var result = await query.ToListAsync();
        return result;
        
    }

    public async Task UpdateCheep(Cheep alteredCheep)
    {
        Cheep cheep = new() {text = alteredCheep.text, author = alteredCheep.author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync(); 
    }
}