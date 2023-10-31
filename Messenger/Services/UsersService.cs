using Messenger.Exceptions;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Services
{
    public static class UsersService
    {
        public static async Task<UserResponse> UpdateProfileAsync(string displayName, string avatarUrl)
        {
            var request = new ProfileUpdateRequest
            {
                DisplayName = displayName,
                AvatarUrl = avatarUrl,
            };

            var result = await Api.PutAsync<UserResponse, ProfileUpdateRequest>("profile", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }

        public static async Task<UserResponse> GetProfileAsync()
        {
            var result = await Api.GetAsync<UserResponse>("profile");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }

        public static async Task<ICollection<UserResponse>> SearchAsync(string? search = null)
        {
            var result = await Api.GetAsync<ICollection<UserResponse>>(search == null ? "users" : $"users?search={search}");
            
            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }
    }
}
