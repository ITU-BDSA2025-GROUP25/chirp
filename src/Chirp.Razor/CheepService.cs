public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    List<CheepViewModel> GetCheeps();
    List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    public List<CheepViewModel> GetCheeps()
    {
        return DBFacade.Cheeps();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return DBFacade.CheepsByAuthor(author);
    }
}