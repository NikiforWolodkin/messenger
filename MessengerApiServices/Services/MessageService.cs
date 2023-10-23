using AutoMapper;
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
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, 
                              IChatRepository chatRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        async Task<MessageResponse> IMessageService.AddAsync(Guid userId, MessageAddRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var chat = await _chatRepository.GetByIdAsync(request.ChatId);

            var message = new Message
            {
                Author = user,
                Chat = chat,
                SendTime = DateTime.Now,
                Text = request.Text,
            };

            await _messageRepository.AddAsync(message);

            return _mapper.Map<MessageResponse>(message);
        }

        async Task<ICollection<MessageResponse>> IMessageService.GetAllChatMessagesAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            var messages = await _messageRepository.GetAllChatMessagesAsync(chat);
            
            return _mapper.Map<ICollection<MessageResponse>>(messages);
        }
    }
}
