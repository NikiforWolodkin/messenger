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

            CreateMap<Message, MessageResponse>()
                .ForMember(destination => destination.AuthorName, config => 
                    config.MapFrom(source => source.Author.DisplayName))
                .ForMember(destination => destination.AuthorId, config =>
                    config.MapFrom(source => source.Author.Id))
                .ForMember(destination => destination.AuthorAvatarUrl, config =>
                    config.MapFrom(source => source.Author.AvatarUrl));
        }
    }
}
