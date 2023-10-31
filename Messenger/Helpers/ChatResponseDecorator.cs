using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Helpers
{
    public class ChatResponseDecorator : ChatResponse
    {
        public int UnreadMessages { get; set; } = 0;

        public ChatResponseDecorator(ChatResponse chat) 
        { 
            Id = chat.Id;
            Name = chat.Name;
            LastMessage = chat.LastMessage;
            LastMessageTime = chat.LastMessageTime;
            ImageUrl = chat.ImageUrl;
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
}
