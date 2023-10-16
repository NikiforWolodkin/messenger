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
    public class SignupPageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private string _repeatPassword;
        public string RepeatPassword
        {
            get { return _repeatPassword; }
            set { _repeatPassword = value; OnPropertyChanged(); }
        }

        public SignupPageViewModel(IWindow window)
        {
            _window = window;

            SignUpCommand = new AsyncRelayCommand(SignUpAsync);
            ToLoginPageCommand = new RelayCommand(ToLoginPage);

            Login = string.Empty;
            Password = string.Empty;
            RepeatPassword = string.Empty;
        }

        public ICommand SignUpCommand { get; set; }
        public ICommand ToLoginPageCommand { get; set; }

        private async Task SignUpAsync(object obj)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Login and password can't be empty.", "Error");
            }

            if (Password != RepeatPassword)
            {
                MessageBox.Show("Please check your password.", "Error");
            }

            try
            {
                await AuthorizationProvider.SignUpAsync(Login, Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");

                return;
            }

            _window.NavigateToLogin();
        }

        private void ToLoginPage(object obj)
        {
            _window.NavigateToLogin();
        }
    }
}
