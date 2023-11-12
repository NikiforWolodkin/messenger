using Messenger.Exceptions;
using Messenger.Interfaces;
using Messenger.Utilities;
using Messenger.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messenger.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IWindow
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                try
                {
                    _currentView = value;
                    OnPropertyChanged();
                }
                catch (LoggedOutException ex)
                {
                    MessageBox.Show($"You are logged out. Please log in again. Details: {ex.Message}", "Logged out");

                    NavigateToLogin();
                }
                catch (HttpException ex)
                {
                    if (ex.ErrorCode == HttpStatusCode.Unauthorized || ex.ErrorCode == HttpStatusCode.Forbidden)
                    {
                        MessageBox.Show($"An authorization error occurred: {ex.Message}", "Error");

                        NavigateToLogin();
                    }
                    else
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error");
                }
            }
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
    }
}
