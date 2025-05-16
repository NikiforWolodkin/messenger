using System.Text.Json.Serialization;

namespace MessengerModels.Models;

public class CalendarEventUserResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }
    [JsonPropertyName("avatarUrl")]
    public string AvatarUrl { get; set; }
    [JsonPropertyName("isAttending")]
    public bool? IsAttending { get; set; }
    [JsonPropertyName("isOrganizer")]
    public bool IsOrganizer { get; set; }
}
