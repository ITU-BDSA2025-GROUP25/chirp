namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    
    public required string Name { get; set; }
    
    public required string Email { get; set; }
    
    public required ICollection<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    public int CheepId { get; set; }
    
    public required string Text { get; set; }

    public DateTime TimeStamp { get; set; }

    public required Author Author { get; set; }
    
    public int AuthorId { get; set; }
}

public class CheepDTO
{
    public required string Message { get; set; }
    
    public required Author Author { get; set; }
    
    public required string Timestamp { get; set; }
    
}