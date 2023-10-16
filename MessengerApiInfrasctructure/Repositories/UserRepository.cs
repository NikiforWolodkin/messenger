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
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        async Task IUserRepository.AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
        }

        async Task<User?> IUserRepository.GetByNameAsync(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Name == name);
        }

        async Task<bool> IUserRepository.UserExistsAsync(string name)
        {
            return await _context.Users.AnyAsync(user => user.Name == name);
        }
    }
}
