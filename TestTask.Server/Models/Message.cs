namespace TestTask.Server.Models;

public class Message()
{
    public long Id { get; set; }
    public int OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Content { get; set; } = string.Empty;
    
    public Message(string content, int orderNumber) : this()
    {
        Content = content;
        OrderNumber = orderNumber;
    }
}