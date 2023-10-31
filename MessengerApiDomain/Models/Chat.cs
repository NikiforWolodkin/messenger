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
        public virtual Guid Id { get; set; }
        public virtual ICollection<User> Participants { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ChatType ChatType { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? ImageUrl { get; set; }
    }
}
