using Messenger.Exceptions;
using Messenger.Interfaces;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messenger.Providers
{
    public static class AuthorizationProvider
    {
        private static IWindow? _window = null;

        private static string? _token = null;
        private static Guid? _userId = null;
        private static bool? _isUserAdmin = null;

        public static string GetToken()
        {
            if (_token is null)
            {
                throw new LoggedOutException("время сессии истекло, пожалуйста, авторизируйтесь снова.");
            }

            return _token;
        }

        public static Guid GetUserId()
        {
            if (_userId is null)
            {
                throw new LoggedOutException("время сессии истекло, пожалуйста, авторизируйтесь снова.");
            }

            return (Guid)_userId;
        }

        public static bool GetIsUserAdmin()
        {
            if (_isUserAdmin is null)
            {
                throw new LoggedOutException("время сессии истекло, пожалуйста, авторизируйтесь снова.");
            }

            return (bool)_isUserAdmin;
        }

        public static async Task LogInAsync(string name, string password)
        {
            var client = new HttpClient();

            var request = new UserLogInRequest
            {
                Name = name,
                Password = password
            };

            var json = JsonSerializer.Serialize(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var response = await client.PostAsync($"{apiUrl}/authorize", data);

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var authResponse = JsonSerializer.Deserialize<AuthorizationResponse>(result);

                _token = authResponse.AuthorizationToken;
                _userId = authResponse.Id;
                _isUserAdmin = authResponse.IsAdmin;
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                throw new AuthorizationFailedException(error.Message);
            }
        }

        public static async Task SignUpAsync(string name, string password)
        {
            var client = new HttpClient();

            var request = new UserSignUpRequest
            {
                Name = name,
                Password = password
            };

            var json = JsonSerializer.Serialize(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var response = await client.PostAsync($"{apiUrl}/users", data);

            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                throw new AuthorizationFailedException(error.Message);
            }
        }

        public static void LogOut()
        {
            _token = null;
            _userId = null;
            _isUserAdmin = null;
        }

        public static void SetWindow(IWindow window)
        {
            _window = window;
        }

        public static void LogOutAndGoToLogin()
        {
            _token = null;
            _userId = null;
            _isUserAdmin = null;

            _window?.NavigateToLogin();
        }
    }
}
