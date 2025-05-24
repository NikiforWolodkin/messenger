namespace MessengerApiDomain.Models;

public class CalendarEvent
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Name { get; set; }
    public string? Agenda { get; set; }
    public bool IsNotificationSent { get; set; }
    public virtual User Organizer { get; set; }
    public virtual ICollection<User> Participants { get; set; }
    public virtual ICollection<UserEventAttendance> Attendance { get; set; }
}
