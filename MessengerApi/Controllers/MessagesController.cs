using MessengerApi.Helpers;
using MessengerApi.Hubs;
using MessengerApiDomain.Models;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MessengerApi.Controllers
{
    [Authorize]
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IHubContext<UserChatsHub> _userChatsHubContext;

        public MessagesController(IMessageService messageService, IChatService chatService,
                                  IHubContext<ChatHub> hubContext, IHubContext<UserChatsHub> userChatsHubContext)
        {
            _messageService = messageService;
            _chatService = chatService;
            _chatHubContext = hubContext;
            _userChatsHubContext = userChatsHubContext;
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

            await SendMessageAsync(id, request.ChatId, message);

            return Created($"api/messages/{message.Id}", message);
        }

        private async Task SendMessageAsync(Guid authorId, Guid chatId, MessageResponse response)
        {
            await _chatHubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", response);

            var chatUsers = await _chatService.GetChatUsers(chatId);

            var chatUsersWithoutAuthor = chatUsers
                .Where(user => user.Id != authorId)
                .ToList();

            foreach (var user in chatUsersWithoutAuthor)
            {
                await _userChatsHubContext.Clients.Group(user.Id.ToString()).SendAsync("ReceiveMessage", chatId, response);
            }
        }
    }
}
