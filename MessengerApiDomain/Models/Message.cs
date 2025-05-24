using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public virtual User Author { get; set; }
        public virtual Chat Chat { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime SendTime { get; set; }
        public virtual ICollection<User> UserReports { get; set; }
        public virtual ICollection<User> Likes { get; set; }
        public DateTime? SelfDeletionDeadline { get; set; }
    }
}
