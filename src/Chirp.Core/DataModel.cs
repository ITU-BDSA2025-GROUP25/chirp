using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

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

    //Internal key by Email
    public required string AuthorKey { get; set; }

    //username displayed in ui
    public required string AuthorDisplayName { get; set; }
    
    public required string Timestamp { get; set; }
    
    public int LikeCount { get; set; }
    public bool HasLiked { get; set; }
    
}

public class Follow
{
    public int Id { get; set; }

    public required string Follower { get; set; }
    public required string Followee { get; set; }
}

public class Like
{
    public int Id { get; set; }
    public int CheepId { get; set; }
    public required string Username { get; set; }
}

public class ApplicationUser : IdentityUser
{
    [Required] 
    [StringLength(20)] 
    public string DisplayName { get; set; } = "";
}

// not currently used cause Author doesnt leave this layer
//public class AuthorDTO
//{
  //  public required string Name { get; set; }
    //public required string Email { get; set; }
//}
// not currently used cause Author doesnt leave this layer
public class FollowDTO
{
    public required string Follower { get; set; }
    public required string Followee { get; set; }
}