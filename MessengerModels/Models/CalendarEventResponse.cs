using System.Text.Json.Serialization;

namespace MessengerModels.Models;

public class CalendarEventResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }
    [JsonPropertyName("durationInMinutes")]
    public int DurationInMinutes { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("agenda")]
    public string? Agenda { get; set; }
    [JsonPropertyName("participants")]
    public ICollection<CalendarEventUserResponse> Participants { get; set; }
}
