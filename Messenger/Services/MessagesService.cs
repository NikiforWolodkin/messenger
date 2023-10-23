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
    public class MessagesService
    {
        public static async Task<ICollection<MessageResponse>> GetAllAsync(Guid chatId)
        {
            var result = await Api.GetAsync<ICollection<MessageResponse>>($"messages/{chatId}");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }

        public static async Task<MessageResponse> AddAsync(Guid chatId, string text)
        {
            var request = new MessageAddRequest
            {
                ChatId = chatId,
                Text = text,
            };

            var result = await Api.PostAsync<MessageResponse, MessageAddRequest>("messages", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }
    }
}
