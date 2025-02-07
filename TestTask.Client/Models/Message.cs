namespace TestTask.Client.Models;

public class Message
{
    public long? Id { get; init; }
    public int OrderNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Content { get; init; }
    
    public DateTime LocalCreatedAt => CreatedAt.ToLocalTime();
}