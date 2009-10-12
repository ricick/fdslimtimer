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

            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;

            taskProxy.Comments = "";
            //The required fields are StartTime, EndTime, Duration, and RelatedTask.
            TimeEntry timeEntry = new TimeEntry(taskProxy.CurrentTask);
            timeEntry.StartTime = DateTime.Now;
        }

    }
}
