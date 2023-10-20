using Messenger.Interfaces;
using Messenger.Utilities;
using Messenger.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public SettingsPageViewModel(IWindow window)
        {
            _window = window;

            BackCommand = new RelayCommand(Back);

            ContactsSelected = true;
            CurrentView = new ContactsPageViewModel(window);
        }

        public ICommand BackCommand { get; set; }

        private void Back(object obj)
        {
            _window.NavigateToMain();
        }
    }
}
