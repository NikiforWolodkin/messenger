using MessengerApiDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class ChatResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("lastMessage")]
        public string? LastMessage { get; set; }
        [JsonPropertyName("lastMessageTime")]
        public DateTime? LastMessageTime { get; set; }
        [JsonPropertyName("type")]
        public ChatType? Type { get; set; }
        [JsonPropertyName("isBlacklisted")]
        public bool? IsBlacklisted { get; set; }
    }
}
