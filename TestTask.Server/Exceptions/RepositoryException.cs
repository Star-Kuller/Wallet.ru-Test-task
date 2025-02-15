namespace TestTask.Server.Exceptions;

public class RepositoryException(string message, Exception innerException)
    : Exception(message, innerException);