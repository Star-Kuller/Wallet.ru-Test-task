namespace TestTask.Client.Interfaces;

public interface IWebSocketService<T>
{
    Task ConnectAsync();
    event Action<T?> OnMessageReceived;
}