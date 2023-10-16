using Messenger.Interfaces;
using Messenger.Models;
using Messenger.Utilities;
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

        private ObservableCollection<Chat> _chats;
        public ObservableCollection<Chat> Chats
        { 
            get { return _chats; }
            set { _chats = value; OnPropertyChanged(); }
        }

        private Chat _selectedChat;
        public Chat SelectedChat
        {
            get { return _selectedChat; }
            set { _selectedChat = value; OnPropertyChanged();}
        }

        public MainPageViewModel(IWindow window)
        {
            _window = window;

            SelectedChat = null;
            Chats = new ObservableCollection<Chat>
            {
                new Chat{ LastMessage = "Hello", LastMessageTime = DateTime.Now, Name = "Jack" },
                new Chat{ LastMessage = "No", LastMessageTime = DateTime.Now, Name = "John" },
                new Chat{ LastMessage = "Yes", LastMessageTime = DateTime.Now, Name = "Jane" },
            };
        }
    }
}
