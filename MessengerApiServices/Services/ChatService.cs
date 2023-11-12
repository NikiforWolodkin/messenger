using AutoMapper;
using MessengerApiDomain.Enums;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Exceptions;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository, 
                           IUserService userService, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        async Task<ChatResponse> IChatService.AddConversationAsync(Guid userId, ConversationAddRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId) 
                ?? throw new NotFoundException("User not found.");
            
            var requestUser = await _userRepository.GetByIdAsync(request.userId) 
                ?? throw new NotFoundException("User not found.");

            var chat = new Chat
            {
                ChatType = ChatType.Conversation,
                Participants = new List<User> { user, requestUser },
            };

            await _chatRepository.AddAsync(chat);

            return await ToModelAsync(chat, userId);
        }

        async Task<ChatResponse> IChatService.AddGroupAsync(Guid userId, GroupAddRequest request)
        {
            var participants = new List<User>();

            try
            {
                participants.Add(await _userRepository.GetByIdAsync(userId));
                foreach (var id in request.ParticipantIds)
                {
                    participants.Add(await _userRepository.GetByIdAsync(id));
                }
            }
            catch
            {
                throw new NotFoundException("User not found.");
            }

            var chat = new Chat
            {
                ChatType = ChatType.Group,
                Name = request.Name,
                ImageUrl = request.ImageUrl,
                Participants = participants,
            };

            await _chatRepository.AddAsync(chat);

            return await ToModelAsync(chat, userId);
        }

        async Task<ICollection<UserResponse>> IChatService.GetChatUsers(Guid id)
        {
            var chat = await _chatRepository.GetByIdAsync(id) 
                ?? throw new NotFoundException("Chat not found.");

            var users = chat.Participants;

            return _mapper.Map<ICollection<UserResponse>>(users);
        }

        async Task<ICollection<ChatResponse>> IChatService.GetUserChatsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            var chats = await _chatRepository.GetUserChatsAsync(user)
                ?? throw new NotFoundException("Chat not found.");

            return await ToModelAsync(chats, userId);
        }

        // TODO: Move to profile
        private async Task<ChatResponse> ToModelAsync(Chat chat, Guid userId)
        {
            if (chat.ChatType == ChatType.Conversation) 
            {
                var lastMessage = chat.Messages?
                    .OrderByDescending(message => message.SendTime)
                    .FirstOrDefault();

                var response = new ChatResponse()
                {
                    Id = chat.Id,
                    Name = chat.Participants.Where(chat => chat.Id != userId).Single().DisplayName,
                    ImageUrl = chat.Participants.Where(chat => chat.Id != userId).Single().AvatarUrl,
                    LastMessage = lastMessage?.Text,
                    LastMessageTime = lastMessage?.SendTime,
                    Type = chat.ChatType,
                    // TODO: Check if this is slow
                    IsBlacklisted = await _userService.IsBlacklistedAsync(chat.Participants.First().Id, chat.Participants.Last().Id),
                };

                return response;
            }

            if (chat.ChatType == ChatType.Group)
            {
                var lastMessage = chat.Messages?
                    .OrderByDescending(message => message.SendTime)
                    .FirstOrDefault();

                var response = new ChatResponse()
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    ImageUrl = chat.ImageUrl,
                    LastMessage = lastMessage?.Text,
                    LastMessageTime = lastMessage?.SendTime,
                    Type = chat.ChatType,
                };

                return response;
            }

            return null;
        }

        private async Task<ICollection<ChatResponse>> ToModelAsync(ICollection<Chat> chats, Guid userId)
        {
            var response = new List<ChatResponse>();

            foreach (var chat in chats)
            {
                var model = await ToModelAsync(chat, userId);

                response.Add(model);
            }

            return response;
        }
    }
}
