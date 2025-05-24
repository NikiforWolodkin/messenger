using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string PasswordHash { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> ReportedMessages { get; set; }
        public virtual ICollection<Message> LikedMessages { get; set; }
        public virtual ICollection<User> Blacklist { get; set; }
        public virtual ICollection<CalendarEvent> Events { get; set; }
        public virtual ICollection<CalendarEvent> OrganizedEvents { get; set; }
    }
}
