using Microsoft.AspNetCore.Components;
using TestTask.Client.Infrastructure;
using TestTask.Client.Interfaces;
using TestTask.Client.Models;

namespace TestTask.Client.Pages;

public partial class StreamReader : ComponentBase, IDisposable
{
    private List<Message> _receivedMessages = [];
    private Error? _error = null;
    [Inject]
    private IWebSocketService<Message> WebSocketService { get; set; }

    private void Receive(Message? message)
    {
        if (message is null) return;
        _receivedMessages.Add(message);
        InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        WebSocketService.OnMessageReceived += Receive;
        WebSocketService.ConnectAsync();
    }
    
    public void Dispose()
    {
        WebSocketService.OnMessageReceived -= Receive;
    }
}