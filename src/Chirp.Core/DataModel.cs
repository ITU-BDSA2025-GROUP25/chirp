using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

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
    [Required(ErrorMessage = "Cheep message is required")]
    [StringLength(160, ErrorMessage = "Cheep cannot exceed 160 characters")]
    public required string Message { get; set; }
    
    public required Author Author { get; set; }
    
    public required string Timestamp { get; set; }
    
}

public class Follow
{
    public int Id { get; set; }

    public string Follower { get; set; }
    public string Followee { get; set; }
}


public class ApplicationUser : IdentityUser
{
    // You can add custom properties here later if needed
    // For example:
    // public string DisplayName { get; set; }
    // public DateTime BirthDate { get; set; }
}

public class AuthorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
}
