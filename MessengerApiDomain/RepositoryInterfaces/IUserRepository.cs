using MessengerApiDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetAllUsersWithoutConversationAsync(User filterUser);
        Task<ICollection<User>> SearchUsersWithoutConversationAsync(string search, User filterUser);
        Task<ICollection<User>> GetAllAsync(User filterUser);
        Task<ICollection<User>> SearchAsync(string search, User filterUser);
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string name);
        Task<User?> GetByNameAsync(string name);
        Task<User?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
