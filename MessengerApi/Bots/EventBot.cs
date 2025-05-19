using MessengerApi.Hubs;
using MessengerApiDomain.Models;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.SignalR;

namespace MessengerApi.Bots;

public class EventBot
{
    public const string BotName = "EventBot";

    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IHubContext<UserChatsHub> _userChatsHubContext;

    public EventBot(IUserService userService,
                    IChatService chatService,
                    IHubContext<ChatHub> chatHubContext,
                    IHubContext<UserChatsHub> userChatsHubContext,
                    IMessageService messageService)
    {
        _userService = userService;
        _chatService = chatService;
        _chatHubContext = chatHubContext;
        _userChatsHubContext = userChatsHubContext;
        _messageService = messageService;
    }

    public async Task NotifyUsersOfUpcomingEventAsync(CalendarEventResponse calendarEvent)
    {
        var users = new List<UserResponse>();
        foreach (var user in calendarEvent.Participants)
        {
            if (user.IsAttending is false)
                continue;

            users.Add(await _userService.GetByIdAsync(user.Id));
        }

        var bot = await _userService.GetByNameAsync(BotName);

        var chatIds = new List<Guid>();
        foreach (var user in users)
        {
            chatIds.Add(await _chatService.GetOrCreateChatBetweenUsersAsync(user.Id, bot.Id));
        }
        var message = $"Скоро начинается мероприятие: {calendarEvent.Name}, {calendarEvent.StartTime} - {calendarEvent.EndTime}; присутствующие: {string.Join(", ", calendarEvent.Participants.Where(participant => participant.IsAttending == true).Select(participant => participant.DisplayName))}";
        foreach (var chatId in chatIds)
        {
            var response = await _messageService.AddAsync(bot.Id, new()
            {
                ChatId = chatId,
                Text = message,
            });
            await SendMessageAsync(bot.Id, chatId, response);
        }
    }

    public async Task NotifyUsersOfNewEventAsync(CalendarEventResponse calendarEvent)
    {
        var users = new List<UserResponse>();
        foreach (var user in calendarEvent.Participants)
        {
            if (user.IsOrganizer)
                continue;
            users.Add(await _userService.GetByIdAsync(user.Id));
        }
        var bot = await _userService.GetByNameAsync(BotName);
        var chatIds = new List<Guid>();
        foreach (var user in users)
        {
            chatIds.Add(await _chatService.GetOrCreateChatBetweenUsersAsync(user.Id, bot.Id));
        }
        
        var message = $"Вас пригласили на мероприятие: {calendarEvent.Name} - {calendarEvent.Agenda}, {calendarEvent.StartTime} - {calendarEvent.EndTime}; участники: {string.Join(", ", calendarEvent.Participants.Select(participant => participant.DisplayName))}";

        foreach (var chatId in chatIds)
        {
            var response = await _messageService.AddAsync(bot.Id, new()
            {
                ChatId = chatId,
                Text = message,
            });

            await SendMessageAsync(bot.Id, chatId, response);
        }
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
