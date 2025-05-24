using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Interfaces;
using MessengerApiServices.Services;
using MessengerModels.Models;
using Moq;
using Xunit;

namespace MessengerApiServices.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

    [Fact]
    public async Task GetAllUsersWithoutConversationAsync_ShouldReturnUserResponses_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var users = new List<User> { new User() };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetAllUsersWithoutConversationAsync(user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(It.IsAny<ICollection<User>>())).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetAllUsersWithoutConversationAsync(userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task SearchUsersWithoutConversationAsync_ShouldReturnUserResponses_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var users = new List<User> { new User() };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.SearchUsersWithoutConversationAsync(It.IsAny<string>(), user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(It.IsAny<ICollection<User>>())).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.SearchUsersWithoutConversationAsync("test", userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUserResponses_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Blacklist = new List<User>() };
        var users = new List<User> { new User() };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetAllAsync(user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(It.IsAny<ICollection<User>>())).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetAllAsync(userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnUserResponses_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Blacklist = new List<User>() };
        var users = new List<User> { new User() };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.SearchAsync(It.IsAny<string>(), user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(It.IsAny<ICollection<User>>())).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.SearchAsync("test", userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnUserResponse_WhenUserIsAdded()
    {
        // Arrange
        var userResponse = new UserResponse();
        var userSignUpRequest = new UserSignUpRequest { Name = "Test", Password = "password" };

        _userRepositoryMock.Setup(x => x.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.AddAsync(userSignUpRequest);

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var user = new User { Name = "Test" };
        var userResponse = new UserResponse();

        _userRepositoryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserResponse?>(It.IsAny<User>())).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetByNameAsync("Test");

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.UserExistsAsync("Test");

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(x => x.UserExistsAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        // Arrange
        var user = new User { Name = "Test", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
        var userLogInRequest = new UserLogInRequest { Name = "Test", Password = "password" };

        _userRepositoryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.VerifyPasswordAsync(userLogInRequest);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(x => x.GetByNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var userResponse = new UserResponse();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserResponse?>(It.IsAny<User>())).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetByIdAsync(userId);

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var userResponse = new UserResponse();
        var profileUpdateRequest = new ProfileUpdateRequest { DisplayName = "Test", AvatarUrl = "https://test.com/avatar.jpg" };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.UpdateProfileAsync(userId, profileUpdateRequest);

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task AddToBlacklistAsync_ShouldSaveChanges_WhenUserAndBlacklistedUserExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blacklistedUserId = Guid.NewGuid();
        var user = new User { Id = userId, Blacklist = new List<User>() };
        var blacklistedUser = new User { Id = blacklistedUserId };
        var blacklistAddRequest = new BlacklistAddRequest { UserId = blacklistedUserId };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(blacklistedUserId)).ReturnsAsync(blacklistedUser);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.AddToBlacklistAsync(userId, blacklistAddRequest);

        // Assert
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveFromBlacklistAsync_ShouldSaveChanges_WhenUserAndBlacklistedUserExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blacklistedUserId = Guid.NewGuid();
        var blacklistedUser = new User { Id = blacklistedUserId };
        var user = new User { Id = userId, Blacklist = new List<User> { blacklistedUser } };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(blacklistedUserId)).ReturnsAsync(blacklistedUser);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.RemoveFromBlacklistAsync(userId, blacklistedUserId);

        // Assert
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task IsBlacklistedAsync_ShouldReturnTrue_WhenUserIsBlacklisted()
    {
        // Arrange
        var firstUserId = Guid.NewGuid();
        var secondUserId = Guid.NewGuid();
        var secondUser = new User { Id = secondUserId, Blacklist = new List<User>() };
        var firstUser = new User { Id = firstUserId, Blacklist = new List<User> { secondUser } };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(firstUserId)).ReturnsAsync(firstUser);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(secondUserId)).ReturnsAsync(secondUser);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.IsBlacklistedAsync(firstUserId, secondUserId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task BanUserAndDeleteAllMessagesAsync_ShouldSaveChanges_WhenMessageExists()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = new Message { Id = messageId, Author = new User { Id = userId, Messages = new List<Message>() } };

        _messageRepositoryMock.Setup(x => x.GetByIdAsync(messageId)).ReturnsAsync(message);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(message.Author);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.BanUserAndDeleteAllMessagesAsync(messageId);

        // Assert
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
