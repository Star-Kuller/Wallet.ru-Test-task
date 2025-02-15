using Microsoft.AspNetCore.Mvc;
using Moq;
using TestTask.Server.Controllers;
using TestTask.Server.DTOs;
using TestTask.Server.Exceptions;
using TestTask.Server.Interfaces;
using TestTask.Server.Models;

namespace TestTask.Server_Tests;

public class MessagesControllerTests
{
    private readonly Mock<IMessagesRepository> _mockRepository;
    private readonly Mock<IMessagesWebsocketController> _mockWebsocketController;
    private readonly MessagesController _controller;

    public MessagesControllerTests()
    {
        _mockRepository = new Mock<IMessagesRepository>();
        _mockWebsocketController = new Mock<IMessagesWebsocketController>();
        _controller = new MessagesController(_mockRepository.Object, _mockWebsocketController.Object);
    }

    #region GetMessages Tests

    [Fact]
    public async Task GetMessages_ShouldReturnAllMessages_WhenNoDateRangeProvided()
    {
        // Arrange
        var expectedMessages = new List<Message>
        {
            new() { Id = 1, OrderNumber = 1, CreatedAt = DateTime.UtcNow, Content = "Message 1" },
            new() { Id = 2, OrderNumber = 2, CreatedAt = DateTime.UtcNow, Content = "Message 2" }
        };

        _mockRepository
            .Setup(repo => repo.GetMessagesAsync(null, null, default))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _controller.GetMessages(null, null);

        // Assert
        var messages = Assert.IsAssignableFrom<IEnumerable<Message>>(result);
        Assert.Equal(expectedMessages.Count, messages.Count());
    }

    [Fact]
    public async Task GetMessages_ShouldThrowValidationException_WhenFromDateIsGreaterThanToDate()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(1);
        var toDate = DateTime.UtcNow;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _controller.GetMessages(fromDate, toDate));
        Assert.Equal("Начальная дата не может быть больше конечной", exception.Message);
    }

    #endregion

    #region CreateMessage Tests

    [Fact]
    public async Task CreateMessage_ShouldAddMessageAndBroadcastIt_WhenMessageIsValid()
    {
        // Arrange
        var messageDto = new MessageDto
        (
            Number: 1,
            Content: "Test content"
        );

        // Act
        var result = await _controller.CreateMessage(messageDto);

        // Assert
        Assert.IsType<CreatedResult>(result);

        _mockRepository.Verify(repo => repo.AddMessageAsync(
                It.Is<Message>(msg =>
                    msg.OrderNumber == messageDto.Number &&
                    msg.Content == messageDto.Content),
                default),
            Times.Once);

        _mockWebsocketController.Verify(ws => ws.BroadcastMessageAsync(
                It.Is<Message>(msg =>
                    msg.OrderNumber == messageDto.Number &&
                    msg.Content == messageDto.Content),
                default),
            Times.Once);
    }

    [Fact]
    public async Task CreateMessage_ShouldThrowValidationException_WhenContentLengthExceedsLimit()
    {
        // Arrange
        var messageDto = new MessageDto
        (
            Number: 1,
            Content: new string('a', 129)
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _controller.CreateMessage(messageDto));
        Assert.Equal("Строка в сообщении превышает 128 символов", exception.Message);
    }

    #endregion
}