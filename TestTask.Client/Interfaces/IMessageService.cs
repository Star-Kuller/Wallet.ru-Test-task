using TestTask.Client.DTOs;
using TestTask.Client.Models;

namespace TestTask.Client.Interfaces;

public interface IMessageService
{
    Task AddMessageAsync(MessageDto message, CancellationToken token = default);
    Task<IEnumerable<Message>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default);
}