namespace Chirp.Razor;

public interface ICheepRepository
{
    Task CreateCheep(Cheep newCheep);
    Task<List<Cheep>> ReadCheep(String authorName);
    Task UpdateCheep(Cheep alteredCheep);
}

public class CheepRepository : ICheepRepository
{
    public Task CreateCheep(Cheep newCheep)
    {
        throw new NotImplementedException();
    }

    public Task<List<Cheep>> ReadCheep(String authorName)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCheep(Cheep alteredCheep)
    {
        throw new NotImplementedException();
    }
}