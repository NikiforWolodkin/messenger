using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class UserResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; }
        [JsonPropertyName("isBlacklisted")]
        public bool? IsBlacklisted { get; set; }
        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }
        [JsonPropertyName("isBanned")]
        public bool IsBanned { get; set; }
    }
}
