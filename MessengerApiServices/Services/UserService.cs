using AutoMapper;
using Azure.Core;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiServices.Exceptions;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        async Task<ICollection<UserResponse>> IUserService.GetAllUsersWithoutConversationAsync(Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId)
                ?? throw new NotFoundException("User not found.");

            var users = await _userRepository.GetAllUsersWithoutConversationAsync(user);

            return _mapper.Map<ICollection<UserResponse>>(users);
        }

        async Task<ICollection<UserResponse>> IUserService.SearchUsersWithoutConversationAsync(string search, Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId)
                ?? throw new NotFoundException("User not found.");

            var users = await _userRepository.SearchUsersWithoutConversationAsync(search, user);

            return _mapper.Map<ICollection<UserResponse>>(users);
        }

        async Task<ICollection<UserResponse>> IUserService.GetAllAsync(Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId)
                ?? throw new NotFoundException("User not found.");

            var users = await _userRepository.GetAllAsync(user);

            var userResponses = _mapper.Map<ICollection<UserResponse>>(users);

            foreach (var response in userResponses)
            {
                if (user.Blacklist.Select(user => user.Id).Contains(response.Id))
                {
                    response.IsBlacklisted = true;
                }
            }

            return userResponses;
        }

        async Task<ICollection<UserResponse>> IUserService.SearchAsync(string search, Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId)
                ?? throw new NotFoundException("User not found.");

            var users = await _userRepository.SearchAsync(search, user);

            var userResponses = _mapper.Map<ICollection<UserResponse>>(users);

            foreach (var response in userResponses)
            {
                if (user.Blacklist.Select(user => user.Id).Contains(response.Id))
                {
                    response.IsBlacklisted = true;
                }
            }

            return userResponses;
        }

        async Task<UserResponse> IUserService.AddAsync(UserSignUpRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                DisplayName = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                AvatarUrl = "http://127.0.0.1:10000/devstoreaccount1/messenger-container/default-avatar.png",
            };

            await _userRepository.AddUserAsync(user);

            return _mapper.Map<UserResponse>(user);
        }

        async Task<UserResponse?> IUserService.GetByNameAsync(string name)
        {
            var user = await _userRepository.GetByNameAsync(name);

            return _mapper.Map<UserResponse?>(user);
        }

        async Task<bool> IUserService.UserExistsAsync(string name)
        {
            return await _userRepository.UserExistsAsync(name);
        }

        async Task<bool> IUserService.VerifyPasswordAsync(UserLogInRequest request)
        {
            var user = await _userRepository.GetByNameAsync(request.Name);

            if (user is null)
            {
                return false;
            }

            bool verificationResult = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            return verificationResult;
        }

        async Task<UserResponse?> IUserService.GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            return _mapper.Map<UserResponse?>(user);
        }

        async Task<UserResponse> IUserService.UpdateProfileAsync(Guid id, ProfileUpdateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User not found.");

            user.DisplayName = request.DisplayName;
            user.AvatarUrl = request.AvatarUrl;

            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserResponse>(user);
        }

        async Task IUserService.AddToBlacklistAsync(Guid id, BlacklistAddRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User not found.");

            var blacklistedUser = await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new NotFoundException("User not found.");

            user.Blacklist.Add(blacklistedUser);

            await _userRepository.SaveChangesAsync();
        }

        async Task IUserService.RemoveFromBlacklistAsync(Guid id, Guid blacklistedUserId)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User not found.");

            var blacklistedUser = await _userRepository.GetByIdAsync(blacklistedUserId)
                ?? throw new NotFoundException("User not found.");

            user.Blacklist.Remove(blacklistedUser);

            await _userRepository.SaveChangesAsync();
        }

        async Task<bool> IUserService.IsBlacklistedAsync(Guid FirstId, Guid SecondId)
        {
            var firstUser = await _userRepository.GetByIdAsync(FirstId)
                ?? throw new NotFoundException("User not found.");

            var secondUser = await _userRepository.GetByIdAsync(SecondId)
                ?? throw new NotFoundException("User not found.");

            if (firstUser.Blacklist.Contains(secondUser))
            {
                return true;
            }

            if (secondUser.Blacklist.Contains(firstUser)) 
            { 
                return true; 
            }

            return false;
        }

        async Task IUserService.BanUserAndDeleteAllMessagesAsync(Guid messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId)
                ?? throw new NotFoundException("Message not found.");

            var user = await _userRepository.GetByIdAsync(message.Author.Id)
                ?? throw new NotFoundException("User not found.");

            user.IsBanned = true;

            user.DisplayName = "[Banned user]";
            user.AvatarUrl = "http://127.0.0.1:10000/devstoreaccount1/messenger-container/default-avatar.png";

            foreach (var userMessage in user.Messages)
            {
                userMessage.Text = "[Deleted message]";
                userMessage.ImageUrl = null;
                userMessage.UserReports.Clear();
            }

            await _userRepository.SaveChangesAsync();
        }
    }
}
