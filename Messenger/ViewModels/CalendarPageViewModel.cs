using Messenger.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using MessengerModels.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Messenger.Interfaces;
using Messenger.Services;
using MessengerApiDomain.Models;
using System.Threading.Tasks;

namespace Messenger.ViewModels;

public class CalendarPageViewModel : ViewModelBase, IDisposable
{
    private readonly IWindow _window;

    private ObservableCollection<CalendarEventResponse> _events = [];
    public ObservableCollection<CalendarEventResponse> Events 
    {
        get => _events;
        set { _events = value; OnPropertyChanged(); }
    }

    private DateTime _selectedDay = DateTime.Now;
    public DateTime SelectedDay
    {
        get => _selectedDay;
        set 
        { 
            _selectedDay = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(IsCurrentDay));
            GetEventsAsync();
        }
    }

    public ICommand BackCommand { get; set; }
    public ICommand NewEventCommand { get; set; }
    public ICommand YesCommand { get; set; }
    public ICommand NoCommand { get; set; }

    public CalendarPageViewModel(IWindow window)
    {
        _window = window;

        BackCommand = new RelayCommand(Back);
        NewEventCommand = new RelayCommand(NewEvent);
        YesCommand = new RelayCommand(YesAsync);
        NoCommand = new RelayCommand(NoAsync);

        InitializeHours();
        InitializeTimer();
        UpdateCurrentTimeIndicator();

        GetEventsAsync();
    }

    public async Task RemoveAsync(object obj)
    {
        if (obj is CalendarEventResponse eventResponse)
        {
            await EventService.DeleteAsync(eventResponse.Id);

            await GetEventsAsync();
        }
    }

    private async Task GetEventsAsync()
    {
        Events = [.. await EventService.GetUserEventsForDayAsync(SelectedDay)];

        CalculateEventLayout();
    }

    public async void YesAsync(object obj)
    {
        if (obj is Guid id)
        {
            await EventService.SetAttendanceAsync(id, true);

            await GetEventsAsync();
        }
    }

    public async void NoAsync(object obj)
    {
        if (obj is Guid id)
        {
            await EventService.SetAttendanceAsync(id, false);

            await GetEventsAsync();
        }
    }

    private void Back(object obj)
    {
        _window.NavigateToMain();
    }

    private void NewEvent(object obj)
    {
        _window.NavigateToNewEvent();
    }

    #region Calendar display logic
    private System.Windows.Threading.DispatcherTimer _timer;

    public List<string> Hours { get; } = [];
    public ObservableCollection<CalendarEventModel> EventModels { get; } = [];

    private double _currentTimePosition;
    public double CurrentTimePosition
    {
        get => _currentTimePosition;
        set { _currentTimePosition = value; OnPropertyChanged(); }
    }

    public bool IsCurrentDay => SelectedDay.Date == DateTime.Now.Date;

    private void InitializeHours()
    {
        for (int hour = 0; hour < 24; hour++)
        {
            Hours.Add($"{hour:00}:00");
        }
    }

    void InitializeTimer()
    {
        _timer = new System.Windows.Threading.DispatcherTimer();
        _timer.Interval = TimeSpan.FromMinutes(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    void OnTimerTick(object? sender, EventArgs e)
    {
        UpdateCurrentTimeIndicator();
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
        EventModels.Clear();

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

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }
    #endregion
}
