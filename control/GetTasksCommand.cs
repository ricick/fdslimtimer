using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;
using System.Threading;
using PluginCore;

namespace SlimTimer.control
{
    class GetTasksCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Thread worker = new Thread(DoGetTasks);
            worker.IsBackground = true;
            worker.Start();
            
        }
        private void DoGetTasks(){
            Console.WriteLine("Loading tasks");
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            SlimTimerApi api = apiProxy.Api;
            //log("loadTasks");
            statusProxy.StatusText = "Loading projects from Slimtimer";
            try
            {
                taskProxy.Tasks= api.ListTasks(SlimTimerApi.ShowCompletedTask.No, SlimTimerApi.TaskFilters.Owner);
                statusProxy.LoadedTasks = true;

                statusProxy.StatusText = ("Logged in as " + settingsProxy.Username);
            }
            catch (Exception exception)
            {
                statusProxy.StatusText = ("Error loading tasks for" + settingsProxy.Username);
                Console.WriteLine("Error loading tasks for" + settingsProxy.Username + " : " + exception.Message);
                /*
                log("exception.Source : " + exception.Source);
                log("exception.StackTrace : " + exception.StackTrace);
                log("exception.Message : " + exception.Message);
                log("exception.Data : " + exception.Data);
                log("Error loading tasks for" + username + " : " + exception.Message);
                 * */
            }
            if (settingsProxy.CleanupDuplicates)
            {
                SendNotification(ApplicationFacade.CLEANUP_DUPLICATES);
            }
            else if (settingsProxy.CleanupOverlaps)
            {
                SendNotification(ApplicationFacade.CLEANUP_OVERLAPS);
            }
            //force change to current project if set
            if(statusProxy.LoadedTasks)SendNotification(ApplicationFacade.CHANGE_PROJECT, PluginBase.CurrentProject);
            //ui.setTasks(tasks);
            //findCurrentTask();
            SendNotification(ApplicationFacade.SET_CURRENT_TASK, taskProxy.CurrentTask);
            SendNotification(ApplicationFacade.RESET_TIMEOUT);
        }

    }
}
