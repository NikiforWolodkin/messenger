using MessengerApiDomain.Models;

namespace MessengerApiDomain.RepositoryInterfaces;

public interface ICalendarEventRepository
{
    Task<CalendarEvent?> GetByIdAsync(Guid id);
    Task<ICollection<CalendarEvent>> GetUserEventsForDayAsync(User user, DateTime day);
    Task<ICollection<CalendarEvent>> GetEventsToNotifyParticipantsAsync(TimeSpan maximumTimeBeforeEvent);
    Task AddAsync(CalendarEvent calendarEvent);
    Task AddUserAttendanceAsync(UserEventAttendance userAttendance);
    Task RemoveAsync(CalendarEvent calendarEvent);
    Task SaveChangesAsync();
}
