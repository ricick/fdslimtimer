using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.model;

namespace SlimTimer.control
{
    class SaveSettingsCommand : SimpleCommand
    {
        public override void Execute(PureMVC.Interfaces.INotification notification)
        {
            base.Execute(notification);
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            settingsProxy.SaveSettings();
        }
    }
}
