using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class GroupAddRequest
    {
        public ICollection<Guid> ParticipantIds { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
