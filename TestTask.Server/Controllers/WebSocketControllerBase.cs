using System.Collections.Concurrent;
using System.Net.WebSockets;
using TestTask.Server.Interfaces;

namespace TestTask.Server.Controllers;

public abstract class WebSocketControllerBase : IWebsocketController
{
    protected readonly ConcurrentBag<WebSocket> Sockets = [];
    
    public async Task HandleWebSocket(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            Sockets.Add(webSocket);
            
            OnClientConnected(webSocket);
            try
            {
                await WaitForClose(webSocket);
            }
            finally
            {
                Sockets.TryTake(out _);
                OnClientDisconnected(webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    private static async Task WaitForClose(WebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>([]), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", CancellationToken.None);
            }
        }
    }
    
    protected async Task BroadcastAsync(byte[] messageBytes)
    {
        foreach (var socket in Sockets.Where(socket => socket.State == WebSocketState.Open))
        {
            await socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
    
    protected virtual void OnClientConnected(WebSocket webSocket) { }
    protected virtual void OnClientDisconnected(WebSocket webSocket) { }
}