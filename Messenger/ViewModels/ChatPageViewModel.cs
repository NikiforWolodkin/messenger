using Messenger.Helpers;
using Messenger.Interfaces;
using Messenger.Providers;
using Messenger.Services;
using Messenger.Utilities;
using MessengerModels.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class ChatPageViewModel : ViewModelBase, IDisposable
    {
        private readonly IMainPage _mainPage;
        private readonly HubConnection _hubConnection;

        private bool _disconnected = false;
        private string? _imageUrl = null;

        public Guid ChatId { get; set; }

        private bool _isBlacklisted;
        public bool IsBlacklisted
        {
            get { return _isBlacklisted; }
            set { _isBlacklisted = value; OnPropertyChanged(); }
        }

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

        private string _attachmentName;
        public string AttachmentName
        {
            get { return _attachmentName; }
            set { _attachmentName = value; OnPropertyChanged(); }
        }

        private bool _isAttachmentAdded;
        public bool IsAttachmentAdded
        {
            get { return _isAttachmentAdded; }
            set { _isAttachmentAdded = value; OnPropertyChanged(); }
        }

        public ChatPageViewModel(IMainPage mainPage, ChatResponse chat)
        {
            _mainPage = mainPage;
            _hubConnection = HubConnectionFactory.CreateChatConnection();

            ChatId = chat.Id;
            IsBlacklisted = (bool)chat.IsBlacklisted;

            SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
            AddAttachmentCommand = new AsyncRelayCommand(AddAttachmentAsync);
            RemoveAttachmentCommand = new AsyncRelayCommand(RemoveAttachmentAsync);
            ReportMessageCommand = new AsyncRelayCommand(ReportMessageAsync);

            Text = string.Empty;
            SelectedItem = null;
            Messages = new ObservableCollection<MessageResponse>();
            IsAttachmentAdded = false;
            AttachmentName = string.Empty;

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
        public ICommand AddAttachmentCommand { get; set; }
        public ICommand RemoveAttachmentCommand { get; set; }
        public ICommand ReportMessageCommand { get; set; }

        private async Task SendMessageAsync(object obj)
        {
            MessageResponse message;

            if (_imageUrl is not null)
            {
                message = await MessagesService.AddImageMessageAsync(ChatId, _imageUrl);
            }
            else
            {
                message = await MessagesService.AddAsync(ChatId, Text);
            }

            Messages.Add(message);

            SelectedItem = message;
            Text = string.Empty;
            IsAttachmentAdded = false;
            _imageUrl = null;

            _mainPage.UpdateLastMessage(ChatId, message);
        }

        private async Task AddAttachmentAsync(object obj)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var extension = Path.GetExtension(openFileDialog.FileName).ToLower();

                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    var imageUrl = await FilesService.UploadImageAsync(openFileDialog.FileName);

                    _imageUrl = imageUrl;

                    IsAttachmentAdded = true;
                    AttachmentName = Path.GetFileName(openFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("Only PNG and JPEG files are allowed.", "Incorrect file format");
                }
            }
        }

        private async Task RemoveAttachmentAsync(object obj)
        {
            _imageUrl = null;
            IsAttachmentAdded = false;
        }

        private async Task ReportMessageAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                var result = MessageBox.Show("Are you sure you want to report this message?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await MessagesService.AddReportAsync(message.Id);
                }
            }
        }

        public void Dispose()
        {
            _disconnected = true;

            // TODO: Handle exceptions
            _ = _hubConnection.InvokeAsync("LeaveChat", ChatId);
        }
    }
}
