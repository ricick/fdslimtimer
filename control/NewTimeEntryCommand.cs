using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;

namespace SlimTimer.control
{
    class NewTimeEntryCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Console.WriteLine("NewTimeEntryCommand.Execute");
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            TimerProxy timerProxy = Facade.RetrieveProxy(TimerProxy.NAME) as TimerProxy;

            taskProxy.Comments = "";
            //The required fields are StartTime, EndTime, Duration, and RelatedTask.
            TimeEntry timeEntry = new TimeEntry(taskProxy.CurrentTask);
            timeEntry.StartTime = DateTime.Now;
            taskProxy.CurrentTimeEntry = timeEntry;
            timerProxy.Timer.Start();
        }

    }
}
