using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiInfrasctructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace MessengerApiInfrasctructure.Repositories;

public class CalendarEventRepository : ICalendarEventRepository
{
    private readonly DataContext _context;

    public CalendarEventRepository(DataContext context)
    {
        _context = context;
    }

    public async Task AddUserAttendanceAsync(UserEventAttendance userAttendance)
    {
        await _context.EventAttendance.AddAsync(userAttendance);

        await _context.SaveChangesAsync();
    }

    async Task ICalendarEventRepository.AddAsync(CalendarEvent calendarEvent)
    {
        await _context.CalendarEvents.AddAsync(calendarEvent);

        await _context.SaveChangesAsync();
    }

    async Task<CalendarEvent?> ICalendarEventRepository.GetByIdAsync(Guid id)
    {
        return await _context.CalendarEvents.FindAsync(id);
    }

    async Task<ICollection<CalendarEvent>> ICalendarEventRepository.GetUserEventsForDayAsync(User user, DateTime day)
    {
        return await _context.CalendarEvents
            .Where(calendarEvent => calendarEvent.Participants.Contains(user))
            .Where(calendarEvent => calendarEvent.StartTime.Date == day.Date)
            .OrderBy(calendarEvent => calendarEvent.StartTime)
            .ToListAsync();
    }

    async Task ICalendarEventRepository.RemoveAsync(CalendarEvent calendarEvent)
    {
        _context.CalendarEvents.Remove(calendarEvent);

        await _context.SaveChangesAsync();
    }

    async Task ICalendarEventRepository.SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
