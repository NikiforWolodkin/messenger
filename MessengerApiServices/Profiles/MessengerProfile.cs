using AutoMapper;
using MessengerApiDomain.Models;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Profiles
{
    public class MessengerProfile : Profile
    {
        public MessengerProfile()
        {
            CreateMap<User, UserResponse>();
        }
    }
}
