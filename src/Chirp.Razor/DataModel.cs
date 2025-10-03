namespace Chirp.Razor;

public class Author
{
    public string name { get; set; }
    
    public string email { get; set; }
    
    public ICollection<Cheep> cheeps { get; set; }
}

public class Cheep
{
    public string text { get; set; }

    public DateTime timeStamp { get; set; }

    public Author author { get; set; }
}