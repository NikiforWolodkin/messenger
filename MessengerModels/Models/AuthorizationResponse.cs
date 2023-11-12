using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class AuthorizationResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }
        [JsonPropertyName("authorizationToken")]
        public string AuthorizationToken { get; set; }
    }
}
