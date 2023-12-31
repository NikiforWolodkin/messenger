﻿using MessengerApi.Helpers;
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetChatMessagesAsync(Guid id)
        {
            return Ok(await _messageService.GetAllChatMessagesAsync(id));
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

            var message = await _messageService.AddUserReportAsync(id, request);

            return Created($"api/messages/{message.Id}", message);
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

            await _messageService.RemoveUserReportsAsync(messageId);

            return NoContent();
        }

        [HttpDelete("{messageId:Guid}")]
        public async Task<IActionResult> RemoveMessageAsync(Guid messageId)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            var user = await _userService.GetByIdAsync(id);

            if (!user.IsAdmin)
            {
                return Unauthorized(new ErrorResponse("You are not an admin and can not perform this operation."));
            }

            await _messageService.RemoveAsync(messageId);

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
