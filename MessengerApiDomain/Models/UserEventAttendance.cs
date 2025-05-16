namespace MessengerApiDomain.Models;

public class UserEventAttendance
{
    public virtual Guid Id { get; set; }
    public virtual User User { get; set; }
    public virtual CalendarEvent Event { get; set; }
    public virtual bool? IsAttending { get; set; }
}
