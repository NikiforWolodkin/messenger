using MessengerApiServices.Exceptions;
using MessengerModels.Models;
using System.Net;
using System.Text.Json;

namespace MessengerApi.Middleware
{
    internal class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleNotFoundException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleUnexpectedException(context, ex);
            }
        }

        public Task HandleNotFoundException(HttpContext context, NotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

            string result = JsonSerializer.Serialize(new ErrorResponse
            {
                Message = ex.Message,
            });

            return context.Response.WriteAsync(result);
        }

        public Task HandleUnexpectedException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

            string result = JsonSerializer.Serialize(new ErrorResponse
            {
                Message = ex.Message,
            });

            return context.Response.WriteAsync(result);
        }
    }
}
