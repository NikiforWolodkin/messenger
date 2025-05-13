using MessengerModels.Models;
using MessengerApiServices.Exceptions;

namespace MessengerApiServices.Interfaces;

public interface IMessageService
{
    Task<ICollection<MessageResponse>> GetAllChatMessagesAsync(Guid chatId);
    Task<ICollection<MessageResponse>> GetReportedMessageAsync();
    Task<MessageResponse> AddAsync(Guid userId, MessageAddRequest request);
    Task<MessageResponse> AddUserReportAsync(Guid userId, UserReportAddRequest request);
    Task RemoveUserReportsAsync(Guid id, Guid adminId);
    /// <exception cref="UnauthorizedException">Thrown when user is not an admin and tries to delete a message that's not his.</exception>
    Task RemoveAsync(Guid id, Guid userId);
}
