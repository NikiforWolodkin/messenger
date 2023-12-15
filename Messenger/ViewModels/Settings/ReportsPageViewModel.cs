using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Messenger.Services;
using Messenger.Utilities;
using MessengerApiDomain.Models;
using MessengerModels.Models;

namespace Messenger.ViewModels.Settings
{
    public class ReportsPageViewModel : ViewModelBase
    {
        private ObservableCollection<MessageResponse> _messages;
        public ObservableCollection<MessageResponse> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }

        public ReportsPageViewModel()
        {
            DismissReportCommand = new AsyncRelayCommand(DismissReportAsync);
            DeleteMessageCommand = new AsyncRelayCommand(DeleteMessageAsync);
            DeleteAndBanUserCommand = new AsyncRelayCommand(DeleteAndBanUserAsync);

            Messages = new ObservableCollection<MessageResponse>();

            GetMessagesAsync();
        }

        private async Task GetMessagesAsync()
        {
            var messages = await MessagesService.GetReportedMessagesAsync();

            Messages = new ObservableCollection<MessageResponse>(messages);
        }

        public ICommand DismissReportCommand { get; set; }
        public ICommand DeleteMessageCommand { get; set; }
        public ICommand DeleteAndBanUserCommand { get; set; }
        
        private async Task DismissReportAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                await MessagesService.DeleteReportAsync(message.Id);

                Messages.Remove(message);
            }
        }

        private async Task DeleteMessageAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                var result = MessageBox.Show("Are you sure you want to delete this message?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await MessagesService.DeleteMessageAsync(message.Id);

                    Messages.Remove(message);
                }
            }
        }

        private async Task DeleteAndBanUserAsync(object obj)
        {
            if (obj is MessageResponse message)
            {
                var result = MessageBox.Show("Are you sure you want to ban this user and delete their messages?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await UsersService.BanAndDeleteMessagesAsync(message.Id);

                    await GetMessagesAsync();
                }
            }
        }

    }
}
