using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<UserResponse>> GetAllUsersWithoutConversationAsync(Guid filterUserId);
        Task<ICollection<UserResponse>> SearchUsersWithoutConversationAsync(string search, Guid filterUserId);
        Task<ICollection<UserResponse>> GetAllAsync(Guid filterUserId);
        Task<ICollection<UserResponse>> SearchAsync(string search, Guid filterUserId);
        Task<UserResponse?> GetByNameAsync(string name);
        Task<UserResponse?> GetByIdAsync(Guid id);
        Task<UserResponse> AddAsync(UserSignUpRequest request);
        Task<UserResponse> UpdateProfileAsync(Guid id, ProfileUpdateRequest request);
        Task<bool> IsBlacklistedAsync(Guid FirstId, Guid SecondId);
        Task AddToBlacklistAsync(Guid id, BlacklistAddRequest request);
        Task RemoveFromBlacklistAsync(Guid id, Guid blacklistedUserId);
        Task BanUserAndDeleteAllMessagesAsync(Guid messageId);
        Task<bool> UserExistsAsync(string name);
        Task<bool> VerifyPasswordAsync(UserLogInRequest request);
    }
}
