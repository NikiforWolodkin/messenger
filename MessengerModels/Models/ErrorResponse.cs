using System.Text.Json.Serialization;

namespace MessengerModels.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public ErrorResponse()
        {    
        }

        public ErrorResponse(string message)
        {
            Message = message;
        }
    }
}
