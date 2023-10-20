using Messenger.Interfaces;
using Messenger.Services;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Messenger.ViewModels.Settings
{
    public class ContactsPageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private string _search;
        public string Search
        {
            get { return _search; }
            set 
            { 
                _search = value;
                OnPropertyChanged(); 
            }
        }

        private ObservableCollection<UserResponse> _users;
        public ObservableCollection<UserResponse> Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged(); }
        }

        public ContactsPageViewModel(IWindow window)
        {
            _window = window;

            AddConversationCommand = new AsyncRelayCommand(AddConversationAsync);
            SearchUsersCommand = new AsyncRelayCommand(SearchUsersAsync);

            Users = new ObservableCollection<UserResponse>();

            GetUsersAsync();
        }

        private async Task GetUsersAsync()
        {
            var users = await UsersService.SearchAsync();

            Users = new ObservableCollection<UserResponse>(users);
        }

        public ICommand AddConversationCommand { get; set; }
        public ICommand SearchUsersCommand { get; set; }

        private async Task AddConversationAsync(object obj)
        {
            if (obj is Guid userId)
            {
                await ChatsService.AddAsync(userId);

                _window.NavigateToMain();
            }
        }

        private async Task SearchUsersAsync(object obj)
        {
            var users = await UsersService.SearchAsync(Search);

            Users = new ObservableCollection<UserResponse>(users);
        }
    }
}
