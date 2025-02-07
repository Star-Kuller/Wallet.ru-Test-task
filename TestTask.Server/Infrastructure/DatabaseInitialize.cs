using Microsoft.AspNetCore.Connections;
using Npgsql;
using TestTask.Server.Interfaces;

namespace TestTask.Server.Infrastructure;

public static class DatabaseInitialize
{
    public static async Task CreateDatabaseIfNotExist(DatabaseParameters parameters, ILogger logger)
    {
        await using var conn = new NpgsqlConnection(parameters.PostgresHost);
        conn.Open();

        await using var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{parameters.DatabaseName}'", conn);
        var result = await cmd.ExecuteScalarAsync();
        if (result == null)
        {
            await using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{parameters.DatabaseName}\"", conn);
            await createCmd.ExecuteNonQueryAsync();
            logger.LogInformation("База данных \'{ParametersDatabaseName}\' успешно создана", parameters.DatabaseName);
        }
        else
        {
            logger.LogInformation("База данных \'{ParametersDatabaseName}\' уже существует", parameters.DatabaseName);
        }
    }

    public static async Task CreateTablesIfNotExist(IDbConnectionFactory connectionFactory, ILogger logger)
    {
        var conn = connectionFactory.NewConnection();
        await conn.OpenAsync();

        // Создаем таблицу, если она не существует
        await using var createTableCmd = new NpgsqlCommand(
            """
                        CREATE TABLE IF NOT EXISTS messages (
                        Id BIGSERIAL PRIMARY KEY,
                        OrderNumber INTEGER NOT NULL,
                        CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        Content TEXT NOT NULL
                    );
            """, conn);

        await createTableCmd.ExecuteNonQueryAsync();
        logger.LogInformation("Таблица 'messages' успешно создана или уже существует.");
    }
}