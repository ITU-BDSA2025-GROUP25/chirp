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

    
    

    //Needs to be refactored: Read all cheeps
    public async Task<List<CheepDTO>> ReadCheep(int limit, int offset)
    {
        var query = from cheep in _dbContext.Cheeps
            select new CheepDTO(){Message = cheep.Text, Author = cheep.Author,};

        var result = await query.ToListAsync();
        return result;
        
    }
    public async Task<List<CheepDTO>> ReadCheepByAuthor(String authorName, int page, int limit)
    {
        var query = from cheep in _dbContext.Cheeps
            where cheep.Author.Name == authorName
            select new CheepDTO(){Message = cheep.Text, Author = cheep.Author,};

        var result = await query.ToListAsync();
        return result;
        
    }

    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        Cheep cheep = new() {Text = alteredCheep.Message, Author = alteredCheep.Author};
        var result = await _dbContext.Cheeps.AddAsync(cheep);
        
        await _dbContext.SaveChangesAsync(); 
    }
}