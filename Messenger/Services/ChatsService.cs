using Messenger.Exceptions;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public static async Task<ChatResponse> AddConversationAsync(Guid userId)
        {
            var request = new ConversationAddRequest
            {
                userId = userId
            };

            var result = await Api.PostAsync<ChatResponse, ConversationAddRequest>("chats/conversations", request);

            if (!result.IsSuccessful)
            {
                MessageBox.Show(result.Error.Message, "Error");
            }

            return result.Response;
        }

        public static async Task<ChatResponse> AddGroupAsync(string name, string imageUrl, ICollection<Guid> participantIds)
        {
            var request = new GroupAddRequest
            {
                Name = name,
                ImageUrl = imageUrl,
                ParticipantIds = participantIds,
            };

            var result = await Api.PostAsync<ChatResponse, GroupAddRequest>("chats/groups", request);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error);
            }

            return result.Response;
        }
    }
}
