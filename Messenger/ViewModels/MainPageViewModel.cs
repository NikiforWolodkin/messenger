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

namespace Messenger.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        private readonly IWindow _window;

        private ObservableCollection<ChatResponse> _chats;
        public ObservableCollection<ChatResponse> Chats
        { 
            get { return _chats; }
            set { _chats = value; OnPropertyChanged(); }
        }

        private ChatResponse _selectedChat;
        public ChatResponse SelectedChat
        {
            get { return _selectedChat; }
            set { _selectedChat = value; OnPropertyChanged();}
        }

        public MainPageViewModel(IWindow window)
        {
            _window = window;

            NewChatCommand = new RelayCommand(NewChat);
            SettingsCommand = new RelayCommand(Settings);

            SelectedChat = null;
            Chats = new ObservableCollection<ChatResponse>();

            GetChatsAsync();
        }

        private async Task GetChatsAsync()
        {
            var chats = await ChatsService.GetAllAsync();

            Chats = new ObservableCollection<ChatResponse>(chats);
        }

        private async Task SearchChatsAsync()
        {

        }

        public ICommand NewChatCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        private void NewChat(object obj)
        {
            _window.NavigateToSettings();
        }

        private void Settings(object obj)
        {
            _window.NavigateToSettings();
        }
    }
}
