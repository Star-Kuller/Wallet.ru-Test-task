namespace TestTask.Client.Models;

public class Message
{
    public long? Id { get; set; }
    public int OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Content { get; set; } = string.Empty;
}