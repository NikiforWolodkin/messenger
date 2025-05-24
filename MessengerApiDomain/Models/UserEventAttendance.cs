namespace MessengerApiDomain.Models;

public class UserEventAttendance
{
    public Guid Id { get; set; }
    public virtual User User { get; set; }
    public virtual CalendarEvent Event { get; set; }
    public bool? IsAttending { get; set; }
}
