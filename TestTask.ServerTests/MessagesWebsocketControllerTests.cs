using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using TestTask.Server.Controllers;
using TestTask.Server.Models;

namespace TestTask.Server_Tests;

public class MessagesWebsocketControllerTests
{
    private readonly Mock<ILogger<MessagesController>> _mockLogger;
    private readonly MessagesWebsocketController _controller;

    public MessagesWebsocketControllerTests()
    {
        _mockLogger = new Mock<ILogger<MessagesController>>();
        _controller = new MessagesWebsocketController(_mockLogger.Object);
    }

    #region BroadcastMessageAsync Tests

    [Fact]
    public async Task BroadcastMessageAsync_ShouldSendMessageToAllActiveSockets()
    {
        // Arrange
        var message = new Message
        {
            Id = 1,
            OrderNumber = 1,
            CreatedAt = DateTime.UtcNow,
            Content = "Test content"
        };

        var mockSocket1 = new Mock<WebSocket>();
        var mockSocket2 = new Mock<WebSocket>();

        mockSocket1.Setup(ws => ws.State).Returns(WebSocketState.Open);
        mockSocket2.Setup(ws => ws.State).Returns(WebSocketState.Open);

        _controller.Sockets.Add(mockSocket1.Object);
        _controller.Sockets.Add(mockSocket2.Object);

        var json = JsonConvert.SerializeObject(message);
        var bytes = Encoding.UTF8.GetBytes(json);

        // Act
        await _controller.BroadcastMessageAsync(message);

        // Assert
        mockSocket1.Verify(ws => ws.SendAsync(
                It.Is<ArraySegment<byte>>(segment => segment.Array.SequenceEqual(bytes)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None),
            Times.Once);

        mockSocket2.Verify(ws => ws.SendAsync(
                It.Is<ArraySegment<byte>>(segment => segment.Array.SequenceEqual(bytes)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task BroadcastMessageAsync_ShouldNotSendMessageToClosedSockets()
    {
        // Arrange
        var message = new Message
        {
            Id = 1,
            OrderNumber = 1,
            CreatedAt = DateTime.UtcNow,
            Content = "Test content"
        };

        var mockSocket1 = new Mock<WebSocket>();
        var mockSocket2 = new Mock<WebSocket>();

        mockSocket1.Setup(ws => ws.State).Returns(WebSocketState.Open);
        mockSocket2.Setup(ws => ws.State).Returns(WebSocketState.Closed);

        _controller.Sockets.Add(mockSocket1.Object);
        _controller.Sockets.Add(mockSocket2.Object);

        var json = JsonConvert.SerializeObject(message);
        var bytes = Encoding.UTF8.GetBytes(json);

        // Act
        await _controller.BroadcastMessageAsync(message);

        // Assert
        mockSocket1.Verify(ws => ws.SendAsync(
                It.Is<ArraySegment<byte>>(segment => segment.Array.SequenceEqual(bytes)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None),
            Times.Once);

        mockSocket2.Verify(ws => ws.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                It.IsAny<WebSocketMessageType>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion
}