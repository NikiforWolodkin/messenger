using MessengerApi.Helpers;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string? search)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            if (string.IsNullOrEmpty(search))
            {
                return Ok(await _userService.GetAllAsync(id));
            }

            return Ok(await _userService.SearchAsync(search, id));
        }

        [HttpPost]
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
