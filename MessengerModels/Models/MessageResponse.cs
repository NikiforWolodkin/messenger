using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class MessageResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("authorId")]
        public Guid AuthorId { get; set; }
        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; }
        [JsonPropertyName("authorAvatarUrl")]
        public string AuthorAvatarUrl {  get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
        [JsonPropertyName("sendTime")]
        public DateTime SendTime { get; set; }
    }
}
