using Messenger.Providers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Helpers
{
    public static class HubConnectionFactory
    {
        private static HubConnection? _chatHubConnection = null;
        private static HubConnection? _userChatsHubConnection = null;
        public static HubConnection CreateChatConnection()
        {
            if (_chatHubConnection is not null)
            {
                return _chatHubConnection;
            }

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var url = new Uri($"{apiUrl}/chat-hub");

            var token = AuthorizationProvider.GetToken();

            var connection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            _chatHubConnection = connection;

            return connection;
        }

        public static HubConnection CreateUserChatsConnection()
        {
            if (_userChatsHubConnection is not null)
            {
                return _userChatsHubConnection;
            }

            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            var url = new Uri($"{apiUrl}/user-chats-hub");

            var token = AuthorizationProvider.GetToken();

            var connection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            _userChatsHubConnection = connection;

            return connection;
        }
    }
}
