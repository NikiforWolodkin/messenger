using MessengerApiDomain.Enums;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        async Task<ChatResponse> IChatService.AddConversationAsync(Guid userId, ConversationAddRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var requestUser = await _userRepository.GetByIdAsync(request.userId);

            var chat = new Chat
            {
                ChatType = ChatType.Conversation,
                Participants = new List<User> { user, requestUser },
            };

            await _chatRepository.AddAsync(chat);

            return ToModel(chat, userId);
        }

        async Task<ICollection<ChatResponse>> IChatService.GetUserChatsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var chats = await _chatRepository.GetUserChatsAsync(user);

            return ToModel(chats, userId);
        }

        private ChatResponse ToModel(Chat chat, Guid userId)
        {
            if (chat.ChatType == ChatType.Conversation) 
            {
                var response = new ChatResponse()
                {
                    Id = chat.Id,
                    Name = chat.Participants.Where(chat => chat.Id != userId).Single().Name,
                };

                return response;
            }

            return null;
        }

        private ICollection<ChatResponse> ToModel(ICollection<Chat> chats, Guid userId)
        {
            var response = new List<ChatResponse>();

            foreach (var chat in chats)
            {
                var model = ToModel(chat, userId);

                response.Add(model);
            }

            return response;
        }
    }
}
