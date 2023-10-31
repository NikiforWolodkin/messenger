using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class ProfileUpdateRequest
    {
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
