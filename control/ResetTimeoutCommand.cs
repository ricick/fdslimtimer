using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.model;
using PureMVC.Interfaces;

namespace SlimTimer.control
{
    class ResetTimeoutCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            TimerProxy timerProxy = Facade.RetrieveProxy(TimerProxy.NAME) as TimerProxy;
            timerProxy.ResetTimeOut();
        }
    }
}
