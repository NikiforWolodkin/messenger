using Messenger.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using MessengerModels.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Messenger.ViewModels;

public class CalendarPageViewModel : ViewModelBase
{
    private ObservableCollection<CalendarEventResponse> _events = [];
    public ObservableCollection<CalendarEventResponse> Events 
    {
        get => _events;
        set { _events = value; OnPropertyChanged(); }
    }

    public CalendarPageViewModel()
    {
        InitializeMockEvents();

        InitializeHours();
        CalculateEventLayout();
        UpdateCurrentTimeIndicator();

        // TODO: Dispose timer on exit
        var timer = new System.Windows.Threading.DispatcherTimer();
        timer.Interval = TimeSpan.FromMinutes(1);
        timer.Tick += (s, e) => UpdateCurrentTimeIndicator();
        timer.Start();
    }

    private void InitializeMockEvents()
    {
        var today = DateTime.Today; // Gets today at 00:00:00

        var events = new[]
        {
            new CalendarEventResponse
            {
                Id = Guid.NewGuid(),
                StartTime = today.AddHours(9), // 09:00 AM
                DurationInMinutes = 60,
                Name = "Team Meeting",
                Agenda = "Discuss project timeline",
                Participants = new List<CalendarEventUserResponse>
                {
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Alex Johnson",
                        AvatarUrl = "https://previews.123rf.com/images/creativenature/creativenature1612/creativenature161200015/69636462-%C5%9Bredniowieczny-lynx-eurazjatycki-lynx-lynx-pochodzi-od-syberii-%C5%9Brodkowej-wschodniej-i.jpg",
                        IsAttending = true,
                        IsOrganizer = true
                    },
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Sam Wilson",
                        AvatarUrl = "https://cdn-kjfbh.nitrocdn.com/NmkBGmJwBiCPDQidszuILLREQDRzBIzf/assets/images/optimized/rev-010ab0f/www.whitemarmotte.com/wp-content/uploads/2020/06/lynx_head.jpg",
                        IsAttending = true,
                        IsOrganizer = false
                    }
                }
            },
            new CalendarEventResponse
            {
                Id = Guid.NewGuid(),
                StartTime = today.AddHours(9.5), // 09:30 AM
                DurationInMinutes = 90,
                Name = "Client Call",
                Agenda = "Requirements review",
                Participants = new List<CalendarEventUserResponse>
                {
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Taylor Smith (Client)",
                        AvatarUrl = "https://previews.123rf.com/images/creativenature/creativenature1612/creativenature161200015/69636462-%C5%9Bredniowieczny-lynx-eurazjatycki-lynx-lynx-pochodzi-od-syberii-%C5%9Brodkowej-wschodniej-i.jpg",
                        IsAttending = true,
                        IsOrganizer = false
                    },
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Alex Johnson",
                        AvatarUrl = "https://previews.123rf.com/images/creativenature/creativenature1612/creativenature161200015/69636462-%C5%9Bredniowieczny-lynx-eurazjatycki-lynx-lynx-pochodzi-od-syberii-%C5%9Brodkowej-wschodniej-i.jpg",
                        IsAttending = true,
                        IsOrganizer = true
                    }
                }
            },
            new CalendarEventResponse
            {
                Id = Guid.NewGuid(),
                StartTime = today.AddHours(11), // 11:00 AM
                DurationInMinutes = 30,
                Name = "Standup",
                Agenda = "Daily updates",
                Participants = new List<CalendarEventUserResponse>
                {
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Morgan Lee",
                        AvatarUrl = "https://cdn-kjfbh.nitrocdn.com/NmkBGmJwBiCPDQidszuILLREQDRzBIzf/assets/images/optimized/rev-010ab0f/www.whitemarmotte.com/wp-content/uploads/2020/06/lynx_head.jpg",
                        IsAttending = true,
                        IsOrganizer = false
                    },
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Alex Johnson",
                        AvatarUrl = "https://previews.123rf.com/images/creativenature/creativenature1612/creativenature161200015/69636462-%C5%9Bredniowieczny-lynx-eurazjatycki-lynx-lynx-pochodzi-od-syberii-%C5%9Brodkowej-wschodniej-i.jpg",
                        IsAttending = true,
                        IsOrganizer = true
                    },
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Casey Kim",
                        AvatarUrl = "https://cdn-kjfbh.nitrocdn.com/NmkBGmJwBiCPDQidszuILLREQDRzBIzf/assets/images/optimized/rev-010ab0f/www.whitemarmotte.com/wp-content/uploads/2020/06/lynx_head.jpg",
                        IsAttending = false,
                        IsOrganizer = false
                    }
                }
            },
            new CalendarEventResponse
            {
                Id = Guid.NewGuid(),
                StartTime = today.AddHours(10.25), // 10:15 AM
                DurationInMinutes = 45,
                Name = "Lunch Break",
                Agenda = "",
                Participants = new List<CalendarEventUserResponse>
                {
                    new CalendarEventUserResponse
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Alex Johnson",
                        AvatarUrl = "https://previews.123rf.com/images/creativenature/creativenature1612/creativenature161200015/69636462-%C5%9Bredniowieczny-lynx-eurazjatycki-lynx-lynx-pochodzi-od-syberii-%C5%9Brodkowej-wschodniej-i.jpg",
                        IsAttending = true,
                        IsOrganizer = true
                    }
                }
            }
        };

        foreach (var e in events)
        {
            Events.Add(e);
        }
    }

    #region Calendar display logic
    public List<string> Hours { get; } = [];
    public ObservableCollection<CalendarEventModel> EventModels { get; } = [];

    private double _currentTimePosition;
    public double CurrentTimePosition
    {
        get => _currentTimePosition;
        set { _currentTimePosition = value; OnPropertyChanged(); }
    }

    private bool _isCurrentDay = true;
    public bool IsCurrentDay
    {
        get => _isCurrentDay;
        set { _isCurrentDay = value; OnPropertyChanged(); }
    }

    private void InitializeHours()
    {
        for (int hour = 0; hour < 24; hour++)
        {
            Hours.Add($"{hour:00}:00");
        }
    }

    private void UpdateCurrentTimeIndicator()
    {
        if (!IsCurrentDay) return;

        var now = DateTime.Now;
        double minutesIntoDay = now.Hour * 60 + now.Minute;
        CurrentTimePosition = minutesIntoDay;
    }

    private void CalculateEventLayout()
    {
        // Group overlapping events
        var eventGroups = new List<List<CalendarEventResponse>>();
        var sortedEvents = Events.OrderBy(e => e.StartTime).ToList();

        foreach (var evt in sortedEvents)
        {
            bool added = false;
            foreach (var group in eventGroups)
            {
                if (group.Any(e => e.EndTime > evt.StartTime && e.StartTime < evt.EndTime))
                {
                    group.Add(evt);
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                eventGroups.Add(new List<CalendarEventResponse> { evt });
            }
        }

        // Create view models with layout information
        foreach (var group in eventGroups)
        {
            if (group.Count == 1)
            {
                // Single event - full width
                EventModels.Add(CreateEventViewModel(group[0], 0, 2));
            }
            else
            {
                // Multiple overlapping events
                for (int i = 0; i < group.Count; i++)
                {
                    if (i == 0)
                    {
                        // First event - left column
                        EventModels.Add(CreateEventViewModel(group[i], 0, 1));
                    }
                    else if (i == 1)
                    {
                        // Second event - right column
                        EventModels.Add(CreateEventViewModel(group[i], 1, 1));
                    }
                    else
                    {
                        // Third+ events - combine into right column
                        var minStartTime = group.Skip(1).Min(e => e.StartTime);
                        var maxEndTime = group.Skip(1).Max(e => e.EndTime);
                        var combinedDuration = (int)(maxEndTime - minStartTime).TotalMinutes;

                        EventModels.Add(new CalendarEventModel
                        {
                            EventText = $"{group.Count - 1} more events ({minStartTime:HH:mm}-{maxEndTime:HH:mm})",
                            Column = 1,
                            ColumnSpan = 1,
                            Top = minStartTime.TimeOfDay.TotalMinutes,
                            Height = combinedDuration
                        });
                        break;
                    }
                }
            }
        }
    }

    private CalendarEventModel CreateEventViewModel(CalendarEventResponse evt, int column, int columnSpan)
    {
        return new CalendarEventModel
        {
            EventText = $"{evt.Name}, {evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}",
            Column = column,
            ColumnSpan = columnSpan,
            Top = evt.StartTime.TimeOfDay.TotalMinutes,
            Height = evt.DurationInMinutes
        };
    }
    #endregion
}

public class CalendarEventModel
{
    public string EventText { get; set; }
    public int Column { get; set; }
    public int ColumnSpan { get; set; }
    public double Top { get; set; }
    public double Height { get; set; }
}
