using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Interfaces
{
    public interface IMainPage
    {
        void UpdateLastMessage(Guid chatId, MessageResponse message);
    }
}
