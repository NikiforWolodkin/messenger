using Messenger.Interfaces;
using Messenger.Utilities;
using System.Windows.Input;

namespace Messenger.ViewModels;

public class LandingPageViewModel : ViewModelBase
{
    private readonly IWindow _window;

    public ICommand LoginCommand { get; set; }
    public ICommand RegisterCommand { get; set; }

    public LandingPageViewModel(IWindow window)
    {
        _window = window;

        LoginCommand = new RelayCommand(Login);
        RegisterCommand = new RelayCommand(Register);
    }

    private void Login(object obj)
    {
        _window.NavigateToLogin();
    }

    private void Register(object obj)
    {
        _window.NavigateToSignup();
    }
}
