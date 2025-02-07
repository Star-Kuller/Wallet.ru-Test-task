using TestTask.Server.Interfaces;
using TestTask.Server.Models;

namespace TestTask.Server.Controllers;

public class MessagesWebsocketController(
    ILogger<MessagesController> logger) : IMessagesWebsocketController
{
    public Task SendMessageAsync(Message message, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}