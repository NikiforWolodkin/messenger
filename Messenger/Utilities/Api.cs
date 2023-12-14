using Messenger.Exceptions;
using Messenger.Providers;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Messenger.Utilities
{
    public static class Api
    {
        public static async Task<Result<TResponse>> GetAsync<TResponse>(string uri)
        {
            using var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.GetAsync($"{apiUrl}/{uri}");

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(result);

                return new Result<TResponse>(response);
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new LoggedOutException("You are logged out, please log in again.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<TResponse>(error, httpResponse.StatusCode);
            }
        }

        public static async Task<Result<TResponse>> PostAsync<TResponse, TBody>(string uri, TBody body)
        {
            using var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.PostAsync($"{apiUrl}/{uri}", data);

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(result);

                return new Result<TResponse>(response);
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new LoggedOutException("You are logged out, please log in again.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<TResponse>(error, httpResponse.StatusCode);
            }
        }

        public static async Task<Result<EmptyResult>> PostAsync<TBody>(string uri, TBody body)
        {
            using var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.PostAsync($"{apiUrl}/{uri}", data);

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                return new Result<EmptyResult>(new EmptyResult());
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new LoggedOutException("You are logged out, please log in again.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<EmptyResult>(error, httpResponse.StatusCode);
            }
        }

        public static async Task<Result<TResponse>> PutAsync<TResponse, TBody>(string uri, TBody body)
        {
            using var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.PutAsync($"{apiUrl}/{uri}", data);

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(result);

                return new Result<TResponse>(response);
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new LoggedOutException("You are logged out, please log in again.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<TResponse>(error, httpResponse.StatusCode);
            }
        }

        public static async Task<Result<string>> PostFileAsync(string uri, string filePath)
        {
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var fileBytes = File.ReadAllBytes(filePath);
            var byteContent = new ByteArrayContent(fileBytes);
            content.Add(byteContent, "image", Path.GetFileName(filePath)); // Add file data to form data

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.PostAsync($"{apiUrl}/{uri}", content); // Send form data

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                return new Result<string>(result);
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new LoggedOutException("You are logged out, please log in again.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<string>(error, httpResponse.StatusCode);
            }
        }

        public static async Task<Result<EmptyResult>> DeleteAsync(string uri)
        {
            using var client = new HttpClient();

            var token = AuthorizationProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var httpResponse = await client.DeleteAsync($"{apiUrl}/{uri}");

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                return new Result<EmptyResult>(new EmptyResult());
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new LoggedOutException("You are logged out, please log in again.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(result);

                return new Result<EmptyResult>(error, httpResponse.StatusCode);
            }
        }
    }
}
