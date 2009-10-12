using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;

namespace SlimTimer.control
{
    class ResumeCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
        }

    }
}
