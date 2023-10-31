using MessengerApiDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.RepositoryInterfaces
{
    public interface IChatRepository
    {
        Task<ICollection<Chat>> GetAllAsync();
        Task<ICollection<Chat>> GetUserChatsAsync(User user);
        Task AddAsync(Chat chat);
        Task<Chat?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
