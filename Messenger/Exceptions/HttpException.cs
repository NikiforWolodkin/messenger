using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Exceptions
{
    public class HttpException : Exception
    {
        public ErrorResponse Error { get; set; }
        public HttpException(ErrorResponse error) : base(error.Message)
        { 
            Error = error;
        }
    }
}
