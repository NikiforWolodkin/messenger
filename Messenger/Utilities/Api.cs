using Messenger.Exceptions;
using Messenger.Providers;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Messenger.Utilities
{
    public static class Api
    {
        public static async Task<Result<TResponse>> GetAsync<TResponse>(string uri)
        {
            var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.GetAsync($"{apiUrl}/{uri}");

            string result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(result);

                return new Result<TResponse>(response);
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<TResponse>(error);
            }
        }

        public static async Task<Result<TResponse>> PostAsync<TResponse, TBody>(string uri, TBody body)
        {
            var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.PostAsync($"{apiUrl}/{uri}", data);

            string result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(result);

                return new Result<TResponse>(response);
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<TResponse>(error);
            }
        }
    }
}
