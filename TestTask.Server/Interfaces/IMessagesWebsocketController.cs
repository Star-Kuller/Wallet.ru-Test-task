
using TestTask.Server.Models;

namespace TestTask.Server.Interfaces;

public interface IMessagesWebsocketController : IWebsocketController
{
    Task BroadcastMessageAsync(Message message, CancellationToken token = default);
}