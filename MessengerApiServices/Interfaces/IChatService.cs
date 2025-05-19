using MessengerApiDomain.Models;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Interfaces
{
    public interface IChatService
    {
        Task<ICollection<ChatResponse>> GetUserChatsAsync(Guid userId);
        Task<ICollection<UserResponse>> GetChatUsers(Guid id);
        Task<Guid> GetOrCreateChatBetweenUsersAsync(Guid firstUserId, Guid secondUserId);
        Task<ChatResponse> AddConversationAsync(Guid userId, ConversationAddRequest request);
        Task<ChatResponse> AddGroupAsync(Guid userId, GroupAddRequest request);
    }
}
