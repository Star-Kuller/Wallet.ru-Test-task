namespace TestTask.Server.Infrastructure;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            context.Response.ContentType = "application/text";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            logger.LogWarning(e.Message);
            await context.Response.WriteAsync(e.Message);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "application/text";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogCritical("Error: {Message} | StackTrace:\n{StackTrace}", e.Message, e.StackTrace);
            //Передаём ошибку, но не stacktrace
            await context.Response.WriteAsync(e.Message);
        }
    }
}