using Npgsql;
using TestTask.Server.Interfaces;

namespace TestTask.Server.Infrastructure;

public class DbConnectionFactory(DatabaseParameters parameters) : IDbConnectionFactory
{
    private readonly string _connectionString = $"{parameters.PostgresHost};Database={parameters.DatabaseName}";
    public NpgsqlConnection NewConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}