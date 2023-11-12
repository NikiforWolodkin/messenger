using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.Models
{
    public class Message
    {
        public virtual Guid Id { get; set; }
        public virtual User Author { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual string? Text { get; set; }
        public virtual string? ImageUrl { get; set; }
        public virtual DateTime SendTime { get; set; }
        public virtual ICollection<User> UserReports { get; set; }
        public virtual bool IsBanned { get; set; }
    }
}
