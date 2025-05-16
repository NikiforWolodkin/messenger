using MessengerApiDomain.Models;

namespace MessengerApiDomain.RepositoryInterfaces;

public interface ICalendarEventRepository
{
    public Task<CalendarEvent?> GetByIdAsync(Guid id);
    public Task<ICollection<CalendarEvent>> GetUserEventsForDayAsync(User user, DateTime day);
    public Task AddAsync(CalendarEvent calendarEvent);
    public Task AddUserAttendanceAsync(UserEventAttendance userAttendance);
    public Task RemoveAsync(CalendarEvent calendarEvent);
    Task SaveChangesAsync();
}
