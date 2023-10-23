using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiInfrasctructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiInfrasctructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext _context;

        public ChatRepository(DataContext context)
        {
            _context = context;
        }

        async Task IChatRepository.AddAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);

            await _context.SaveChangesAsync();
        }

        async Task<ICollection<Chat>> IChatRepository.GetAllAsync()
        {
            return await _context.Chats.ToListAsync();
        }

        async Task<Chat?> IChatRepository.GetByIdAsync(Guid id)
        {
            return await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == id);
        }

        async Task<ICollection<Chat>> IChatRepository.GetUserChatsAsync(User user)
        {
            return await _context.Chats
                .Where(chat => chat.Participants.Contains(user))
                .ToListAsync();
        }
    }
}
