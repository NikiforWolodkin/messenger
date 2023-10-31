using MessengerApi.Helpers;
using MessengerApiServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApi.Controllers
{
    [Authorize]
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageAsync(IFormFile image)
        {
            var id = JwtClaimsHelper.GetId(User.Identity);

            // TODO: Check if users exist

            var imageName = await _imageService.UploadImageAsync(id, image);

            return Ok($"http://127.0.0.1:10000/devstoreaccount1/messenger-container/{imageName}");
        }
    }
}
