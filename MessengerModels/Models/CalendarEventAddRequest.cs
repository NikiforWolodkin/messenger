namespace MessengerModels.Models;

public class CalendarEventAddRequest
{
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Name { get; set; }
    public string? Agenda { get; set; }
    public ICollection<Guid> ParticipantIds { get; set; }
}
