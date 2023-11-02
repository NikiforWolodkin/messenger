using MessengerApiDomain.Models;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Helpers
{
    public class UserResponseDecorator : UserResponse
    {
        public bool IsSelected { get; set; } = false;

        private UserResponseDecorator(UserResponse user)
        {
            Id = user.Id;
            Name = user.Name;
            DisplayName = user.DisplayName;
            AvatarUrl = user.AvatarUrl;
        }

        public static UserResponseDecorator ToDecorator(UserResponse user)
        {
            return new UserResponseDecorator(user);
        }

        public static ICollection<UserResponseDecorator> ToDecorator(ICollection<UserResponse> users) 
        {
            var decorators = new List<UserResponseDecorator>();

            foreach (var user in users)
            {
                decorators.Add(new UserResponseDecorator(user));
            }

            return decorators;
        }
    }
}
