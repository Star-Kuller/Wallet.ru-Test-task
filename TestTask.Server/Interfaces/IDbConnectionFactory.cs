using Npgsql;

namespace TestTask.Server.Interfaces;

public interface IDbConnectionFactory
{
    NpgsqlConnection NewConnection();
}