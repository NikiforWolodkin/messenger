using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Exceptions;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;

namespace MessengerApiServices.Services;

public class CalendarEventService : ICalendarEventService
{
    private readonly ICalendarEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CalendarEventService(ICalendarEventRepository eventRepository, 
                                IUserRepository userRepository, 
                                IMapper mapper)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<CalendarEventResponse> AddAsync(Guid organizerId, CalendarEventAddRequest addCalendarEventRequest)
    {
        var organizer = await _userRepository.GetByIdAsync(organizerId)
            ?? throw new NotFoundException("User not found.");

        var participants = new List<User>();
        foreach (var userId in addCalendarEventRequest.ParticipantIds)
        {
            var participant = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            participants.Add(participant);
        }

        if (!participants.Contains(organizer))
        {
            participants.Add(organizer);
        }

        var calendarEvent = new CalendarEvent
        {
            Name = addCalendarEventRequest.Name,
            Agenda = addCalendarEventRequest.Agenda,
            StartTime = addCalendarEventRequest.StartTime,
            DurationInMinutes = addCalendarEventRequest.DurationInMinutes,
            Organizer = organizer,
            Participants = participants,
        };

        await _eventRepository.AddAsync(calendarEvent);

        return ToModel(calendarEvent);
    }

    public async Task<ICollection<CalendarEventResponse>> GetUserEventsForDayAsync(Guid userId, DateTime day)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var events = await _eventRepository.GetUserEventsForDayAsync(user, day);

        return ToModel(events);
    }

    public async Task RemoveAsync(Guid id, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var calendarEvent = await _eventRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Event not found.");

        if (calendarEvent.Organizer != user)
        {
            throw new UnauthorizedException("You are not the organizer and can not delete this event.");
        }

        await _eventRepository.RemoveAsync(calendarEvent);
    }

    public async Task<CalendarEventResponse> SetUserAttendanceAsync(Guid id, Guid userId, bool isAttending)
    {
        var calendarEvent = await _eventRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Event not found.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (!calendarEvent.Participants.Contains(user))
        {
            throw new NotFoundException("User is not a participant in the event.");
        }

        var userAttendance = calendarEvent.Attendance
            .FirstOrDefault(attendance => attendance.User == user);

        if (userAttendance is not null)
        {
            userAttendance.IsAttending = isAttending;
        }
        else
        {
            userAttendance = new UserEventAttendance
            {
                User = user,
                Event = calendarEvent,
                IsAttending = isAttending,
            };

            await _eventRepository.AddUserAttendanceAsync(userAttendance);

            calendarEvent.Attendance.Add(userAttendance);
        }

        await _eventRepository.SaveChangesAsync();

        return ToModel(calendarEvent);
    }

    private ICollection<CalendarEventResponse> ToModel(ICollection<CalendarEvent> events) 
    {
        var eventResponses = new List<CalendarEventResponse>();

        foreach (var calendarEvent in events)
        {
            eventResponses.Add(ToModel(calendarEvent));
        }

        return eventResponses;
    }

    private CalendarEventResponse ToModel(CalendarEvent calendarEvent)
    {
        var eventResponse = _mapper.Map<CalendarEventResponse>(calendarEvent);

        eventResponse.Participants = [];
        foreach (var participant in calendarEvent.Participants)
        {
            var participantResponse = _mapper.Map<CalendarEventUserResponse>(participant);

            participantResponse.IsOrganizer = participant == calendarEvent.Organizer;
            participantResponse.IsAttending = calendarEvent.Attendance
                .FirstOrDefault(attendance => attendance.User == participant)?
                .IsAttending ?? null;

            eventResponse.Participants.Add(participantResponse);
        }

        return eventResponse;
    }
}
