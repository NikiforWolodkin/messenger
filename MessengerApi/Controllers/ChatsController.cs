using MessengerApi.Helpers;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApi.Controllers
{
    [Authorize]
    [Route("api/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string? search)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            return Ok(await _chatService.GetUserChatsAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(ConversationAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            var chat = await _chatService.AddConversationAsync(id, request);

            return Created($"api/chats/{chat.Id}", chat);
        }
    }
}
