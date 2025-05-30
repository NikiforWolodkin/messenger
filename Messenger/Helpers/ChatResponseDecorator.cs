﻿using MessengerModels.Models;
using System;
using System.Collections.Generic;

namespace Messenger.Helpers;

public class ChatResponseDecorator : ChatResponse
{
    public int UnreadMessages { get; set; } = 0;
    public bool LastMessageIsFromToday => LastMessageTime?.Date == DateTime.Now.Date;

    private ChatResponseDecorator(ChatResponse chat) 
    { 
        Id = chat.Id;
        Name = chat.Name;
        LastMessage = chat.LastMessage;
        LastMessageTime = chat.LastMessageTime;
        ImageUrl = chat.ImageUrl;
        IsBlacklisted = chat.IsBlacklisted;
    }

    public static ChatResponseDecorator ToDecorator(ChatResponse chat)
    {
        return new ChatResponseDecorator(chat);
    }

    public static ICollection<ChatResponseDecorator> ToDecorator(ICollection<ChatResponse> chats)
    {
        var decorators = new List<ChatResponseDecorator>();

        foreach (var chat in chats)
        {
            decorators.Add(new ChatResponseDecorator(chat));
        }

        return decorators;
    }
}
