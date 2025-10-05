namespace MyChat.Razor.Models;

public class Message
{
    public int MessageId { get; set; }  // Primary key
    public int UserId { get; set; }
    public string Text { get; set; }
    public User User { get; set; }
    

}