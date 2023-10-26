using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace MessengerApi.Hubs
{
    [Authorize]
    public class UserChatsHub : Hub
    {
        public async Task SendMessage(Guid userId, Guid chatId, MessageResponse response)
        {
            await Clients.Group(userId.ToString()).SendAsync("ReceiveMessage", chatId, response);
        }

        public async Task AddChat(Guid userId, ChatResponse chat)
        {
            await Clients.Group(userId.ToString()).SendAsync("ReceiveChat", chat);
        }

        public async Task JoinHub(Guid userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        }

        public async Task LeaveHub(Guid userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        }
    }
}
