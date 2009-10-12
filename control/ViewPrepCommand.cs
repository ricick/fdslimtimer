using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.view;

namespace SlimTimer.control
{
    class ViewPrepCommand :SimpleCommand
    {
        public override void Execute(PureMVC.Interfaces.INotification notification)
        {
            base.Execute(notification);
            Facade.RegisterMediator(new PluginMainMediator(notification.Body as PluginMain));
        }
    }
}
