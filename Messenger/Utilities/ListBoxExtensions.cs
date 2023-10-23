using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Messenger.Utilities
{
    public static class ListBoxExtensions
    {
        public static readonly DependencyProperty AutoScrollToEndProperty = DependencyProperty.RegisterAttached(
            "AutoScrollToEnd",
            typeof(bool),
            typeof(ListBoxExtensions),
            new PropertyMetadata(default(bool), AutoScrollToEndChanged)
        );

        private static void AutoScrollToEndChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependency is ListBox listBox && eventArgs.NewValue is bool newValue && newValue)
            {
                listBox.SelectionChanged += (s, _) => listBox.ScrollIntoView(listBox.SelectedItem);
            }
        }

        public static void SetAutoScrollToEnd(ListBox element, bool value)
        {
            element.SetValue(AutoScrollToEndProperty, value);
        }

        public static bool GetAutoScrollToEnd(ListBox element)
        {
            return (bool)element.GetValue(AutoScrollToEndProperty);
        }
    }
}
