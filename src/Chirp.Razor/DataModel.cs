namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    
    public string name { get; set; }
    
    public string email { get; set; }
    
    public ICollection<Cheep> cheeps { get; set; }
}

public class Cheep
{
    public int CheepId { get; set; }
    
    public string Message { get; set; }

    public DateTime timeStamp { get; set; }

    public Author Author { get; set; }
}

public class CheepDTO
{
    public string Message { get; set; }
    
    public Author Author { get; set; }
    
    public string Timestamp { get; set; }
    
}