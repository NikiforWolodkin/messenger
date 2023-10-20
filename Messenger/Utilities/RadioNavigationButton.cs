using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Utilities
{
    public class RadioNavigationButton : RadioButton
    {
        static RadioNavigationButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioNavigationButton), 
                new FrameworkPropertyMetadata(typeof(RadioNavigationButton)));
        }
    }
}
