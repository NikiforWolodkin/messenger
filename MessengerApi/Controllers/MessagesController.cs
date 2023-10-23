using MessengerApi.Helpers;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApi.Controllers
{
    [Authorize]
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetChatMessagesAsync(Guid id)
        {
            return Ok(await _messageService.GetAllChatMessagesAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(MessageAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            var message = await _messageService.AddAsync(id, request);

            return Created($"api/messages/{message.Id}", message);
        }
    }
}
