using TestTask.Client.Interfaces;
using TestTask.Client.Models;

namespace TestTask.Client.Models;

public class MessageRepository : IMessageRepository
{
    public Task<long> AddMessage(Message message)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<long>> GetMessages(DateTime? from, DateTime? to)
    {
        throw new NotImplementedException();
    }
}