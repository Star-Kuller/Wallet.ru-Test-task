using TestTask.Server.Interfaces;

namespace TestTask.Server.Models;

public class MessagesRepository : IMessagesRepository
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