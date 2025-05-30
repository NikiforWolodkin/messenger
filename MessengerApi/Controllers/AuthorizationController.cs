﻿using MessengerApiServices.Exceptions;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessengerApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthorizationController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> AuthorizeAsync(UserLogInRequest request)
        {
            var verificationResult = await _userService.VerifyPasswordAsync(request);

            if (!verificationResult)
            {
                return BadRequest(new ErrorResponse("Неверный логин или пароль."));
            }

            var user = await _userService.GetByNameAsync(request.Name)
                ?? throw new NotFoundException("Пользователь не найден.");

            if (user.IsBanned)
            {
                return BadRequest(new ErrorResponse("Вы были заблокированы и не можете пользоваться сервисом."));
            }

            var token = CreateToken(user.Id, user.Name);

            var response = new AuthorizationResponse
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                AuthorizationToken = token,
            };

            return Ok(response);
        }

        private string CreateToken(Guid id, string name)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Token").Value!
            ));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.Now.AddHours(24)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
