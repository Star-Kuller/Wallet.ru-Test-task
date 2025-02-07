namespace TestTask.Server.Interfaces;

public interface IWebsocketController
{
    Task HandleWebSocket(HttpContext context);
}