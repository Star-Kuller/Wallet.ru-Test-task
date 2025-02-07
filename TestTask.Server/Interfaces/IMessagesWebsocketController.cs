
using TestTask.Server.Models;

namespace TestTask.Server.Interfaces;

public interface IMessagesWebsocketController
{
    Task SendMessageAsync(Message message, CancellationToken token = default);
}