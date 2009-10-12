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
                    //log("Duplicate project found :" + checkTask.Name + " hours = " + checkTask.Hours);
                    Collection<TimeEntry> potentialEntries = apiProxy.Api.ListTimeEntries(checkTask.CreatedTime, checkTask.UpdatedTime);
                    foreach (TimeEntry checkEntry in potentialEntries)
                    {
                        if (checkEntry.RelatedTask.Id == checkTask.Id)
                        {
                            //log("Entry in duplicate project found :" + checkEntry.StartTime);
                            //set the timeentry to be associated with the original task and save it
                            checkEntry.RelatedTask = originalTask;
                            apiProxy.Api.UpdateTimeEntry(checkEntry);
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
