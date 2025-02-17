using System.Data;
using Npgsql;
using TestTask.Server.Interfaces.Database;

namespace TestTask.Server.Infrastructure.Database;

public class DatabaseExecutor(IDbConnectionFactory connectionFactory) : IDatabaseExecutor
{
    public async Task<T?> ExecuteCommandAsync<T>(string query, IDictionary<string, object?> parameters, CancellationToken token = default)
    {
        await using var conn = connectionFactory.NewConnection();
        await conn.OpenAsync(token);
        
        await using var cmd = GetCommand(conn, query, parameters);
        var result = await cmd.ExecuteScalarAsync(token);
        return (T?)Convert.ChangeType(result, typeof(T));
    }

    public async Task<IEnumerable<T>> GetListAsync<T>(string query, IDictionary<string, object?> parameters, Func<IDataRecord, T> map, CancellationToken token = default)
    {
        await using var conn = connectionFactory.NewConnection();
        await conn.OpenAsync(token);
        
        await using var cmd = GetCommand(conn, query, parameters);
        await using var reader = await cmd.ExecuteReaderAsync(token);
        
        var results = new List<T>();
        while (await reader.ReadAsync(token))
        {
            results.Add(map(reader));
        }
        return results;
    }

    private NpgsqlCommand GetCommand(NpgsqlConnection conn, string query, IDictionary<string, object?> parameters)
    {
        var cmd = new NpgsqlCommand(query, conn);
        foreach (var param in parameters)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }
        return cmd;
    }
}