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

    public int CheepId { get; set; }
    public required string AuthorName { get; set; }
    
    public required string Timestamp { get; set; }
    
    public int LikeCount { get; set; }
    public bool HasLiked { get; set; }
    
}

public class Follow
{
    public int Id { get; set; }

    public string Follower { get; set; }
    public string Followee { get; set; }
}

public class Like
{
    public int Id { get; set; }
    public int CheepId { get; set; }
    public string Username { get; set; }
}

public class ApplicationUser : IdentityUser
{
    // You can add custom properties here later if needed
    // For example:
    // public string DisplayName { get; set; }
    // public DateTime BirthDate { get; set; }
}

// not currently used cause Author doesnt leave this layer
public class AuthorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// not currently used cause Author doesnt leave this layer
public class FollowDTO
{
    public string Follower { get; set; }
    public string Followee { get; set; }
}