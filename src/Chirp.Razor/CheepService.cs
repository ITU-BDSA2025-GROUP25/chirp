using System.Collections.Generic;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    List<CheepViewModel> GetCheeps(int page = 1);
    List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1);
}

public class CheepService : ICheepService
{
    private const int PageSize = 32;

    private static int CalculateOffset(int page)
    {
        if (page < 1) page = 1;
        return (page - 1) * PageSize;
    }

    public List<CheepViewModel> GetCheeps(int page = 1)
    {
        return DBFacade.Cheeps(PageSize, CalculateOffset(page));
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1)
    {
        return DBFacade.CheepsByAuthor(author, PageSize, CalculateOffset(page));
    }
}