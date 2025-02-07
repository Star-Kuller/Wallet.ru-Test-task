using System.Net;

namespace TestTask.Client.Infrastructure.Exceptions;

public class InternalServerErrorHttpException(string? message)
    : HttpRequestException(message, null, HttpStatusCode.InternalServerError);