using MessengerApiDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public virtual ICollection<User> Participants { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public ChatType ChatType { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}
