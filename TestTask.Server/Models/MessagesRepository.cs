using Npgsql;
using TestTask.Server.Interfaces;

namespace TestTask.Server.Models;

public class MessagesRepository(IDbConnectionFactory connectionFactory) : IMessagesRepository
{
    public async Task AddMessageAsync(Message message, CancellationToken token = default)
    {
        var conn = connectionFactory.NewConnection();
        await conn.OpenAsync(token);
        
        const string insertQuery =
            """
            INSERT INTO messages (OrderNumber, Content)
                    VALUES (@OrderNumber, @Content)
                    RETURNING Id;
            """;

        await using var cmd = new NpgsqlCommand(insertQuery, conn);
        cmd.Parameters.AddWithValue("OrderNumber", message.OrderNumber);
        cmd.Parameters.AddWithValue("Content", message.Content);
        
        var result = await cmd.ExecuteScalarAsync(token);
        if (result is long id)
            message.Id = id;
    }

    public async Task<IEnumerable<long>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}