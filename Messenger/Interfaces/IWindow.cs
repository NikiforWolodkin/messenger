﻿using Messenger.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Interfaces
{
    public interface IWindow
    {
        void NavigateToLogin();
        void NavigateToSignup();
        void NavigateToMain();
        void NavigateToSettings(SettingsTab tab);
        void NavigateToCalendar();
        void NavigateToNewEvent();
    }
}
