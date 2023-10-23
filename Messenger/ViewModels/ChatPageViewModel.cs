using Messenger.Interfaces;
using Messenger.Services;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class ChatPageViewModel : ViewModelBase
    {
        private readonly IMainPage _mainPage;

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

            ChatId = chatId;

            SendMessageCommand = new AsyncRelayCommand(SendMessage);

            Text = string.Empty;
            SelectedItem = null;
            Messages = new ObservableCollection<MessageResponse>();

            GetMessagesAsync();
        }
        private async Task GetMessagesAsync()
        {
            var messages = await MessagesService.GetAllAsync(ChatId);

            Messages = new ObservableCollection<MessageResponse>(messages);
            SelectedItem = messages.Last();
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
    }
}
