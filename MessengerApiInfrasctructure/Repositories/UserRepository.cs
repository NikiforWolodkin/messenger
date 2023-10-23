﻿using MessengerApiDomain.Models;
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

        public async Task<ICollection<User>> GetAllAsync(User filterUser)
        {
            var usersWithConversation = filterUser
                .Chats
                .SelectMany(chat => chat.Participants)
                .Where(user => user.Id != filterUser.Id)
                .Select(user => user.Id);

            return await _context.Users
                .Where(user => user.Id != filterUser.Id)
                .Where(user => usersWithConversation.Contains(user.Id) != true)
                .ToListAsync();
        }

        public async Task<ICollection<User>> SearchAsync(string search, User filterUser)
        {
            var usersWithConversation = filterUser
                .Chats
                .SelectMany(chat => chat.Participants)
                .Where(user => user.Id != filterUser.Id)
                .Select(user => user.Id);

            return await _context.Users
                .Where(user => 
                    user.Name.ToUpper().Contains(search.ToUpper()) ||
                    user.DisplayName.ToUpper().Contains(search.ToUpper())
                )
                .Where(user => user.Id != filterUser.Id)
                .Where(user => usersWithConversation.Contains(user.Id) != true)
                .ToListAsync();
        }

        async Task IUserRepository.AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
        }

        async Task<User?> IUserRepository.GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
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