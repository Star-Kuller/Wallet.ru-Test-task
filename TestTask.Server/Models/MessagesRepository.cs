using System.Data;
using System.Text;
using TestTask.Server.Exceptions;
using TestTask.Server.Interfaces;
using TestTask.Server.Interfaces.Database;

namespace TestTask.Server.Models;

public class MessagesRepository(IDatabaseExecutor databaseExecutor)
    : IMessagesRepository
{
    public async Task AddMessageAsync(Message message, CancellationToken token = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message), "Message cannot be null");
        
        try
        {
            const string insertQuery = """
                INSERT INTO messages (OrderNumber, CreatedAt, Content)
                VALUES (@OrderNumber, @CreatedAt, @Content)
                RETURNING Id;
            """;

            var parameters = new Dictionary<string, object?>
            {
                { "OrderNumber", message.OrderNumber },
                { "CreatedAt", message.CreatedAt },
                { "Content", message.Content }
            };

            var id = await databaseExecutor.ExecuteCommandAsync<long>(insertQuery, parameters, token);
            message.Id = id;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to add message.", ex);
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default)
    {
        try
        {
            var query = BuildGetMessagesQuery(from, to);
            
            var parameters = new Dictionary<string, object?>();
            if (from.HasValue)
                parameters["From"] = from.Value;
            if (to.HasValue)
                parameters["To"] = to.Value;

            return await databaseExecutor.GetListAsync(query, parameters, MapMessage, token);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to retrieve messages.", ex);
        }
    }
    
    private static string BuildGetMessagesQuery(DateTime? from, DateTime? to)
    {
        var queryBuilder = new StringBuilder("SELECT * FROM messages");
        if (!from.HasValue && !to.HasValue) return queryBuilder.ToString();
        
        queryBuilder.Append(" WHERE ");
        if (from.HasValue)
            queryBuilder.Append("CreatedAt >= @From");
        if (from.HasValue && to.HasValue)
            queryBuilder.Append(" AND ");
        if (to.HasValue)
            queryBuilder.Append("CreatedAt <= @To");
        return queryBuilder.ToString();
    }
    
    private static Message MapMessage(IDataRecord record)
    {
        return new Message
        {
            Id = record.GetInt64(record.GetOrdinal("Id")),
            OrderNumber = record.GetInt32(record.GetOrdinal("OrderNumber")),
            CreatedAt = record.GetDateTime(record.GetOrdinal("CreatedAt")),
            Content = record.GetString(record.GetOrdinal("Content"))
        };
    }
}