using Messenger.Providers;
using MessengerModels.Models;
using System;
using System.Collections.Generic;

namespace Messenger.Helpers;

public class MessageResponseDecorator : MessageResponse
{
    public bool BelongsToCurrentUser => AuthorizationProvider.GetUserId() == AuthorId;
    public bool IsFromToday => SendTime.Date == DateTime.Now.Date;

    public MessageResponseDecorator(MessageResponse message)
    {
        Id = message.Id;
        AuthorId = message.AuthorId;
        AuthorName = message.AuthorName;
        AuthorAvatarUrl = message.AuthorAvatarUrl;
        Text = message.Text;
        ImageUrl = message.ImageUrl;
        IsLiked = message.IsLiked;
        LikeAmount = message.LikeAmount;
        SendTime = message.SendTime;
    }

    public static MessageResponseDecorator ToDecorator(MessageResponse message)
    {
        return new MessageResponseDecorator(message);
    }

    public static ICollection<MessageResponseDecorator> ToDecorator(ICollection<MessageResponse> messages)
    {
        var decorators = new List<MessageResponseDecorator>();

        foreach (var message in messages)
        {
            decorators.Add(new MessageResponseDecorator(message));
        }

        return decorators;
    }
}
