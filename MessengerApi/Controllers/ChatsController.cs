using MessengerApi.Helpers;
using MessengerApi.Hubs;
using MessengerApiDomain.Enums;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MessengerApi.Controllers
{
    [Authorize]
    [Route("api/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<UserChatsHub> _userChatsHubContext;

        public ChatsController(IChatService chatService, IHubContext<UserChatsHub> userChatsHubContext)
        {
            _chatService = chatService;
            _userChatsHubContext = userChatsHubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string? search)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            return Ok(await _chatService.GetUserChatsAsync(id));
        }

        [HttpPost("conversations")]
        public async Task<IActionResult> AddConversationAsync(ConversationAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            var chat = await _chatService.AddConversationAsync(id, request);

            await SendChatAsync(id, chat);

            return Created($"api/chats/{chat.Id}", chat);
        }

        [HttpPost("groups")]
        public async Task<IActionResult> AddGroupAsync(GroupAddRequest request)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            var chat = await _chatService.AddGroupAsync(id, request);

            await SendChatAsync(id, chat);

            return Created($"api/chats/{chat.Id}", chat);
        }

        private async Task SendChatAsync(Guid authorId, ChatResponse chat)
        {
            var chatUsers = await _chatService.GetChatUsers(chat.Id);

            var chatUsersWithoutAuthor = chatUsers
                .Where(user => user.Id != authorId)
                .ToList();

            var chatToSend = chat;

            if (chat.Type == ChatType.Conversation)
            {
                chatToSend = new ChatResponse
                {
                    Id = chat.Id,
                    ImageUrl = chatUsers.Where(user => user.Id == authorId).First().AvatarUrl,
                    Name = chatUsers.Where(user => user.Id == authorId).First().DisplayName,
                    Type = ChatType.Conversation,
                };
            }

            foreach (var user in chatUsersWithoutAuthor)
            {
                await _userChatsHubContext.Clients.Group(user.Id.ToString()).SendAsync("ReceiveChat", chatToSend);
            }
        }
    }
}
