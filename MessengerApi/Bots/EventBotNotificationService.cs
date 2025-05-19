using MessengerApiServices.Interfaces;

namespace MessengerApi.Bots;

public class EventNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _notificationWindow = TimeSpan.FromMinutes(15);

    public EventNotificationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_checkInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckAndNotifyEventsAsync(stoppingToken);
        }
    }

    private async Task CheckAndNotifyEventsAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var calendarEventService = scope.ServiceProvider.GetRequiredService<ICalendarEventService>();
        var eventBot = scope.ServiceProvider.GetRequiredService<EventBot>();

        var events = await calendarEventService.GetEventsToNotifyParticipantsAsync(_notificationWindow);

        foreach (var calendarEvent in events)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            await eventBot.NotifyUsersOfUpcomingEventAsync(calendarEvent);
        }
    }
}