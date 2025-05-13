using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class MessageAddRequest
    {
        public Guid ChatId { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public int? MinutesBeforeSelfDeletion { get; set; }
    }
}
