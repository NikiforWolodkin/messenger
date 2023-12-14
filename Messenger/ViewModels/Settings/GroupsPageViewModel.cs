using Messenger.Helpers;
using Messenger.Interfaces;
using Messenger.Services;
using Messenger.Utilities;
using MessengerApiDomain.Models;
using MessengerModels.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Messenger.ViewModels.Settings
{
    public class GroupsPageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private List<UserResponseDecorator> _selectedUsers = new ();

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _search;
        public string Search
        {
            get { return _search; }
            set { _search = value; OnPropertyChanged(); }
        }

        private string _avatarUrl;
        public string AvatarUrl 
        {
            get { return _avatarUrl; }
            set { _avatarUrl = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UserResponseDecorator> _users;
        public ObservableCollection<UserResponseDecorator> Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged(); }
        }

        public GroupsPageViewModel(IWindow window)
        {
            AddParticipantCommand = new AsyncRelayCommand(AddParticipantAsync);
            RemoveParticipantCommand = new AsyncRelayCommand(RemoveParticipantAsync);
            CreateGroupCommand = new AsyncRelayCommand(CreateGroupAsync);
            SearchUsersCommand = new AsyncRelayCommand(SearchUsersAsync);
            UploadImageCommand = new AsyncRelayCommand(UploadImageAsync);

            Name = string.Empty;
            Search = string.Empty;
            AvatarUrl = "http://127.0.0.1:10000/devstoreaccount1/messenger-container/default-avatar.png";

            GetUsersAsync();
            _window = window;
        }

        private async Task GetUsersAsync()
        {
            var users = await UsersService.SearchAsync();

            Users = new ObservableCollection<UserResponseDecorator>(UserResponseDecorator.ToDecorator(users));
        }

        public ICommand AddParticipantCommand { get; set; }
        public ICommand RemoveParticipantCommand { get; set; }
        public ICommand CreateGroupCommand { get; set; }
        public ICommand SearchUsersCommand { get; set; }
        public ICommand UploadImageCommand { get; set; }

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

        private async Task CreateGroupAsync(object obj)
        {
            var participantIds = _selectedUsers.Select(user => user.Id).ToList();

            await ChatsService.AddGroupAsync(Name, AvatarUrl, participantIds);

            _window.NavigateToMain();
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

        private async Task UploadImageAsync(object obj)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var extension = Path.GetExtension(openFileDialog.FileName).ToLower();

                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    var imageUrl = await FilesService.UploadImageAsync(openFileDialog.FileName);

                    AvatarUrl = imageUrl;
                }
                else
                {
                    MessageBox.Show("Only PNG and JPEG files are allowed.", "Incorrect file format");
                }
            }
        }
    }
}
