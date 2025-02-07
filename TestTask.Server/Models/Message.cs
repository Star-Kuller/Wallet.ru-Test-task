namespace TestTask.Server.Models;

public class Message()
{
    //Выставляется БД, необходим, чтобы гарантировать уникальности записи
    public long Id { get; set; }
    //Порядковый номер сообщения от одного экземпляра клиента, задаётся на клиенте по условию задания
    public int OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Content { get; set; } = string.Empty;
    
    public Message(string content, int orderNumber) : this()
    {
        Content = content;
        OrderNumber = orderNumber;
    }
}