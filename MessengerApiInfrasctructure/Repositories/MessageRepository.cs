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
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;
        }

        async Task IMessageRepository.AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();
        }

        async Task<ICollection<Message>> IMessageRepository.GetAllChatMessagesAsync(Chat chat)
        {
            return await _context.Messages
                .Where(message => message.Chat.Id == chat.Id)
                .OrderBy(message => message.SendTime)
                .ToListAsync();
        }

        async Task IMessageRepository.SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
