using Messenger.Helpers;
using Messenger.Interfaces;
using Messenger.Providers;
using Messenger.Services;
using Messenger.Utilities;
using Messenger.ViewModels.Settings;
using Messenger.Views;
using MessengerModels.Models;
using Microsoft.AspNetCore.SignalR.Client;
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
    class MainPageViewModel : ViewModelBase, IMainPage, IDisposable
    {
        private readonly IWindow _window;
        private readonly HubConnection _hubConnection;

        private bool _disconnected;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ChatResponseDecorator> _chats;
        public ObservableCollection<ChatResponseDecorator> Chats
        { 
            get { return _chats; }
            set { _chats = value; OnPropertyChanged(); }
        }

        private ChatResponseDecorator _selectedChat;
        public ChatResponseDecorator SelectedChat
        {
            get { return _selectedChat; }
            set { _selectedChat = value; OnPropertyChanged();}
        }

        public MainPageViewModel(IWindow window)
        {
            _window = window;
            _hubConnection = HubConnectionFactory.CreateUserChatsConnection();

            NewChatCommand = new RelayCommand(NewChat);
            SettingsCommand = new RelayCommand(Settings);
            SelectChatCommand = new RelayCommand(SelectChat);

            SelectedChat = null;
            Chats = new ObservableCollection<ChatResponseDecorator>();
            CurrentView = null;

            GetChatsAsync();
            ConnectToHubAsync();
        }

        private async Task GetChatsAsync()
        {
            var chats = await ChatsService.GetAllAsync();

            Chats = new ObservableCollection<ChatResponseDecorator>(ChatResponseDecorator.ToDecorator(chats));
        }

        private async Task ConnectToHubAsync()
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await _hubConnection.StartAsync();
            }

            var userId = AuthorizationProvider.GetUserId();

            await _hubConnection.InvokeAsync("JoinHub", userId);

            _hubConnection.On<Guid, MessageResponse>("ReceiveMessage", (chatId, message) =>
            {
                if (_disconnected)
                {
                    return;
                }

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    UpdateLastMessage(chatId, message);

                    if (SelectedChat?.Id != chatId)
                    {
                        var chat = Chats.Where(chat => chat.Id == chatId).FirstOrDefault();

                        if (chat is not null)
                        {
                            IncrementUnreadMessages(chat);
                        }
                    }
                });
            });

            _hubConnection.On<ChatResponse>("ReceiveChat", (chat) =>
            {
                if (_disconnected)
                {
                    return;
                }

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Chats.Add(ChatResponseDecorator.ToDecorator(chat));
                });
            });
        }

        private void IncrementUnreadMessages(ChatResponseDecorator chat)
        {
            chat.UnreadMessages++;

            int oldIndex = Chats.IndexOf(chat);
            Chats.Move(oldIndex, 0);
        }

        public ICommand NewChatCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand SelectChatCommand { get; set; }

        private void NewChat(object obj)
        {
            _window.NavigateToSettings(SettingsTab.Contacts);
        }

        private void Settings(object obj)
        {
            _window.NavigateToSettings(SettingsTab.Profile);
        }

        private void SelectChat(object obj)
        {
            if (obj is ChatResponseDecorator chat)
            {
                if (CurrentView is ChatPageViewModel chatPage)
                {
                    chatPage.Dispose();
                }

                SelectedChat = chat;

                SelectedChat.UnreadMessages = 0;

                CollectionViewSource.GetDefaultView(Chats).Refresh();

                CurrentView = new ChatPageViewModel(this, chat.Id);
            }
        }

        public void UpdateLastMessage(Guid chatId, MessageResponse message)
        {
            var chat = Chats.FirstOrDefault(chat => chat.Id == chatId);

            if (chat is not null)
            {
                chat.LastMessage = message.Text;
                chat.LastMessageTime = message.SendTime;

                CollectionViewSource.GetDefaultView(Chats).Refresh();
            }
        }

        public void Dispose()
        {
            _disconnected = true;

            var userId = AuthorizationProvider.GetUserId();

            // TODO: Handle exceptions
            _ = _hubConnection.InvokeAsync("LeaveHub", userId);
        }
    }
}
