using Messenger.Exceptions;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Services
{
    public static class FilesService
    {
        public static async Task<string> UploadImageAsync(string filePath)
        {
            var result = await Api.PostFileAsync("images", filePath);

            if (!result.IsSuccessful)
            {
                throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
            }

            return result.Response;
        }
    }
}
