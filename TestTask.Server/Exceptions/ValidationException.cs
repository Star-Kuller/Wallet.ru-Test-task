namespace TestTask.Server.Exceptions;

public class ValidationException(string message) : Exception(message);