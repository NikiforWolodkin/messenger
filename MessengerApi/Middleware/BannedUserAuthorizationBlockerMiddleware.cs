using MessengerApi.Helpers;
using MessengerApiDomain.Models;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MessengerApi.Middleware
{
    public class BannedUserAuthorizationBlockerMiddleware
    {
        private readonly RequestDelegate _next;

        public BannedUserAuthorizationBlockerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);

                return;
            }

            var id = JwtClaimsHelper.GetId(context.User.Identity);

            var user = await userService.GetByIdAsync(id);

            if (user.IsBanned)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

                string result = JsonSerializer.Serialize(new ErrorResponse
                {
                    Message = "Вы были заблокированы и не можете пользоваться сервисом.",
                });

                await context.Response.WriteAsync(result);

                return;
            }

            await _next(context);
        }
    }
}
