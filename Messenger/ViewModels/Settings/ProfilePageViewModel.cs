using Messenger.Interfaces;
using Messenger.Providers;
using Messenger.Services;
using Messenger.Utilities;
using MessengerModels.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels.Settings
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private UserResponse _profile;
        public UserResponse Profile
        {
            get { return _profile; }
            set { _profile = value; OnPropertyChanged(); }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set 
            { 
                _displayName = value; 
                OnPropertyChanged(); 

                if (DisplayName is not null)
                {
                    Profile.DisplayName = value;
                }
            }
        }

        public ProfilePageViewModel(IWindow window)
        {
            _window = window;

            SignOutCommand = new RelayCommand(SignOut);
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            UploadImageCommand = new AsyncRelayCommand(UploadImageAsync);

            Profile = new UserResponse
            {
                Name = string.Empty,
                DisplayName = string.Empty,
                AvatarUrl = "http://127.0.0.1:10000/devstoreaccount1/messenger-container/default-avatar.png",
            };
            DisplayName = string.Empty;

            GetProfileAsync();
        }

        private async Task GetProfileAsync()
        {
            var profile = await UsersService.GetProfileAsync();

            Profile = profile;
            DisplayName = profile.DisplayName;
        }

        public ICommand SignOutCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand UploadImageCommand { get; set; }

        private void SignOut(object obj)
        {
            AuthorizationProvider.LogOut();

            _window.NavigateToLogin();
        }

        private async Task SaveAsync(object obj)
        {
            await UsersService.UpdateProfileAsync(DisplayName, Profile.AvatarUrl);

            _window.NavigateToMain();
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

                    Profile.AvatarUrl = imageUrl;
                    OnPropertyChanged(nameof(Profile));
                }
                else
                {
                    MessageBox.Show("Only PNG and JPEG files are allowed.", "Incorrect file format");
                }
            }
        }
    }
}
