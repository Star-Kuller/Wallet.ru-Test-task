using System.Data;

namespace TestTask.Server.Interfaces.Database;

public interface IDatabaseExecutor
{
    Task<T?> ExecuteCommandAsync<T>(string query, IDictionary<string, object?> parameters, CancellationToken token = default);
    Task<IEnumerable<T>> GetListAsync<T>(string query, IDictionary<string, object?> parameters, Func<IDataRecord, T> map, CancellationToken token = default);
}