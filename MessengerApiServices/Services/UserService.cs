using AutoMapper;
using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
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
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        async Task<ICollection<UserResponse>> IUserService.GetAllUsersWithoutConversationAsync(Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId);

            var users = await _userRepository.GetAllUsersWithoutConversationAsync(user);

            return _mapper.Map<ICollection<UserResponse>>(users);
        }

        async Task<ICollection<UserResponse>> IUserService.SearchUsersWithoutConversationAsync(string search, Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId);

            var users = await _userRepository.SearchUsersWithoutConversationAsync(search, user);

            return _mapper.Map<ICollection<UserResponse>>(users);
        }

        async Task<ICollection<UserResponse>> IUserService.GetAllAsync(Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId);

            var users = await _userRepository.GetAllAsync(user);

            return _mapper.Map<ICollection<UserResponse>>(users);
        }

        async Task<ICollection<UserResponse>> IUserService.SearchAsync(string search, Guid filterUserId)
        {
            var user = await _userRepository.GetByIdAsync(filterUserId);

            var users = await _userRepository.SearchAsync(search, user);

            return _mapper.Map<ICollection<UserResponse>>(users);
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
            var user = await _userRepository.GetByIdAsync(id);

            user.DisplayName = request.DisplayName;
            user.AvatarUrl = request.AvatarUrl;

            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserResponse>(user);
        }
    }
}
