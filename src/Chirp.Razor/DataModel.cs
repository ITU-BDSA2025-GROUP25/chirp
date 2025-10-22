namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    
    public required string Name { get; set; }
    
    public required string Email { get; set; }
    
    public List<Cheep> Cheeps { get; set; } = new(); // Default empty list
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

public class AuthorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
}
