using TestTask.Server.Exceptions;

namespace TestTask.Server.Infrastructure;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.ContentType = "application/text";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            logger.LogWarning(ex, "Ошибка валидации");
            await context.Response.WriteAsync(ex.Message);
        }
        catch (RepositoryException ex)
        {
            context.Response.ContentType = "application/text";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogWarning(ex, "Ошибка базы данных данных");
            await context.Response.WriteAsync("Ошибка базы данных базу данных");
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/text";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogCritical(ex, "Необработанная ошибка");
            //Передаём ошибку, но не stacktrace
            await context.Response.WriteAsync(ex.Message);
        }
    }
}