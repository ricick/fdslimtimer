using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SlimTimer.model;
using System.Collections.ObjectModel;
using System.Collections;
using Inikus.SlimTimer;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace SlimTimer.control
{
    class CleanOverlapsCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Thread worker = new Thread(DoCleanOverlaps);
            worker.IsBackground = true;
            worker.Start();

        }
        private void DoCleanOverlaps()
        {
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;

            Collection<Task> tasks = taskProxy.Tasks;

            Console.WriteLine("Cleaning overlapped entries");

            foreach (Task checkTask in tasks)
            {
                Console.WriteLine("Checking overlap entries for " + checkTask.Name);
                Collection<TimeEntry> potentialEntries = apiProxy.Api.ListTaskTimeEntries(checkTask.Id, checkTask.CreatedTime, checkTask.UpdatedTime);
                if (potentialEntries == null || (potentialEntries.Count == 0 && checkTask.Hours > 0))
                {

                    Console.WriteLine("Problem getting entries for " + checkTask.Name);
                    continue;
                }
                Hashtable entryHash = new Hashtable();
                foreach (TimeEntry entry in potentialEntries)
                {
                    TimeEntry overlap = (TimeEntry)entryHash[entry.StartTime];
                    TimeEntry entryToHash = entry;
                    if (overlap != null)
                    {
                        Console.WriteLine("Found overlap for " + checkTask.Name);
                        //keep the longest, discard the other
                        Console.WriteLine("overlap.Duration : " + overlap.Duration + " vs entry.Duration : " + entry.Duration + "");
                        if (overlap.Duration > entry.Duration)
                        {
                            apiProxy.Api.DeleteTimeEntry(entry.Id);
                            entryToHash = overlap;
                            Console.WriteLine("Keeping original");
                        }
                        else
                        {
                            apiProxy.Api.DeleteTimeEntry(overlap.Id);
                            Console.WriteLine("Keeping new");
                        }
                    }
                    entryHash[entry.StartTime] = entryToHash;
                }
            }
            Console.WriteLine("Done overlap cleaning");
        }
    }
}
