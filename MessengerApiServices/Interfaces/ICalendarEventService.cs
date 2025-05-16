using MessengerModels.Models;

namespace MessengerApiServices.Interfaces;

public interface ICalendarEventService
{
    Task<ICollection<CalendarEventResponse>> GetUserEventsForDayAsync(Guid userId, DateTime day);
    Task<CalendarEventResponse> AddAsync(Guid organizerId, CalendarEventAddRequest addCalendarEventRequest);
    Task RemoveAsync(Guid id, Guid userId);
    Task<CalendarEventResponse> SetUserAttendanceAsync(Guid id, Guid userId, bool isAttending);
}
