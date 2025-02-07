using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using TestTask.Server.Interfaces;
using TestTask.Server.Models;

namespace TestTask.Server.Controllers;

public class MessagesWebsocketController(
    ILogger<MessagesController> logger) : WebSocketControllerBase, IMessagesWebsocketController
{
    protected override void OnClientConnected(WebSocket webSocket)
    {
        logger.LogInformation("Клиент подключен. Текущее количество клиентов: {SocketsCount}", Sockets.Count);
    }
    
    protected override void OnClientDisconnected(WebSocket webSocket)
    {
        logger.LogInformation("Клиент отключен. Осталось клиентов: {SocketsCount}", Sockets.Count);
    }
    
    public async Task BroadcastMessageAsync(Message message, CancellationToken token = default)
    {
        var json = JsonConvert.SerializeObject(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        await BroadcastAsync(bytes);
    }
}