using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Views;

/// <summary>
/// Interaction logic for CalendarPage.xaml
/// </summary>
public partial class CalendarPage : UserControl
{
    public CalendarPage()
    {
        InitializeComponent();
    }

    private async void RemoveClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menu &&
            DataContext is CalendarPageViewModel vm)
        {
            await vm.RemoveAsync(menu.DataContext);
        }
    }
}