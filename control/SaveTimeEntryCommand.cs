using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;

namespace SlimTimer.control
{
    class SaveTimeEntryCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;

            TimeEntry timeEntry = taskProxy.CurrentTimeEntry;
            //log("submitTimeEntry");
            if (!statusProxy.LoggedIn)
            {
                //log("notlogged in");
                return;
            }
            if (timeEntry == null)
            {
                //log("no time entry");
                return;
            }
            if (timeEntry.RelatedTask == null || timeEntry.RelatedTask.Id == null || timeEntry.RelatedTask.Id.Length == 0)
            {
                //log("no task to submit to");
                return;
            }
            DateTime startTime = timeEntry.StartTime;
            timeEntry.EndTime = DateTime.Now;
            timeEntry.Comments = taskProxy.Comments;
            //log("timeEntry.EndTime = " + timeEntry.EndTime);
            TimeSpan duration = timeEntry.EndTime.Subtract(timeEntry.StartTime);
            //log("timeEntry.StartTime = " + timeEntry.StartTime);
            //log("duration = " + duration);
            //int minimumTime = minimumTime;
            if (settingsProxy.MinimumTime < 1) settingsProxy.MinimumTime = 1;
            timeEntry.Duration = Convert.ToInt32(Math.Floor(duration.TotalSeconds));
            if (duration.TotalSeconds < settingsProxy.MinimumTime)
            {
                //log("not enough seconds to submit");
                return;
            }
            try
            {
                timeEntry = apiProxy.Api.UpdateTimeEntry(timeEntry);
                //replace timeentry with result
                //log("timeEntry submitted " + timeEntry.Id);
            }
            catch (Exception exception)
            {
                statusProxy.StatusText = ("Error submitting time entry for " + settingsProxy.Username);
                Console.WriteLine("exception.Source : " + exception.Source);
                Console.WriteLine("exception.StackTrace : " + exception.StackTrace);
                Console.WriteLine("exception.Message : " + exception.Message);
                Console.WriteLine("exception.Data : " + exception.Data);
                Console.WriteLine("Error submitting time entry for " + settingsProxy.Username + " : " + exception.Message);
            }
            //set start time back to cached value (submitting returns server time)
            timeEntry.StartTime = startTime;
        }

    }
}
