using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Exceptions
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(string message) : base(message)
        {
        }
    }
}
