using Messenger.Exceptions;
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
        private static string? _token = null;

        public static string GetToken()
        {
            if (_token is null)
            {
                throw new LoggedOutException("You are logged out. Please log in again.");
            }

            return _token;
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

            var response = await client.PostAsync($"{apiUrl}/api/authorize", data);

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _token = result;
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

            var response = await client.PostAsync($"{apiUrl}/api/users", data);

            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                throw new AuthorizationFailedException(error.Message);
            }
        }
    }
}
