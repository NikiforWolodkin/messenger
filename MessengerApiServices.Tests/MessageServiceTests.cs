using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Interfaces;
using MessengerApiServices.Services;
using MessengerModels.Models;
using Moq;
using Xunit;

namespace MessengerApiServices.Tests;

public class MessageServiceTests
{
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>();
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IChatRepository> _chatRepositoryMock = new Mock<IChatRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

    [Fact]
    public async Task AddAsync_ShouldReturnMessageResponse_WhenUserAndChatExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        var user = new User { Id = userId };
        var chat = new Chat { Id = chatId };
        var messageResponse = new MessageResponse();
        var messageAddRequest = new MessageAddRequest { ChatId = chatId, Text = "Test message", ImageUrl = "https://test.com/image.jpg" };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
        _messageRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<MessageResponse>(It.IsAny<Message>())).Returns(messageResponse);

        IMessageService service = new MessageService(_messageRepositoryMock.Object, _userRepositoryMock.Object, _chatRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.AddAsync(userId, messageAddRequest);

        // Assert
        Assert.IsType<MessageResponse>(result);
    }

    [Fact]
    public async Task AddUserReportAsync_ShouldReturnMessageResponse_WhenUserAndMessageExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var user = new User { Id = userId };
        var message = new Message { Id = messageId, UserReports = new List<User>() };
        var messageResponse = new MessageResponse();
        var userReportAddRequest = new UserReportAddRequest { MessageId = messageId };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _messageRepositoryMock.Setup(x => x.GetByIdAsync(messageId)).ReturnsAsync(message);
        _messageRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<MessageResponse>(It.IsAny<Message>())).Returns(messageResponse);

        IMessageService service = new MessageService(_messageRepositoryMock.Object, _userRepositoryMock.Object, _chatRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.AddUserReportAsync(userId, userReportAddRequest);

        // Assert
        Assert.IsType<MessageResponse>(result);
    }

    [Fact]
    public async Task GetAllChatMessagesAsync_ShouldReturnMessageResponses_WhenChatExists()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var chat = new Chat { Id = chatId };
        var messages = new List<Message> { new Message() };
        var messageResponses = new List<MessageResponse> { new MessageResponse() };

        _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
        _messageRepositoryMock.Setup(x => x.GetAllChatMessagesAsync(chat)).ReturnsAsync(messages);
        _mapperMock.Setup(x => x.Map<ICollection<MessageResponse>>(It.IsAny<ICollection<Message>>())).Returns(messageResponses);

        IMessageService service = new MessageService(_messageRepositoryMock.Object, _userRepositoryMock.Object, _chatRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetAllChatMessagesAsync(chatId);

        // Assert
        Assert.IsType<List<MessageResponse>>(result);
    }

    [Fact]
    public async Task GetReportedMessageAsync_ShouldReturnMessageResponses_WhenMessagesExist()
    {
        // Arrange
        var messages = new List<Message> { new Message() };
        var messageResponses = new List<MessageResponse> { new MessageResponse() };

        _messageRepositoryMock.Setup(x => x.GetReportedMessageAsync()).ReturnsAsync(messages);
        _mapperMock.Setup(x => x.Map<ICollection<MessageResponse>>(It.IsAny<ICollection<Message>>())).Returns(messageResponses);

        IMessageService service = new MessageService(_messageRepositoryMock.Object, _userRepositoryMock.Object, _chatRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetReportedMessageAsync();

        // Assert
        Assert.IsType<List<MessageResponse>>(result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldSaveChanges_WhenMessageExists()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var message = new Message { Id = messageId, UserReports = new List<User>() };

        _messageRepositoryMock.Setup(x => x.GetByIdAsync(messageId)).ReturnsAsync(message);
        _messageRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IMessageService service = new MessageService(_messageRepositoryMock.Object, _userRepositoryMock.Object, _chatRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.RemoveAsync(messageId);

        // Assert
        _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveUserReportsAsync_ShouldSaveChanges_WhenMessageExists()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var message = new Message { Id = messageId, UserReports = new List<User>() };

        _messageRepositoryMock.Setup(x => x.GetByIdAsync(messageId)).ReturnsAsync(message);
        _messageRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IMessageService service = new MessageService(_messageRepositoryMock.Object, _userRepositoryMock.Object, _chatRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.RemoveUserReportsAsync(messageId);

        // Assert
        _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
