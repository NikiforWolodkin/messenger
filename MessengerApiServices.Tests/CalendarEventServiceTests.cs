using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Exceptions;
using MessengerApiServices.Services;
using MessengerModels.Models;
using Moq;
using Xunit;

namespace MessengerApiServices.Tests;

public class CalendarEventServiceTests
{
    private readonly Mock<ICalendarEventRepository> _eventRepositoryMock = new Mock<ICalendarEventRepository>();
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly CalendarEventService _service;

    public CalendarEventServiceTests()
    {
        _service = new CalendarEventService(
            _eventRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task AddAsync_ShouldReturnEventResponse_WhenValidRequest()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var organizer = new User { Id = organizerId };
        var participant = new User { Id = participantId };
        var request = new CalendarEventAddRequest
        {
            Name = "Test Event",
            Agenda = "Test Agenda",
            StartTime = DateTime.Now.AddHours(1),
            DurationInMinutes = 60,
            ParticipantIds = new List<Guid> { participantId }
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(organizerId)).ReturnsAsync(organizer);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(participantId)).ReturnsAsync(participant);
        _eventRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CalendarEvent>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<CalendarEventResponse>(It.IsAny<CalendarEvent>()))
            .Returns(new CalendarEventResponse());
        _mapperMock.Setup(x => x.Map<CalendarEventUserResponse>(It.IsAny<User>()))
            .Returns(new CalendarEventUserResponse());

        // Act
        var result = await _service.AddAsync(organizerId, request);

        // Assert
        Assert.NotNull(result);
        _eventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CalendarEvent>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var request = new CalendarEventAddRequest { ParticipantIds = new List<Guid>() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(organizerId)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddAsync(organizerId, request));
    }

    [Fact]
    public async Task GetEventsToNotifyParticipantsAsync_ShouldReturnEventsAndMarkThemAsNotified()
    {
        // Arrange
        var timeBeforeEvent = TimeSpan.FromMinutes(30);
        var events = new List<CalendarEvent>
        {
            new CalendarEvent { IsNotificationSent = false, Participants = [] }
        };
        var expectedResponses = new List<CalendarEventResponse> { new CalendarEventResponse() };

        _eventRepositoryMock.Setup(x => x.GetEventsToNotifyParticipantsAsync(timeBeforeEvent))
            .ReturnsAsync(events);
        _eventRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<CalendarEventResponse>(It.IsAny<CalendarEvent>()))
            .Returns(new CalendarEventResponse());
        _mapperMock.Setup(x => x.Map<CalendarEventUserResponse>(It.IsAny<User>()))
            .Returns(new CalendarEventUserResponse());

        // Act
        var result = await _service.GetEventsToNotifyParticipantsAsync(timeBeforeEvent);

        // Assert
        Assert.Single(result);
        Assert.True(events[0].IsNotificationSent);
        _eventRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserEventsForDayAsync_ShouldReturnEvents_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var day = DateTime.Today;
        var user = new User { Id = userId };
        var events = new List<CalendarEvent> { new CalendarEvent() { Participants = [] } };
        var expectedResponses = new List<CalendarEventResponse> { new CalendarEventResponse() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _eventRepositoryMock.Setup(x => x.GetUserEventsForDayAsync(user, day)).ReturnsAsync(events);
        _mapperMock.Setup(x => x.Map<CalendarEventResponse>(It.IsAny<CalendarEvent>()))
            .Returns(new CalendarEventResponse());
        _mapperMock.Setup(x => x.Map<CalendarEventUserResponse>(It.IsAny<User>()))
            .Returns(new CalendarEventUserResponse());

        // Act
        var result = await _service.GetUserEventsForDayAsync(userId, day);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserEventsForDayAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetUserEventsForDayAsync(userId, DateTime.Today));
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveEvent_WhenUserIsOrganizer()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var calendarEvent = new CalendarEvent { Organizer = user };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(calendarEvent);

        // Act
        await _service.RemoveAsync(eventId, userId);

        // Assert
        _eventRepositoryMock.Verify(x => x.RemoveAsync(calendarEvent), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldThrowUnauthorizedException_WhenUserIsNotOrganizer()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var user = new User { Id = userId };
        var organizer = new User { Id = otherUserId };
        var calendarEvent = new CalendarEvent { Organizer = organizer };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(calendarEvent);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.RemoveAsync(eventId, userId));
    }

    [Fact]
    public async Task SetUserAttendanceAsync_ShouldUpdateAttendance_WhenUserIsParticipant()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var calendarEvent = new CalendarEvent
        {
            Participants = new List<User> { user },
            Attendance = new List<UserEventAttendance>()
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(calendarEvent);
        _eventRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(x => x.Map<CalendarEventResponse>(It.IsAny<CalendarEvent>()))
            .Returns(new CalendarEventResponse());
        _mapperMock.Setup(x => x.Map<CalendarEventUserResponse>(It.IsAny<User>()))
            .Returns(new CalendarEventUserResponse());

        // Act
        var result = await _service.SetUserAttendanceAsync(eventId, userId, true);

        // Assert
        Assert.NotNull(result);
        Assert.Single(calendarEvent.Attendance);
        Assert.True(calendarEvent.Attendance.First().IsAttending);
    }

    [Fact]
    public async Task SetUserAttendanceAsync_ShouldThrowNotFoundException_WhenUserNotParticipant()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var calendarEvent = new CalendarEvent { Participants = new List<User>() };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(calendarEvent);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.SetUserAttendanceAsync(eventId, userId, true));
    }

    //[Fact]
    //public void ToModel_ShouldMapEventCorrectly()
    //{
    //    // Arrange
    //    var userId = Guid.NewGuid();
    //    var user = new User { Id = userId };
    //    var calendarEvent = new CalendarEvent
    //    {
    //        Organizer = user,
    //        Participants = new List<User> { user },
    //        Attendance = new List<UserEventAttendance>
    //        {
    //            new UserEventAttendance { User = user, IsAttending = true }
    //        }
    //    };

    //    _mapperMock.Setup(x => x.Map<CalendarEventResponse>(It.IsAny<CalendarEvent>()))
    //        .Returns(new CalendarEventResponse());
    //    _mapperMock.Setup(x => x.Map<CalendarEventUserResponse>(It.IsAny<User>()))
    //        .Returns(new CalendarEventUserResponse());

    //    // Act
    //    var result = _service.ToModel(calendarEvent);

    //    // Assert
    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public void ToModel_ShouldMapEventCollectionCorrectly()
    //{
    //    // Arrange
    //    var events = new List<CalendarEvent> { new CalendarEvent() };
    //    _mapperMock.Setup(x => x.Map<CalendarEventResponse>(It.IsAny<CalendarEvent>()))
    //        .Returns(new CalendarEventResponse());

    //    // Act
    //    var result = _service.ToModel(events);

    //    // Assert
    //    Assert.Single(result);
    //}
}