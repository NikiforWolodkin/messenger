using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Guid busStopId, IFormFile image);
    }
}
