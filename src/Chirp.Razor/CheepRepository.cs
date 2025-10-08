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
        Cheep cheep = new() {text = newCheep.text, author = newCheep.author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync();
    }

    
    

    //Needs to be refactored: Read all cheeps
    public async Task<List<CheepDTO>> ReadCheep(int limit, int offset)
    {
        var query = from cheep in _dbContext.cheeps
            select new CheepDTO(){text = cheep.text, author = cheep.author,};

        var result = await query.ToListAsync();
        return result;
        
    }
    public async Task<List<CheepDTO>> ReadCheepByAuthor(String authorName, int page, int limit)
    {
        var query = from cheep in _dbContext.cheeps
            where cheep.author.name == authorName
            select new CheepDTO(){text = cheep.text, author = cheep.author,};

        var result = await query.ToListAsync();
        return result;
        
    }

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        Cheep cheep = new() {text = alteredCheep.text, author = alteredCheep.author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync(); 
    }
}