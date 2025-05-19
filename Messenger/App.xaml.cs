using Messenger.Exceptions;
using Messenger.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Messenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Handle exceptions on the UI thread.
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // Handle exceptions on all other threads.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Handle HttpException differently
            if (e.Exception is HttpException httpEx)
            {
                if (httpEx.ErrorCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show($"Ошибка авторизации: {httpEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    AuthorizationProvider.LogOutAndGoToLogin();
                }
            }
            else if (e.Exception is LoggedOutException loggedOutEx)
            {
                MessageBox.Show($"Ошибка авторизации: {loggedOutEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                AuthorizationProvider.LogOutAndGoToLogin();
            }
            else
            {
                MessageBox.Show($"Ошибка: {e.Exception.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle HttpException differently
            if (e.ExceptionObject is HttpException httpEx)
            {
                if (httpEx.ErrorCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show($"Ошибка авторизации: {httpEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    AuthorizationProvider.LogOutAndGoToLogin();
                }
            }
            else if (e.ExceptionObject is LoggedOutException loggedOutEx)
            {
                MessageBox.Show($"Ошибка авторизации: {loggedOutEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                AuthorizationProvider.LogOutAndGoToLogin();
            }
            else
            {
                MessageBox.Show($"Ошибка: {(e.ExceptionObject as Exception).Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
