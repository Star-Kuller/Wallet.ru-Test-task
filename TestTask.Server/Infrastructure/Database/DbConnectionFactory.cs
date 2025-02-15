using Npgsql;
using TestTask.Server.Interfaces.Database;

namespace TestTask.Server.Infrastructure.Database;

public class DbConnectionFactory(DatabaseParameters parameters) : IDbConnectionFactory
{
    private readonly string _connectionString = $"{parameters.PostgresHost};Database={parameters.DatabaseName}";
    public NpgsqlConnection NewConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}