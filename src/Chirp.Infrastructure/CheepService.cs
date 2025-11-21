using Chirp.Core;

//public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    Task<List<CheepDTO>> GetCheeps(int page = 1);
    Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page = 1);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;
    
	//32 messages per page
    private const int PageSize = 32;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
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