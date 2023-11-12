using MessengerApiDomain.Models;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Interfaces
{
    public interface IMessageService
    {
        Task<ICollection<MessageResponse>> GetAllChatMessagesAsync(Guid chatId);
        Task<ICollection<MessageResponse>> GetReportedMessageAsync();
        Task<MessageResponse> AddAsync(Guid userId, MessageAddRequest request);
        Task<MessageResponse> AddUserReportAsync(Guid userId, UserReportAddRequest request);
        Task RemoveUserReportsAsync(Guid id);
        Task RemoveAsync(Guid id);
    }
}
