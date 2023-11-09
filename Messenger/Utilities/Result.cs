using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Utilities
{
    public class Result<T>
    {
        public bool IsSuccessful { get; set; }
        public T? Response { get; set; }
        public ErrorResponse? Error { get; set; }

        public Result(T response)
        {
            IsSuccessful = true;
            Response = response;
        }

        public Result(ErrorResponse error)
        {
            IsSuccessful = false;
            Error = error;
        }
    }

    public record struct EmptyResult;
}
