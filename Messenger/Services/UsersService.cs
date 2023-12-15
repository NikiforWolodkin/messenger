using Messenger.Exceptions;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task AddToBlacklistAsync(Guid userId)
        {
            var request = new BlacklistAddRequest
            {
                UserId = userId,
            };

            var result = await Api.PostAsync<BlacklistAddRequest>("profile/blacklist", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }
        }

        public static async Task RemoveFromBlacklistAsync(Guid userId)
        {
            var result = await Api.DeleteAsync($"profile/blacklist/{userId}");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }
        }

        public static async Task<UserResponse> GetProfileAsync()
        {
            var result = await Api.GetAsync<UserResponse>("profile");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task<ICollection<UserResponse>> SearchAsync(string? search = null)
        {
            var result = await Api.GetAsync<ICollection<UserResponse>>(search == null ? "users" : $"users?search={search}");
            
            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task<ICollection<UserResponse>> SearchUsersWithoutConversationAsync(string? search = null)
        {
            var result = await Api.GetAsync<ICollection<UserResponse>>(search == null 
                ? "users?filterUsersWithConversation=true"
                : $"users?search={search}&filterUsersWithConversation=true");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task BanAndDeleteMessagesAsync(Guid messageId)
        {
            var result = await Api.DeleteAsync($"users/{messageId}");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }
        }
    }
}
