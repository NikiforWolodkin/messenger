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

        async Task<UserResponse> IUserService.AddAsync(UserSignUpRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
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
    }
}
