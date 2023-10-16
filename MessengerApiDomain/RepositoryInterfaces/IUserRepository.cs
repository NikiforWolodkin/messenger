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
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string name);
        Task<User?> GetByNameAsync(string name);
    }
}
