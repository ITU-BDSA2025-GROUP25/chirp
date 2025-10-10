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
        Cheep cheep = new() {Message = newCheep.Message, Author = newCheep.Author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync();
    }

    
    

    //Needs to be refactored: Read all cheeps
    public async Task<List<CheepDTO>> ReadCheep(int limit, int offset)
    {
        var query = from cheep in _dbContext.cheeps
            select new CheepDTO(){Message = cheep.Message, Author = cheep.Author,};

        var result = await query.ToListAsync();
        return result;
        
    }
    public async Task<List<CheepDTO>> ReadCheepByAuthor(String authorName, int page, int limit)
    {
        var query = from cheep in _dbContext.cheeps
            where cheep.Author.name == authorName
            select new CheepDTO(){Message = cheep.Message, Author = cheep.Author,};

        var result = await query.ToListAsync();
        return result;
        
    }

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        Cheep cheep = new() {Message = alteredCheep.Message, Author = alteredCheep.Author};
        var result = await _dbContext.cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync(); 
    }
}