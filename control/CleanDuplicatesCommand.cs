using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using System.Collections;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;

namespace SlimTimer.control
{
    class CleanDuplicatesCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;

            Collection<Task> tasks = taskProxy.Tasks;

            ArrayList checkedProjects = new ArrayList();
            Task originalTask = new Task("null");
            foreach (Task checkTask in tasks)
            {
                bool found = false;
                foreach (Task compareTask in checkedProjects)
                {
                    if (compareTask.Name == checkTask.Name)
                    {
                        found = true;
                        originalTask = compareTask;
                        break;
                    }
                }
                if (found)
                {
                    //move entries to original and delete duplicate
                    //api;
                    if (checkTask.Id == originalTask.Id)
                    {

                        Console.WriteLine("Not really a duplicate project found :" + checkTask.Name);
                        continue;
                    }
                    Console.WriteLine("Duplicate project found :" + checkTask.Name + " hours = " + checkTask.Hours);
                    Collection<TimeEntry> potentialEntries = apiProxy.Api.ListTaskTimeEntries(checkTask.Id, checkTask.CreatedTime, checkTask.UpdatedTime);
                    //Collection<TimeEntry> potentialEntries = apiProxy.Api.ListTaskTimeEntries(checkTask.Id, new DateTime(0), new DateTime());
                    if (potentialEntries.Count == 0 && checkTask.Hours>0)
                    {

                        Console.WriteLine("Problem getting entries for " + checkTask.Name);
                        continue;
                    }
                    DateTime startTime = new DateTime(0);
                    foreach (TimeEntry checkEntry in potentialEntries)
                    {
                        if (checkEntry.RelatedTask.Id == checkTask.Id)
                        {
                            Console.WriteLine("Entry in duplicate project found :" + checkEntry.StartTime);
                            //set the timeentry to be associated with the original task and save it
                            if (checkEntry.StartTime.CompareTo(startTime) == 0)
                            {
                                Console.WriteLine("Duplicate timeentry ignoring :" + checkEntry.StartTime);
                            }
                            checkEntry.RelatedTask = originalTask;
                            apiProxy.Api.UpdateTimeEntry(checkEntry);
                            startTime = checkEntry.StartTime;
                        }
                    }
                    //delete the task
                    apiProxy.Api.DeleteTask(checkTask.Id);

                }
                else
                {
                    checkedProjects.Add(checkTask);
                }
            }
        }

    }
}
