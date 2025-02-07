namespace TestTask.Client.Infrastructure.Exceptions;

public class NoConnectHttpException(string? message, Exception? inner)
    : HttpRequestException(message, inner, null);