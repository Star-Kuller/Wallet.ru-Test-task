using System.Data;
using Moq;
using TestTask.Server.Exceptions;
using TestTask.Server.Interfaces.Database;
using TestTask.Server.Models;

namespace TestTask.Server_Tests;

public class MessagesRepositoryTests
{
    private readonly Mock<IDatabaseExecutor> _mockDatabaseExecutor;
    private readonly MessagesRepository _repository;

    public MessagesRepositoryTests()
    {
        _mockDatabaseExecutor = new Mock<IDatabaseExecutor>();
        _repository = new MessagesRepository(_mockDatabaseExecutor.Object);
    }

    #region AddMessageAsync Tests

    [Fact]
    public async Task AddMessageAsync_ShouldAddMessageAndSetId_WhenMessageIsValid()
    {
        // Arrange
        var message = new Message
        {
            OrderNumber = 1,
            CreatedAt = DateTime.UtcNow,
            Content = "Test content"
        };

        _mockDatabaseExecutor
            .Setup(executor => executor.ExecuteCommandAsync<long>(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(123L);
        
        const string insertQuery = """
                                       INSERT INTO messages (OrderNumber, CreatedAt, Content)
                                       VALUES (@OrderNumber, @CreatedAt, @Content)
                                       RETURNING Id;
                                   """;
        
        // Act
        await _repository.AddMessageAsync(message);

        // Assert
        Assert.Equal(123L, message.Id);
        _mockDatabaseExecutor.Verify(executor => executor.ExecuteCommandAsync<long>(
                It.Is<string>(query => query == insertQuery),
                It.Is<IDictionary<string, object?>>(parameters =>
                    parameters.ContainsKey("OrderNumber") &&
                    parameters["OrderNumber"] is int &&
                    (int)parameters["OrderNumber"]! == message.OrderNumber &&
                    parameters.ContainsKey("CreatedAt") &&
                    parameters["CreatedAt"] is DateTime &&
                    (DateTime)parameters["CreatedAt"]! == message.CreatedAt &&
                    parameters.ContainsKey("Content") &&
                    parameters["Content"] is string &&
                    (string)parameters["Content"]! == message.Content),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddMessageAsync_ShouldThrowArgumentNullException_WhenMessageIsNull()
    {
        // Arrange
        Message message = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddMessageAsync(message));
    }

    [Fact]
    public async Task AddMessageAsync_ShouldThrowRepositoryException_WhenDatabaseExecutorThrowsException()
    {
        // Arrange
        var message = new Message
        {
            OrderNumber = 1,
            CreatedAt = DateTime.UtcNow,
            Content = "Test content"
        };

        _mockDatabaseExecutor
            .Setup(executor => executor.ExecuteCommandAsync<long>(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RepositoryException>(() => _repository.AddMessageAsync(message));
        Assert.Equal("Failed to add message.", exception.Message);
    }

    #endregion

    #region GetMessagesAsync Tests

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnAllMessages_WhenNoDateRangeProvided()
    {
        // Arrange
        var expectedMessages = new List<Message>
        {
            new Message { Id = 1, OrderNumber = 1, CreatedAt = DateTime.UtcNow, Content = "Message 1" },
            new Message { Id = 2, OrderNumber = 2, CreatedAt = DateTime.UtcNow, Content = "Message 2" }
        };

        _mockDatabaseExecutor
            .Setup(executor => executor.GetListAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _repository.GetMessagesAsync(null, null);

        // Assert
        Assert.Equal(expectedMessages.Count, result.Count());
        _mockDatabaseExecutor.Verify(executor => executor.GetListAsync(
                It.Is<string>(query => query == "SELECT * FROM messages"),
                It.Is<IDictionary<string, object?>>(parameters => parameters.Count == 0),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessagesWithStartDate_WhenOnlyFromDateProvided()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-1);
        var expectedMessages = new List<Message>
        {
            new Message { Id = 1, OrderNumber = 1, CreatedAt = fromDate.AddHours(1), Content = "Message 1" }
        };

        _mockDatabaseExecutor
            .Setup(executor => executor.GetListAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _repository.GetMessagesAsync(fromDate, null);

        // Assert
        Assert.Single(result);
        _mockDatabaseExecutor.Verify(executor => executor.GetListAsync(
                It.Is<string>(query => query.Contains("WHERE CreatedAt >= @From")),
                It.Is<IDictionary<string, object?>>(parameters => parameters.ContainsKey("From")),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessagesWithEndDate_WhenOnlyToDateProvided()
    {
        // Arrange
        var toDate = DateTime.UtcNow.AddDays(1);
        var expectedMessages = new List<Message>
        {
            new Message { Id = 1, OrderNumber = 1, CreatedAt = toDate.AddHours(-1), Content = "Message 1" }
        };

        _mockDatabaseExecutor
            .Setup(executor => executor.GetListAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _repository.GetMessagesAsync(null, toDate);

        // Assert
        Assert.Single(result);
        _mockDatabaseExecutor.Verify(executor => executor.GetListAsync(
                It.Is<string>(query => query.Contains("WHERE CreatedAt <= @To")),
                It.Is<IDictionary<string, object?>>(parameters => parameters.ContainsKey("To")),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessagesWithinDateRange_WhenBothDatesProvided()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-1);
        var toDate = DateTime.UtcNow.AddDays(1);
        var expectedMessages = new List<Message>
        {
            new Message { Id = 1, OrderNumber = 1, CreatedAt = fromDate.AddHours(1), Content = "Message 1" }
        };

        _mockDatabaseExecutor
            .Setup(executor => executor.GetListAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _repository.GetMessagesAsync(fromDate, toDate);

        // Assert
        Assert.Single(result);
        _mockDatabaseExecutor.Verify(executor => executor.GetListAsync(
                It.Is<string>(query => query.Contains("WHERE CreatedAt >= @From AND CreatedAt <= @To")),
                It.Is<IDictionary<string, object?>>(parameters =>
                    parameters.ContainsKey("From") && parameters.ContainsKey("To")),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldThrowRepositoryException_WhenDatabaseExecutorThrowsException()
    {
        // Arrange
        _mockDatabaseExecutor
            .Setup(executor => executor.GetListAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<Func<IDataRecord, Message>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RepositoryException>(() => _repository.GetMessagesAsync(null, null));
        Assert.Equal("Failed to retrieve messages.", exception.Message);
    }

    #endregion
}