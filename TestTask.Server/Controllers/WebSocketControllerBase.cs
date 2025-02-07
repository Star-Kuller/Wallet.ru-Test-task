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
            
            await OnClientConnected(webSocket);
            try
            {
                await Receive(webSocket);
            }
            finally
            {
                Sockets.TryTake(out _);
                await OnClientDisconnected(webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    private async Task Receive(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            await OnReceive(result);
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
    protected virtual Task OnReceive(WebSocketReceiveResult result) => Task.CompletedTask;
    protected virtual Task OnClientConnected(WebSocket webSocket) => Task.CompletedTask;
    protected virtual Task OnClientDisconnected(WebSocket webSocket) => Task.CompletedTask;
}