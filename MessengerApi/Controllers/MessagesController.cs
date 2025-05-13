using MessengerApi.Helpers;
using MessengerApi.Hubs;
using MessengerApiDomain.Models;
using MessengerApiServices.Exceptions;
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
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IHubContext<UserChatsHub> _userChatsHubContext;

        public MessagesController(IMessageService messageService, IChatService chatService,
                                  IUserService userService, IHubContext<ChatHub> hubContext, 
                                  IHubContext<UserChatsHub> userChatsHubContext) 
        {
            _messageService = messageService;
            _chatService = chatService;
            _userService = userService;
            _chatHubContext = hubContext;
            _userChatsHubContext = userChatsHubContext;
        }

        [HttpGet("{chatId:guid}")]
        public async Task<IActionResult> GetChatMessagesAsync(Guid chatId)
        {
            var userId = JwtClaimsHelper.GetId(User.Identity);

            return Ok(await _messageService.GetAllChatMessagesAsync(chatId, userId));
        }

        [HttpGet("reported")]
        public async Task<IActionResult> GetReportedMessagesAsync()
        {
            return Ok(await _messageService.GetReportedMessageAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(MessageAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            var message = await _messageService.AddAsync(id, request);

            await SendMessageAsync(id, request.ChatId, message);

            return Created($"api/messages/{message.Id}", message);
        }

        [HttpPost("reports")]
        public async Task<IActionResult> AddUserReportAsync(UserReportAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            await _messageService.AddUserReportAsync(id, request);

            return Ok();
        }

        [HttpPut("likes/{messageId:Guid}")]
        public async Task<IActionResult> LikeAsync(Guid messageId)
        {
            var userId = JwtClaimsHelper.GetId(User.Identity);

            var message = await _messageService.LikeAsync(messageId, userId);

            return Ok(message);
        }

        [HttpDelete("reports/{messageId:Guid}")]
        public async Task<IActionResult> RemoveUserReportsAsync(Guid messageId)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            var user = await _userService.GetByIdAsync(id);

            if (!user.IsAdmin)
            {
                return Unauthorized(new ErrorResponse("You are not an admin and can not perform this operation."));
            }

            await _messageService.RemoveUserReportsAsync(messageId, id);

            return NoContent();
        }

        [HttpDelete("{messageId:Guid}")]
        public async Task<IActionResult> RemoveMessageAsync(Guid messageId)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            try
            {
                await _messageService.RemoveAsync(messageId, id);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new ErrorResponse(ex.Message));
            }

            return NoContent();
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
