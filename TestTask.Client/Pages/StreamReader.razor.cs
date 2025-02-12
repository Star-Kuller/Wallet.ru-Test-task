using System.Net.WebSockets;
using Microsoft.AspNetCore.Components;
using TestTask.Client.Infrastructure;
using TestTask.Client.Interfaces;
using TestTask.Client.Models;

namespace TestTask.Client.Pages;

public partial class StreamReader : ComponentBase, IDisposable
{
    private readonly List<Message> _receivedMessages = [];
    private Error? _error = null;
    [Inject]
    private IWebSocketService<Message> WebSocketService { get; set; }

    private void Receive(Message? message)
    {
        if (message is null) return;
        _receivedMessages.Add(message);
        InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        WebSocketService.OnMessageReceived += Receive;
        try
        {
            await WebSocketService.ConnectAsync();
        }
        catch (WebSocketException e)
        {
            _error = new Error($"Ошибка веб-сокета: {e.Message}");
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            _error = new Error($"Необработанная ошибка при подключении: {e.Message}");
            Console.WriteLine(e);
        }
    }
    
    public void Dispose()
    {
        WebSocketService.OnMessageReceived -= Receive;
    }
}