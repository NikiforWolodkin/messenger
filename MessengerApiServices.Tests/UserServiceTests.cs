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

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>();
    private readonly Mock<IOperationLogRepository> _operationLogRepositoryMock = new Mock<IOperationLogRepository>();
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
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(users)).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

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
        var search = "test";
        var user = new User { Id = userId };
        var users = new List<User> { new User() };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.SearchUsersWithoutConversationAsync(search, user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(users)).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.SearchUsersWithoutConversationAsync(search, userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUserResponsesWithBlacklistStatus_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Blacklist = new List<User>() };
        var users = new List<User> { new User { Id = Guid.NewGuid() } };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetAllAsync(user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(users)).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetAllAsync(userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
        Assert.All(result, response => Assert.False(response.IsBlacklisted ?? false));
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnUserResponsesWithBlacklistStatus_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var search = "test";
        var user = new User { Id = userId, Blacklist = new List<User>() };
        var users = new List<User> { new User { Id = Guid.NewGuid() } };
        var userResponses = new List<UserResponse> { new UserResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.SearchAsync(search, user)).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<ICollection<UserResponse>>(users)).Returns(userResponses);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.SearchAsync(search, userId);

        // Assert
        Assert.IsType<List<UserResponse>>(result);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnUserResponse_WhenValidRequest()
    {
        // Arrange
        var request = new UserSignUpRequest { Name = "test", Password = "password" };
        var user = new User { Name = request.Name };
        var userResponse = new UserResponse();

        _userRepositoryMock.Setup(x => x.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.AddAsync(request);

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var name = "test";
        var user = new User { Name = name };
        var userResponse = new UserResponse();

        _userRepositoryMock.Setup(x => x.GetByNameAsync(name)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserResponse?>(user)).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.GetByNameAsync(name);

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var name = "test";

        _userRepositoryMock.Setup(x => x.UserExistsAsync(name)).ReturnsAsync(true);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.UserExistsAsync(name);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var request = new UserLogInRequest { Name = "test", Password = "password" };
        var user = new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password) };

        _userRepositoryMock.Setup(x => x.GetByNameAsync(request.Name)).ReturnsAsync(user);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.VerifyPasswordAsync(request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var userResponse = new UserResponse();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserResponse?>(user)).Returns(userResponse);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

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
        var request = new ProfileUpdateRequest { DisplayName = "new name", AvatarUrl = "http://test.com/image.jpg" };
        var user = new User { Id = userId };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(new UserResponse());

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.UpdateProfileAsync(userId, request);

        // Assert
        Assert.IsType<UserResponse>(result);
    }

    [Fact]
    public async Task AddToBlacklistAsync_ShouldSaveChanges_WhenUsersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blacklistedUserId = Guid.NewGuid();
        var request = new BlacklistAddRequest { UserId = blacklistedUserId };
        var user = new User { Id = userId, Blacklist = new List<User>() };
        var blacklistedUser = new User { Id = blacklistedUserId };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(blacklistedUserId)).ReturnsAsync(blacklistedUser);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.AddToBlacklistAsync(userId, request);

        // Assert
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveFromBlacklistAsync_ShouldSaveChanges_WhenUsersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blacklistedUserId = Guid.NewGuid();
        var blacklistedUser = new User { Id = blacklistedUserId };
        var user = new User { Id = userId, Blacklist = new List<User> { blacklistedUser } };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(blacklistedUserId)).ReturnsAsync(blacklistedUser);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

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
        var firstUser = new User { Id = firstUserId, Blacklist = new List<User>() };
        var secondUser = new User { Id = secondUserId };
        firstUser.Blacklist.Add(secondUser);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(firstUserId)).ReturnsAsync(firstUser);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(secondUserId)).ReturnsAsync(secondUser);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        var result = await service.IsBlacklistedAsync(firstUserId, secondUserId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task BanUserAndDeleteAllMessagesAsync_ShouldSaveChanges_WhenUsersExist()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var message = new Message { Id = messageId, Author = new User { Id = authorId }, UserReports = [] };
        var admin = new User { Id = adminId };
        var author = new User { Id = authorId, Messages = new List<Message> { message } };

        _messageRepositoryMock.Setup(x => x.GetByIdAsync(messageId)).ReturnsAsync(message);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(authorId)).ReturnsAsync(author);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId)).ReturnsAsync(admin);
        _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _operationLogRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OperationLog>())).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.BanUserAndDeleteAllMessagesAsync(messageId, adminId);

        // Assert
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.True(author.IsBanned);
    }

    [Fact]
    public async Task RecordUserOperationAsync_ShouldAddLog_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "test", DisplayName = "Test User" };
        var time = DateTime.Now;
        var operationName = "TestOperation";
        var operationDescription = "Test description";

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _operationLogRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OperationLog>())).Returns(Task.CompletedTask);

        IUserService service = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object,
            _operationLogRepositoryMock.Object, _mapperMock.Object);

        // Act
        await service.RecordUserOperationAsync(userId, time, operationName, operationDescription);

        // Assert
        _operationLogRepositoryMock.Verify(x => x.AddAsync(It.Is<OperationLog>(log =>
            log.UserId == userId &&
            log.UserName == user.Name &&
            log.UserDisplayName == user.DisplayName &&
            log.OperationName == operationName &&
            log.OperationDescription == operationDescription &&
            log.Time == time
        )), Times.Once);
    }
}