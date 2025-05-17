using Messenger.Interfaces;
using Messenger.Utilities;
using Messenger.ViewModels.Settings;

namespace Messenger.ViewModels;

public class MainWindowViewModel : ViewModelBase, IWindow
{
    private object _currentView;
    public object CurrentView
    {
        get { return _currentView; }
        set { _currentView = value; OnPropertyChanged(); }
    }

    public MainWindowViewModel()
    {
        CurrentView = new LoginPageViewModel(this);
    }

    public void NavigateToLogin()
    {
        CurrentView = new LoginPageViewModel(this);
    }

    public void NavigateToSignup()
    {
        CurrentView = new SignupPageViewModel(this);
    }

    public void NavigateToMain()
    {
        CurrentView = new MainPageViewModel(this);
    }

    public void NavigateToSettings(SettingsTab tab)
    {
        CurrentView = new SettingsPageViewModel(this, tab);
    }

    public void NavigateToCalendar()
    {
        CurrentView = new CalendarPageViewModel();
    }
}
