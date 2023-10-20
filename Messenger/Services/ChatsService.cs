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
    public static class ChatsService
    {
        public static async Task<ICollection<ChatResponse>> GetAllAsync()
        {
            var result = await Api.GetAsync<ICollection<ChatResponse>>("chats");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }

        public static async Task<ChatResponse> AddAsync(Guid userId)
        {
            var request = new ConversationAddRequest
            {
                userId = userId
            };

            var result = await Api.PostAsync<ChatResponse, ConversationAddRequest>("chats", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }
    }
}
