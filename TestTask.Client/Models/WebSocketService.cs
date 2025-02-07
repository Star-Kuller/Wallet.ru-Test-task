using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using TestTask.Client.Interfaces;

namespace TestTask.Client.Models;

public class WebSocketService<T>(Uri uri) : IWebSocketService<T>, IAsyncDisposable
{
    private readonly ClientWebSocket _webSocket = new();

    public event Action<T?> OnMessageReceived;
    
    public async Task ConnectAsync()
    {
        await _webSocket.ConnectAsync(uri, CancellationToken.None);
        _ = StartReceivingMessagesAsync();
    }

    private async Task CloseAsync()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", CancellationToken.None);
        }
    }

    public async Task SendMessageAsync(T message)
    {
        var json = JsonConvert.SerializeObject(message);
        var bytes = Encoding.UTF8.GetBytes(json);

        await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task StartReceivingMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    var messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var message = JsonConvert.DeserializeObject<T>(messageText);
                    OnMessageReceived?.Invoke(message);
                    break;

                case WebSocketMessageType.Close:
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", CancellationToken.None);
                    break;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
    }
}