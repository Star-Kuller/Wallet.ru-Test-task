using System.Text;
using Npgsql;
using TestTask.Server.Interfaces;

namespace TestTask.Server.Models;

public class MessagesRepository(IDbConnectionFactory connectionFactory) : IMessagesRepository
{
    public async Task AddMessageAsync(Message message, CancellationToken token = default)
    {
        await using var conn = connectionFactory.NewConnection();
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

    public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default)
    {
        await using var conn = connectionFactory.NewConnection();
        await conn.OpenAsync(token);
        
        var queryBuilder = new StringBuilder("SELECT * FROM messages WHERE 1=1");
        if (from.HasValue)
            queryBuilder.Append(" AND CreatedAt >= @From");
        if (to.HasValue)
            queryBuilder.Append(" AND CreatedAt <= @To");
        var query = queryBuilder.ToString();
        await using var cmd = new NpgsqlCommand(query, conn);
        
        if (from.HasValue)
            cmd.Parameters.AddWithValue("From", from.Value);
        if (to.HasValue)
            cmd.Parameters.AddWithValue("To", to.Value);
        
        var messages = new List<Message>();
        await using var reader = await cmd.ExecuteReaderAsync(token);

        while (await reader.ReadAsync(token))
        {
            messages.Add(new Message
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                OrderNumber = reader.GetInt32(reader.GetOrdinal("OrderNumber")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                Content = reader.GetString(reader.GetOrdinal("Content"))
            });
        }
        return messages;
    }
}