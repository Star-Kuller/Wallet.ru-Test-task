namespace TestTask.Server.Models;

public class Message
{
    public long? Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Content { get; set; } = string.Empty;
}