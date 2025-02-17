using Npgsql;
using TestTask.Server.Interfaces.Database;

namespace TestTask.Server.Infrastructure.Database;

public static class DatabaseInitialize
{
    public static async Task CreateDatabaseIfNotExist(DatabaseParameters parameters, ILogger logger)
    {
        const int retryCount = 5;
        const int delayMs = 3000;

        for (var attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                await using var conn = new NpgsqlConnection(parameters.PostgresHost);
                conn.Open();
                await using var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{parameters.DatabaseName}'", conn);
                var result = await cmd.ExecuteScalarAsync();

                if (result == null)
                {
                    await using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{parameters.DatabaseName}\"", conn);
                    await createCmd.ExecuteNonQueryAsync();
                    logger.LogInformation("База данных '{ParametersDatabaseName}' успешно создана", parameters.DatabaseName);
                }
                else
                {
                    logger.LogInformation("База данных '{ParametersDatabaseName}' уже существует", parameters.DatabaseName);
                }

                return;
            }
            catch (NpgsqlException ex)
            {
                logger.LogWarning(ex, "Попытка {Attempt} из {RetryCount} завершилась неудачей. Повтор через {DelayMs} мс...", attempt, retryCount, delayMs);
                await Task.Delay(delayMs);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Ошибка создания базы данных");
            }
        }
    }

    public static async Task CreateTablesIfNotExist(IDbConnectionFactory connectionFactory, ILogger logger)
    {
        try
        {
            await using var conn = connectionFactory.NewConnection();
            await conn.OpenAsync();
        
            await using var createTableCmd = new NpgsqlCommand(
                """
                            CREATE TABLE IF NOT EXISTS messages (
                            Id BIGSERIAL PRIMARY KEY,
                            OrderNumber INTEGER NOT NULL,
                            CreatedAt TIMESTAMP NOT NULL,
                            Content TEXT NOT NULL
                        );
                """, conn);

            await createTableCmd.ExecuteNonQueryAsync();
            logger.LogInformation("Таблица 'messages' успешно создана или уже существует");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Ошибка создания таблиц");
        }
    }
}