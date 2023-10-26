using MessengerApi.Helpers;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessengerApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(Guid chatId, MessageResponse response)
        {
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", response);
        }

        public async Task JoinChat(Guid id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
        }

        public async Task LeaveChat(Guid id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToString());
        }
    }
}
