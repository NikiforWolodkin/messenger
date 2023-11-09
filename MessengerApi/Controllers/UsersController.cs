﻿using MessengerApi.Helpers;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            return Ok(await _userService.GetByIdAsync(id));
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfileAsync(ProfileUpdateRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            return Ok(await _userService.UpdateProfileAsync(id, request));
        }

        [Authorize]
        [HttpPost("profile/blacklist")]
        public async Task<IActionResult> AddToBlacklistAsync(BlacklistAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            await _userService.AddToBlacklistAsync(id, request);

            return Ok();
        }

        [Authorize]
        [HttpDelete("profile/blacklist/{blacklistedUserId:Guid}")]
        public async Task<IActionResult> AddToBlacklistAsync(Guid blacklistedUserId)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            await _userService.RemoveFromBlacklistAsync(id, blacklistedUserId);

            return Ok();
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllAsync(string? search, bool? filterUsersWithConversation)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            if (filterUsersWithConversation is not null && filterUsersWithConversation == true)
            {
                if (string.IsNullOrEmpty(search))
                {
                    return Ok(await _userService.GetAllUsersWithoutConversationAsync(id));
                }

                return Ok(await _userService.SearchUsersWithoutConversationAsync(search, id));
            }

            if (string.IsNullOrEmpty(search))
            {
                return Ok(await _userService.GetAllAsync(id));
            }

            return Ok(await _userService.SearchAsync(search, id));
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddAsync(UserSignUpRequest request)
        {
            if (await _userService.UserExistsAsync(request.Name))
            {
                return BadRequest(new ErrorResponse("This username is already in use."));
            }

            var user = await _userService.AddAsync(request);

            return Created($"api/users/{user.Id}", user);
        }
    }
}
