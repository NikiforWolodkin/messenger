using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Exceptions
{
    public class LoggedOutException : Exception
    {
        public LoggedOutException(string message) : base(message)
        {
        }
    }
}
