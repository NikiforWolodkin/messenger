﻿using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Exceptions;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;

namespace MessengerApiServices.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, 
                          IChatRepository chatRepository, IUserService userService, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _userService = userService;
        _mapper = mapper;
    }

    async Task<MessageResponse> IMessageService.AddAsync(Guid userId, MessageAddRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var chat = await _chatRepository.GetByIdAsync(request.ChatId)
            ?? throw new NotFoundException("Chat not found.");

        var message = new Message
        {
            Author = user,
            Chat = chat,
            SendTime = DateTime.Now,
            Text = request.Text,
            ImageUrl = request.ImageUrl,
            SelfDeletionDeadline = request.MinutesBeforeSelfDeletion is not null 
                ? DateTime.Now.AddMinutes((int)request.MinutesBeforeSelfDeletion) : null,
        };

        await _messageRepository.AddAsync(message);

        return _mapper.Map<MessageResponse>(message);
    }

    async Task<MessageResponse> IMessageService.AddUserReportAsync(Guid userId, UserReportAddRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var message = await _messageRepository.GetByIdAsync(request.MessageId)
            ?? throw new NotFoundException("Message not found.");

        message.UserReports.Add(user);

        await _messageRepository.SaveChangesAsync();

        return _mapper.Map<MessageResponse>(message);
    }

    async Task<MessageResponse> IMessageService.LikeAsync(Guid id, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var message = await _messageRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Message not found.");

        var isLiked = false;
        if (message.Likes.Contains(user))
        {
            message.Likes.Remove(user);
        }
        else
        {
            message.Likes.Add(user);
            isLiked = true;
        }

        await _messageRepository.SaveChangesAsync();

        var response = _mapper.Map<MessageResponse>(message);
        response.IsLiked = isLiked;

        return response;
    }

    async Task<ICollection<MessageResponse>> IMessageService.GetAllChatMessagesAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId)
            ?? throw new NotFoundException("Chat not found.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var messages = await _messageRepository.GetAllChatMessagesAsync(chat);

        await DeleteSelfDeletingMessages(messages);
        
        var responses = _mapper.Map<ICollection<MessageResponse>>(messages);

        var mergedMessages = messages.Zip(responses, (message, response) => new
        {
            Message = message,
            Response = response
        });

        foreach (var item in mergedMessages)
        {
            item.Response.IsLiked = item.Message.Likes.Contains(user);
        }

        return responses;
    }

    async Task<ICollection<MessageResponse>> IMessageService.GetReportedMessageAsync()
    {
        var messages = await _messageRepository.GetReportedMessageAsync();

        return _mapper.Map<ICollection<MessageResponse>>(messages);
    }

    async Task IMessageService.RemoveAsync(Guid id, Guid userId)
    {
        var message = await _messageRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Message not found.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (user.IsAdmin is false &&
            user.Id != message.Author.Id)
        {
            throw new UnauthorizedException("You are not an admin and can not perform this operation.");
        }

        message.UserReports.Clear();
        message.Text = "[Сообщение удалено]";
        message.ImageUrl = null;

        await _messageRepository.SaveChangesAsync();

        await _userService.RecordUserOperationAsync(userId,
                                                    DateTime.Now,
                                                    nameof(IMessageService.RemoveAsync),
                                                    $"Removed message with id {id}.");
    }

    async Task IMessageService.RemoveUserReportsAsync(Guid id, Guid adminId)
    {
        var message = await _messageRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Message not found.");

        message.UserReports.Clear();

        await _messageRepository.SaveChangesAsync();

        await _userService.RecordUserOperationAsync(adminId,
                                                    DateTime.Now,
                                                    nameof(IMessageService.RemoveUserReportsAsync),
                                                    $"Removed all reports from message with id {id}.");
    }

    private async Task DeleteSelfDeletingMessages(ICollection<Message> messages)
    {
        foreach (var message in messages)
        {
            if (message.SelfDeletionDeadline is null ||
                message.SelfDeletionDeadline > DateTime.Now)
            {
                continue;
            }

            message.UserReports.Clear();
            message.Text = "[Удаленное сообщение]";
            message.ImageUrl = null;
        }

        await _messageRepository.SaveChangesAsync();
    }
}
