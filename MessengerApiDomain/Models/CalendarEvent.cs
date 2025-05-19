namespace MessengerApiDomain.Models;

public class CalendarEvent
{
    public virtual Guid Id { get; set; }
    public virtual DateTime StartTime { get; set; }
    public virtual int DurationInMinutes { get; set; }
    public virtual string Name { get; set; }
    public virtual string? Agenda { get; set; }
    public virtual bool IsNotificationSent { get; set; }
    public virtual User Organizer { get; set; }
    public virtual ICollection<User> Participants { get; set; }
    public virtual ICollection<UserEventAttendance> Attendance { get; set; }
}
