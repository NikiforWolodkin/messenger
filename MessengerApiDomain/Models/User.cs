using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.Models
{
    public class User
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string AvatarUrl { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual bool IsBanned { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> ReportedMessages { get; set; }
        public virtual ICollection<User> Blacklist { get; set; }
    }
}
