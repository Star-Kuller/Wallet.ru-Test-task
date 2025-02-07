using TestTask.Server.Models;

namespace TestTask.Server.Interfaces;

public interface IMessagesRepository
{
    Task AddMessageAsync(Message message, CancellationToken token = default);
    Task<IEnumerable<long>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default);
}