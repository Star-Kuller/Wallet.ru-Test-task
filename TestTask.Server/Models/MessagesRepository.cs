using TestTask.Server.Interfaces;

namespace TestTask.Server.Models;

public class MessagesRepository(IDbConnectionFactory connectionFactory) : IMessagesRepository
{
    public async Task AddMessageAsync(Message message, CancellationToken token = default)
    {
        var conn = connectionFactory.NewConnection();
        await conn.OpenAsync(token);
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<long>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}