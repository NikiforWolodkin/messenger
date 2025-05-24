using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Exceptions;
using MessengerApiServices.Interfaces;
using MessengerApiServices.Services;
using MessengerModels.Models;
using Moq;
using Xunit;

namespace MessengerApiServices.Tests;

public class ChatServiceTests
{
    private readonly Mock<IChatRepository> _chatRepositoryMock = new Mock<IChatRepository>();
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

    [Fact]
    public async Task AddConversationAsync_ShouldCallAddAsync_WhenUsersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestUserId = Guid.NewGuid();
        var user = new User { Id = userId };
        var requestUser = new User { Id = requestUserId };
        var chatResponse = new ChatResponse();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(requestUserId)).ReturnsAsync(requestUser);
        _chatRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Chat>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<ChatResponse>(It.IsAny<Chat>())).Returns(chatResponse);

        IChatService service = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _mapperMock.Object);

        // Act
        var result = await service.AddConversationAsync(userId, new ConversationAddRequest { userId = requestUserId });

        // Assert
        _chatRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Chat>()), Times.Once);
    }

    [Fact]
    public async Task AddGroupAsync_ShouldCallAddAsync_WhenUsersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var user = new User { Id = userId };
        var participant = new User { Id = participantId };
        var chatResponse = new ChatResponse();
        var groupAddRequest = new GroupAddRequest
        {
            Name = "Test Group",
            ImageUrl = "https://test.com/image.jpg",
            ParticipantIds = new List<Guid> { participantId }
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(participantId)).ReturnsAsync(participant);
        _chatRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Chat>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<ChatResponse>(It.IsAny<Chat>())).Returns(chatResponse);

        IChatService service = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _mapperMock.Object);

        // Act
        var result = await service.AddGroupAsync(userId, groupAddRequest);

        // Assert
        _chatRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Chat>()), Times.Once);
    }

    [Fact]
    public async Task GetChatUsers_ShouldReturnUserResponses_WhenChatExists()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var chat = new Chat { Id = chatId, Participants = new List<User> { user } };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(It.IsAny<ICollection<User>>())).Returns(userResponses);

        IChatService service = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetChatUsers(chatId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task GetUserChatsAsync_ShouldReturnChatResponses_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var chats = new List<Chat> { new Chat { ChatType = MessengerApiDomain.Enums.ChatType.Group, Messages = new List<Message>(), Participants = new List<User>() } };
        var chatResponses = new List<ChatResponse> { new ChatResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _chatRepositoryMock.Setup(x => x.GetUserChatsAsync(user)).ReturnsAsync(chats);
        // Assuming ToModelAsync is a method in your service that maps a collection of Chat to ChatResponse
        _mapperMock.Setup(x => x.Map<ICollection<ChatResponse>>(It.IsAny<ICollection<Chat>>())).Returns(chatResponses);

        IChatService service = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetUserChatsAsync(userId);

        // Assert
        Assert.IsType<List<ChatResponse>>(result);
    }

    [Fact]
    public async Task GetChatUsers_ShouldThrowNotFoundException_WhenChatDoesNotExist()
    {
        // Arrange
        var chatId = Guid.NewGuid();

        _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync((Chat)null);

        IChatService service = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _mapperMock.Object);

        // Act and Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetChatUsers(chatId));
    }
}
