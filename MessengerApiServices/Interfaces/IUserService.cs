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
        Task<ICollection<UserResponse>> GetAllAsync(Guid filterUserId);
        Task<ICollection<UserResponse>> SearchAsync(string search, Guid filterUserId);
        Task<UserResponse?> GetByNameAsync(string name);
        Task<UserResponse> AddAsync(UserSignUpRequest request);
        Task<bool> UserExistsAsync(string name);
        Task<bool> VerifyPasswordAsync(UserLogInRequest request);
    }
}
