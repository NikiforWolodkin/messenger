using Messenger.Helpers;
using Messenger.Interfaces;
using Messenger.Providers;
using Messenger.Services;
using Messenger.Utilities;
using MessengerModels.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class ChatPageViewModel : ViewModelBase, IDisposable
    {
        private readonly IMainPage _mainPage;
        private readonly HubConnection _hubConnection;

        private bool _disconnected = false;

        public Guid ChatId { get; set; }

        private ObservableCollection<MessageResponse> _messages;
        public ObservableCollection<MessageResponse> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }

        private MessageResponse _selectedItem;
        public MessageResponse SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }

        public ChatPageViewModel(IMainPage mainPage, Guid chatId)
        {
            _mainPage = mainPage;
            _hubConnection = HubConnectionFactory.CreateChatConnection();

            ChatId = chatId;

            SendMessageCommand = new AsyncRelayCommand(SendMessage);

            Text = string.Empty;
            SelectedItem = null;
            Messages = new ObservableCollection<MessageResponse>();

            GetMessagesAsync();
            ConnectToHubAsync();
        }
        private async Task GetMessagesAsync()
        {
            var messages = await MessagesService.GetAllAsync(ChatId);

            Messages = new ObservableCollection<MessageResponse>(messages);
            SelectedItem = messages.Last();
        }

        private async Task ConnectToHubAsync()
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await _hubConnection.StartAsync();
            }

            await _hubConnection.InvokeAsync("JoinChat", ChatId);

            _hubConnection.On<MessageResponse>("ReceiveMessage", (message) =>
            {
                if (_disconnected)
                {
                    return;
                }

                if (message.AuthorId == AuthorizationProvider.GetUserId())
                {
                    return;
                }

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Messages.Add(message);

                    SelectedItem = message;
                    Text = string.Empty;

                    _mainPage.UpdateLastMessage(ChatId, message);
                });
            });
        }

        public ICommand SendMessageCommand { get; set; }

        private async Task SendMessage(object obj)
        {
            var message = await MessagesService.AddAsync(ChatId, Text);

            Messages.Add(message);

            SelectedItem = message;
            Text = string.Empty;

            _mainPage.UpdateLastMessage(ChatId, message);
        }

        public void Dispose()
        {
            _disconnected = true;

            // TODO: Handle exceptions
            _ = _hubConnection.InvokeAsync("LeaveChat", ChatId);
        }
    }
}
