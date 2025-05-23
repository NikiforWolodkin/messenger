﻿using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : UserControl
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private async void ReportClicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menu &&
                DataContext is ChatPageViewModel vm)
            {
                await vm.ReportMessageAsync(menu.DataContext);
            }
        }

        private async void DeleteClicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menu &&
                DataContext is ChatPageViewModel vm)
            {
                await vm.DeleteMessageAsync(menu.DataContext);
            }
        }
    }
}
