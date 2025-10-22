using System.Collections.Generic;
using Chirp.Razor;

//public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    Task<List<CheepDTO>> GetCheeps(int page = 1);
    Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;
    
    private const int PageSize = 32;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }
    

    private static int CalculateOffset(int page)
    {
        if (page < 1) page = 1;
        return (page - 1) * PageSize;
    }

    public async Task <List<CheepDTO>> GetCheeps(int page = 1)
    {
        return await _repository.ReadCheep(page, PageSize);
    }

    public async Task <List<CheepDTO>> GetCheepsFromAuthor(String author ,int page = 1)
    {
        return await _repository.ReadCheepByAuthor(author, page, PageSize);
    }
}