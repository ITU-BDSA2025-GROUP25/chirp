namespace MyChat.Razor.Models;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public ICollection<Message> Messages { get; set; }
}