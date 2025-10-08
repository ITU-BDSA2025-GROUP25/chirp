namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public ICollection<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    public int CheepId { get; set; }
    
    public int AuthorId { get; set; }
    
    public string Text { get; set; }

    public DateTime TimeStamp { get; set; }

    public Author Author { get; set; }
}

public class CheepDTO
{
    public string text { get; set; }
    
    public Author author { get; set; }
    
}