﻿using Messenger.Helpers;
using Messenger.Interfaces;
using Messenger.Providers;
using Messenger.Services;
using Messenger.Utilities;
using MessengerApiDomain.Models;
using MessengerModels.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace Messenger.ViewModels
{
    public class ChatPageViewModel : ViewModelBase, IDisposable
    {
        private readonly IMainPage _mainPage;
        private readonly HubConnection _hubConnection;

        private bool _disconnected = false;
        private string? _imageUrl = null;

        private int? _selfDeletionTime = null;

        public Guid ChatId { get; set; }

        private bool _isBlacklisted;
        public bool IsBlacklisted
        {
            get { return _isBlacklisted; }
            set { _isBlacklisted = value; OnPropertyChanged(); }
        }

        private ObservableCollection<MessageResponseDecorator> _messages;
        public ObservableCollection<MessageResponseDecorator> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }

        private MessageResponseDecorator _selectedItem;
        public MessageResponseDecorator SelectedItem
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

        private bool _isSelfDeletionTimeSet;
        public bool IsSelfDeletionTimeSet
        {
            get { return _isSelfDeletionTimeSet; }
            set { _isSelfDeletionTimeSet = value; OnPropertyChanged(); }
        }

        public ChatPageViewModel(IMainPage mainPage, ChatResponse chat)
        {
            _mainPage = mainPage;
            _hubConnection = HubConnectionFactory.CreateChatConnection();

            ChatId = chat.Id;
            IsBlacklisted = chat.IsBlacklisted ?? false;

            SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
            AddAttachmentCommand = new AsyncRelayCommand(AddAttachmentAsync);
            DownloadAttachmentCommand = new AsyncRelayCommand(DownloadAttachmentAsync);
            RemoveAttachmentCommand = new AsyncRelayCommand(RemoveAttachmentAsync);
            LikeMessageCommand = new AsyncRelayCommand(LikeMessageAsync);
            SetSelfDeletionTimeCommand = new AsyncRelayCommand(SetSelfDeletionTimeAsync);

            Text = string.Empty;
            SelectedItem = null;
            Messages = new ObservableCollection<MessageResponseDecorator>();
            IsAttachmentAdded = false;
            AttachmentName = string.Empty;

            GetMessagesAsync();
            ConnectToHubAsync();
        }
        private async Task GetMessagesAsync()
        {
            var messages = await MessagesService.GetAllAsync(ChatId);

            Messages = [.. MessageResponseDecorator.ToDecorator(messages)];
            SelectedItem = MessageResponseDecorator.ToDecorator(messages.Last());
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
                    Messages.Add(MessageResponseDecorator.ToDecorator(message));

                    SelectedItem = MessageResponseDecorator.ToDecorator(message);
                    Text = string.Empty;

                    _mainPage.UpdateLastMessage(ChatId, message);
                });
            });
        }

        public ICommand SendMessageCommand { get; set; }
        public ICommand AddAttachmentCommand { get; set; }
        public ICommand DownloadAttachmentCommand { get; set; }
        public ICommand RemoveAttachmentCommand { get; set; }
        public ICommand LikeMessageCommand { get; set; }
        public ICommand SetSelfDeletionTimeCommand { get; set; }

        private async Task SendMessageAsync(object obj)
        {
            MessageResponse message;

            if (_imageUrl is not null)
            {
                message = await MessagesService.AddImageMessageAsync(ChatId, _imageUrl, _selfDeletionTime);
            }
            else
            {
                message = await MessagesService.AddAsync(ChatId, Text, _selfDeletionTime);
            }

            Messages.Add(MessageResponseDecorator.ToDecorator(message));

            SelectedItem = MessageResponseDecorator.ToDecorator(message);
            Text = string.Empty;
            IsAttachmentAdded = false;
            _imageUrl = null;
            _selfDeletionTime = null;
            IsSelfDeletionTimeSet = false;

            _mainPage.UpdateLastMessage(ChatId, message);
        }

        private async Task AddAttachmentAsync(object obj)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var imageUrl = await FilesService.UploadImageAsync(openFileDialog.FileName);

                _imageUrl = imageUrl;

                IsAttachmentAdded = true;
                AttachmentName = Path.GetFileName(openFileDialog.FileName);
            }
        }

        private async Task RemoveAttachmentAsync(object obj)
        {
            _imageUrl = null;
            IsAttachmentAdded = false;
        }

        private async Task DownloadAttachmentAsync(object obj)
        {
            if (obj is string url)
            {
                try
                {
                    using var client = new HttpClient();

                    var fileName = Path.GetFileName(new Uri(url).AbsolutePath);

                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = fileName; // Default file name
                    saveFileDialog.DefaultExt = Path.GetExtension(fileName); // Default file extension
                    saveFileDialog.Filter = "All files (*.*)|*.*"; // Filter files by extension

                    // Show save file dialog box
                    var result = saveFileDialog.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        var downloadPath = saveFileDialog.FileName;
                        var fileBytes = await client.GetByteArrayAsync(url);
                        await System.IO.File.WriteAllBytesAsync(downloadPath, fileBytes);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task ReportMessageAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                var result = MessageBox.Show("Вы уверены, что хотите пожаловаться на это сообщение?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await MessagesService.AddReportAsync(message.Id);
                }
            }
        }

        public async Task DeleteMessageAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить это сообщение?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await MessagesService.DeleteMessageAsync(message.Id);

                        message.Text = "[Удаленное сообщение]";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async Task LikeMessageAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                message.IsLiked = !message.IsLiked;
                message.LikeAmount += message.IsLiked ? 1 : -1;

                await MessagesService.LikeAsync(message.Id);
            }
        }

        private async Task SetSelfDeletionTimeAsync(object param)
        {
            if (param is null)
            {
                _selfDeletionTime = null;
                IsSelfDeletionTimeSet = false;

                return;
            }

            // TODO: This is bad but I couldn't be bothered
            if (int.TryParse(param as string, out var time))
            {
                _selfDeletionTime = time;
                IsSelfDeletionTimeSet = true;
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
