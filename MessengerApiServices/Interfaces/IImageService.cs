using Microsoft.AspNetCore.Http;

namespace MessengerApiServices.Interfaces;

public interface IImageService
{
    Task<string> UploadImageAsync(Guid id, IFormFile image);
}
