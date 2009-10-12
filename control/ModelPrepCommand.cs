using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.model;
using PureMVC.Interfaces;
namespace SlimTimer.control
{
    class ModelPrepCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Console.WriteLine(notification.ToString());
            Facade.RegisterProxy(new SettingsProxy());
            Facade.RegisterProxy(new TaskProxy());
            Facade.RegisterProxy(new APIProxy());
        }
    }
}
