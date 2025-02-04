using TestTask.Server.Models;

namespace TestTask.Server.Interfaces;

public interface IMessagesRepository
{
    Task<long> AddMessage(Message message);
    Task<IEnumerable<long>> GetMessages(DateTime? from, DateTime? to);
}