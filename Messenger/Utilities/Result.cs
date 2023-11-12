using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Utilities
{
    public class Result<T>
    {
        public bool IsSuccessful { get; set; }
        public T? Response { get; set; }
        public ErrorResponse? Error { get; set; }
        public HttpStatusCode? StatusCode { get; set; }

        public Result(T response)
        {
            IsSuccessful = true;
            Response = response;
        }

        public Result(ErrorResponse error, HttpStatusCode statusCode)
        {
            IsSuccessful = false;
            Error = error;
            StatusCode = statusCode;
        }
    }

    public record struct EmptyResult;
}
