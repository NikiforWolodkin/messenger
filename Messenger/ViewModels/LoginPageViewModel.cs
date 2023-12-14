using Messenger.Interfaces;
using Messenger.Providers;
using Messenger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private string _login;
        public string Login
        {
            get { return _login; }
            set 
            { 
                _login = value; 
                OnPropertyChanged(); 
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        public LoginPageViewModel(IWindow window)
        {
            _window = window;

            LogInCommand = new AsyncRelayCommand(LogInAsync);
            ToSignupPageCommand = new RelayCommand(ToSignupPage);

            Login = string.Empty;
            Password = string.Empty;
        }

        public ICommand LogInCommand { get; set; }
        public ICommand ToSignupPageCommand { get; set; }

        private async Task LogInAsync(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Login and password can't be empty.", "Error");

                    return;
                }

                await AuthorizationProvider.LogInAsync(Login, Password);

                AuthorizationProvider.SetWindow(_window);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            _window.NavigateToMain();
        }

        private void ToSignupPage(object obj)
        {
            _window.NavigateToSignup();
        }
    }
}
