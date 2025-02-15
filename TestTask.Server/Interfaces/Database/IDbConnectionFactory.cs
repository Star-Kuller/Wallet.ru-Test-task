using Npgsql;

namespace TestTask.Server.Interfaces.Database;

public interface IDbConnectionFactory
{
    NpgsqlConnection NewConnection();
}