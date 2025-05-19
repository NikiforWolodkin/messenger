using Messenger.Helpers;
using Messenger.Interfaces;
using Messenger.Services;
using Messenger.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;

namespace Messenger.ViewModels;

public class EventPageViewModel : ViewModelBase
{
    private readonly IWindow _window;
    private List<UserResponseDecorator> _selectedUsers = new();

    // Event properties
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private DateTime? _date = DateTime.Today;
    public DateTime? Date
    {
        get => _date;
        set { _date = value; OnPropertyChanged(); }
    }

    private TimeSpan _startTime;
    public TimeSpan StartTime
    {
        get => _startTime;
        set { _startTime = value; OnPropertyChanged(); UpdateEndTimes(); }
    }

    private TimeSpan _endTime;
    public TimeSpan EndTime
    {
        get => _endTime;
        set { _endTime = value; OnPropertyChanged(); }
    }

    private string _agenda = string.Empty;
    public string Agenda
    {
        get => _agenda;
        set { _agenda = value; OnPropertyChanged(); }
    }

    private string _search;
    public string Search
    {
        get => _search;
        set { _search = value; OnPropertyChanged(); }
    }

    private ObservableCollection<UserResponseDecorator> _users;
    public ObservableCollection<UserResponseDecorator> Users
    {
        get => _users;
        set { _users = value; OnPropertyChanged(); }
    }

    private ObservableCollection<TimeSpan> _availableStartTimes;
    public ObservableCollection<TimeSpan> AvailableStartTimes
    {
        get => _availableStartTimes;
        set { _availableStartTimes = value; OnPropertyChanged(); }
    }

    private ObservableCollection<TimeSpan> _availableEndTimes;
    public ObservableCollection<TimeSpan> AvailableEndTimes
    {
        get => _availableEndTimes;
        set { _availableEndTimes = value; OnPropertyChanged(); }
    }

    public EventPageViewModel(IWindow window)
    {
        _window = window;

        AddParticipantCommand = new AsyncRelayCommand(AddParticipantAsync);
        RemoveParticipantCommand = new AsyncRelayCommand(RemoveParticipantAsync);
        CreateEventCommand = new AsyncRelayCommand(CreateEventAsync);
        SearchUsersCommand = new AsyncRelayCommand(SearchUsersAsync);
        BackCommand = new RelayCommand(Back);

        InitializeTimeSlots();
        GetUsersAsync();
    }

    private void InitializeTimeSlots()
    {
        AvailableStartTimes = new ObservableCollection<TimeSpan>();
        AvailableEndTimes = new ObservableCollection<TimeSpan>();

        // Generate time slots from 8:00 AM to 8:00 PM in 30 minute increments
        for (int hours = 8; hours <= 20; hours++)
        {
            for (int minutes = 0; minutes < 60; minutes += 30)
            {
                AvailableStartTimes.Add(new TimeSpan(hours, minutes, 0));
            }
        }

        StartTime = new TimeSpan(9, 0, 0); // Default start time 9:00 AM
        UpdateEndTimes();
    }

    private void UpdateEndTimes()
    {
        AvailableEndTimes.Clear();

        // Only allow end times after the selected start time
        foreach (var time in AvailableStartTimes)
        {
            if (time > StartTime)
            {
                AvailableEndTimes.Add(time);
            }
        }

        // Default end time to 1 hour after start
        EndTime = StartTime.Add(new TimeSpan(1, 0, 0));
    }

    private async Task GetUsersAsync()
    {
        var users = await UsersService.SearchAsync();

        Users = [.. UserResponseDecorator.ToDecorator(users)];
    }

    public ICommand AddParticipantCommand { get; }
    public ICommand RemoveParticipantCommand { get; }
    public ICommand CreateEventCommand { get; }
    public ICommand SearchUsersCommand { get; }
    public ICommand BackCommand { get; }

    private void Back(object obj)
    {
        _window.NavigateToCalendar();
    }

    private async Task AddParticipantAsync(object obj)
    {
        if (obj is UserResponseDecorator user)
        {
            user.IsSelected = true;
            _selectedUsers.Add(user);
            CollectionViewSource.GetDefaultView(Users).Refresh();
        }
    }

    private async Task RemoveParticipantAsync(object obj)
    {
        if (obj is UserResponseDecorator user)
        {
            user.IsSelected = false;
            _selectedUsers.Remove(user);
            CollectionViewSource.GetDefaultView(Users).Refresh();
        }
    }

    private async Task CreateEventAsync(object obj)
    {
        if (string.IsNullOrEmpty(Name))
        {
            MessageBox.Show("Имя не может быть пустым.");
            return;
        }

        if (Date == null)
        {
            MessageBox.Show("Пожалуйста выберите дату.");
            return;
        }

        if (EndTime <= StartTime)
        {
            MessageBox.Show("Время окончания должно быть после времени начала.");
            return;
        }

        var participantIds = _selectedUsers.Select(user => user.Id).ToList();
        var startDateTime = Date.Value.Date.Add(StartTime);
        var endDateTime = Date.Value.Date.Add(EndTime);
        var duration = endDateTime - startDateTime;
        int durationInMinutes = (int)duration.TotalMinutes;

        await EventService.AddAsync(Name, Agenda, startDateTime, durationInMinutes, participantIds);

        _window.NavigateToCalendar();
    }

    private async Task SearchUsersAsync(object obj)
    {
        var users = await UsersService.SearchAsync(Search);
        var decorators = UserResponseDecorator.ToDecorator(users);

        foreach (var decorator in decorators)
        {
            if (_selectedUsers.Select(user => user.Id).Contains(decorator.Id))
            {
                decorator.IsSelected = true;
            }
        }

        Users = new ObservableCollection<UserResponseDecorator>(decorators);
    }
}
