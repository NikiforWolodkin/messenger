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
    public class MessagesService
    {
        public static async Task<ICollection<MessageResponse>> GetAllAsync(Guid chatId)
        {
            var result = await Api.GetAsync<ICollection<MessageResponse>>($"messages/{chatId}");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task<ICollection<MessageResponse>> GetReportedMessagesAsync()
        {
            var result = await Api.GetAsync<ICollection<MessageResponse>>($"messages/reported");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
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
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task<MessageResponse> AddImageMessageAsync(Guid chatId, string imageUrl)
        {
            var request = new MessageAddRequest
            {
                ChatId = chatId,
                ImageUrl = imageUrl,
            };

            var result = await Api.PostAsync<MessageResponse, MessageAddRequest>("messages", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task<MessageResponse> AddReportAsync(Guid messageId)
        {
            var request = new UserReportAddRequest
            {
                MessageId = messageId,
            };

            var result = await Api.PostAsync<MessageResponse, UserReportAddRequest>("messages/reports", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }

        public static async Task DeleteReportAsync(Guid messageId)
        {
            var result = await Api.DeleteAsync($"messages/reports/{messageId}");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }
        }

        public static async Task DeleteMessageAsync(Guid messageId)
        {
            var result = await Api.DeleteAsync($"messages/{messageId}");

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }
        }
    }
}
