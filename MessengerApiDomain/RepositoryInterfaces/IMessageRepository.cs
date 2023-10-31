using MessengerApiDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiDomain.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        Task<ICollection<Message>> GetAllChatMessagesAsync(Chat chat);
        Task AddAsync(Message message);
        Task SaveChangesAsync();
    }
}
