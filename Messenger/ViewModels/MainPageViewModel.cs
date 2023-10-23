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

namespace Messenger.ViewModels
{
    class MainPageViewModel : ViewModelBase, IMainPage
    {
        private readonly IWindow _window;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

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
            SelectChatCommand = new RelayCommand(SelectChat);

            SelectedChat = null;
            Chats = new ObservableCollection<ChatResponse>();
            CurrentView = null;

            GetChatsAsync();
        }

        private async Task GetChatsAsync()
        {
            var chats = await ChatsService.GetAllAsync();

            Chats = new ObservableCollection<ChatResponse>(chats);
        }

        public ICommand NewChatCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand SelectChatCommand { get; set; }

        private void NewChat(object obj)
        {
            _window.NavigateToSettings();
        }

        private void Settings(object obj)
        {
            _window.NavigateToSettings();
        }

        private void SelectChat(object obj)
        {
            if (obj is ChatResponse chat)
            {
                SelectedChat = chat;
                CurrentView = new ChatPageViewModel(this, chat.Id);
            }
        }

        public void UpdateLastMessage(Guid chatId, MessageResponse message)
        {
            var chat = Chats.First(chat => chat.Id == chatId);

            chat.LastMessage = message.Text;
            chat.LastMessageTime = message.SendTime;

            CollectionViewSource.GetDefaultView(Chats).Refresh();
        }
    }
}
