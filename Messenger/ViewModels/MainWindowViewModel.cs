using Messenger.Models;
using Messenger.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Chat> _chats;
        public ObservableCollection<Chat> Chats
        { 
            get { return _chats; }
            set { _chats = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            Chats = new ObservableCollection<Chat> 
            {
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
                new Chat{ LastMessage = "Test message", LastMessageTime = DateTime.Now, Name = "Test user" },
            };
        }
    }
}
