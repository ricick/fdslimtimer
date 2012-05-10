using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;
using System.Threading;

namespace SlimTimer.control
{
    class SaveTimeEntryCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Thread worker = new Thread(DoSaveTimeEntry);
            worker.IsBackground = true;
            worker.Start();

        }
        private void DoSaveTimeEntry()
        {
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;

            TimeEntry timeEntry = taskProxy.CurrentTimeEntry;
            Console.WriteLine("submitTimeEntry " + timeEntry);
            if (!statusProxy.LoggedIn)
            {
                Console.WriteLine("notlogged in");
                return;
            }
            if (timeEntry == null)
            {
                Console.WriteLine("no time entry");
                return;
            }
            if (timeEntry.RelatedTask == null || timeEntry.RelatedTask.Id == null || timeEntry.RelatedTask.Id.Length == 0)
            {
                Console.WriteLine("no task to submit to");
                return;
            }
            DateTime startTime = timeEntry.StartTime;
            timeEntry.EndTime = DateTime.Now;
            timeEntry.Comments = taskProxy.Comments;
            //Console.WriteLine("timeEntry.EndTime = " + timeEntry.EndTime);
            TimeSpan duration = timeEntry.EndTime.Subtract(timeEntry.StartTime);
            //Console.WriteLine("timeEntry.StartTime = " + timeEntry.StartTime);
            //Console.WriteLine("duration = " + duration);
            //int minimumTime = minimumTime;
            if (settingsProxy.MinimumTime < 1) settingsProxy.MinimumTime = 1;
            timeEntry.Duration = Convert.ToInt32(Math.Floor(duration.TotalSeconds));
            if (duration.TotalSeconds < settingsProxy.MinimumTime)
            {
                Console.WriteLine("not enough seconds to submit");
                return;
            }
            try
            {
                timeEntry = apiProxy.Api.UpdateTimeEntry(timeEntry);
                //replace timeentry with result
                //Console.WriteLine("timeEntry submitted " + timeEntry.Id);
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
            taskProxy.CurrentTimeEntry = timeEntry;
        }

    }
}
