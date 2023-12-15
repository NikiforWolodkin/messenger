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
using System.Windows.Data;
using System.Windows.Input;

namespace Messenger.ViewModels.Settings
{
    public class BlacklistPageViewModel : ViewModelBase
    {
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

        public BlacklistPageViewModel()
        {
            AddToBlacklistCommand = new AsyncRelayCommand(AddToBlacklistAsync);
            RemoveFromBlacklistCommand = new AsyncRelayCommand(RemoveFromBlacklistAsync);
            SearchUsersCommand = new AsyncRelayCommand(SearchUsersAsync);

            Users = new ObservableCollection<UserResponse>();

            GetUsersAsync();
        }

        private async Task GetUsersAsync()
        {
            var users = await UsersService.SearchAsync();

            Users = new ObservableCollection<UserResponse>(users);
        }

        public ICommand AddToBlacklistCommand { get; set; }
        public ICommand RemoveFromBlacklistCommand { get; set; }
        public ICommand SearchUsersCommand { get; set; }

        private async Task AddToBlacklistAsync(object obj)
        {
            if (obj is UserResponse user)
            {
                await UsersService.AddToBlacklistAsync(user.Id);

                user.IsBlacklisted = true;

                CollectionViewSource.GetDefaultView(Users).Refresh();
            }
        }

        private async Task RemoveFromBlacklistAsync(object obj)
        {
            if (obj is UserResponse user)
            {
                await UsersService.RemoveFromBlacklistAsync(user.Id);

                user.IsBlacklisted = false;

                CollectionViewSource.GetDefaultView(Users).Refresh();
            }
        }

        private async Task SearchUsersAsync(object obj)
        {
            var users = await UsersService.SearchAsync(Search);

            Users = new ObservableCollection<UserResponse>(users);
        }
    }
}
