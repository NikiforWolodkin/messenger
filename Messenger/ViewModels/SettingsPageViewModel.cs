using Messenger.Interfaces;
using Messenger.Utilities;
using Messenger.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        private bool _profileSelected;
        public bool ProfileSelected
        {
            get { return _profileSelected; }
            set { _profileSelected = value; OnPropertyChanged(); }
        }

        private bool _groupsSelected;
        public bool GroupsSelected
        {
            get { return _groupsSelected; }
            set { _groupsSelected = value; OnPropertyChanged(); }
        }

        private bool _contactsSelected;
        public bool ContactsSelected
        {
            get { return _contactsSelected; }
            set { _contactsSelected = value; OnPropertyChanged(); }
        }

        private bool _blacklistSelected;
        public bool BlacklistSelected
        {
            get { return _blacklistSelected; }
            set { _blacklistSelected = value; OnPropertyChanged(); }
        }

        private bool _reportsSelected;
        public bool ReportsSelected
        {
            get { return _reportsSelected; }
            set { _reportsSelected = value; OnPropertyChanged(); }
        }

        public SettingsPageViewModel(IWindow window, SettingsTab tab)
        {
            _window = window;

            ProfileCommand = new RelayCommand(obj => SelectTab(SettingsTab.Profile));
            GroupsCommand = new RelayCommand(obj => SelectTab(SettingsTab.Groups));
            ContactsCommand = new RelayCommand(obj => SelectTab(SettingsTab.Contacts));
            BlacklistCommand = new RelayCommand(obj => SelectTab(SettingsTab.Blacklist));
            ReportsCommand = new RelayCommand(obj => SelectTab(SettingsTab.Reports));
            BackCommand = new RelayCommand(Back);

            switch (tab)
            {
                case SettingsTab.Profile:
                    ProfileSelected = true;
                    CurrentView = new ProfilePageViewModel(_window);
                    break;
                case SettingsTab.Groups:
                    GroupsSelected = true;
                    CurrentView = new GroupsPageViewModel(_window);
                    break;
                case SettingsTab.Contacts:
                    ContactsSelected = true;
                    CurrentView = new ContactsPageViewModel(_window);
                    break;
                default:
                    CurrentView = null;
                    break;
            }
        }

        public ICommand ProfileCommand { get; set; }
        public ICommand GroupsCommand { get; set; }
        public ICommand ContactsCommand { get; set; }
        public ICommand BlacklistCommand { get; set; }
        public ICommand ReportsCommand { get; set; }
        public ICommand BackCommand { get; set; }

        private void SelectTab(SettingsTab tab)
        {
            switch (tab)
            {
                case SettingsTab.Profile:
                    CurrentView = new ProfilePageViewModel(_window);
                    break;
                case SettingsTab.Groups:
                    CurrentView = new GroupsPageViewModel(_window);
                    break;
                case SettingsTab.Contacts:
                    CurrentView = new ContactsPageViewModel(_window);
                    break;
                default:
                    break;
            }
        }

        private void Back(object obj)
        {
            _window.NavigateToMain();
        }
    }
}
