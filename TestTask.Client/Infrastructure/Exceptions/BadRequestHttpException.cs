using System.Net;

namespace TestTask.Client.Infrastructure.Exceptions;

public class BadRequestHttpException(string? message)
    : HttpRequestException(message, null, HttpStatusCode.BadRequest);