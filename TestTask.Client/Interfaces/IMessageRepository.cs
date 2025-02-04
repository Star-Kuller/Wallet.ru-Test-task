using TestTask.Client.Models;

namespace TestTask.Client.Interfaces;

public interface IMessageRepository
{
    Task<long> AddMessage(Message message);
    Task<IEnumerable<long>> GetMessages(DateTime? from, DateTime? to);
}