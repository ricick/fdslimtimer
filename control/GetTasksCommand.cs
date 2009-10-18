using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;

namespace SlimTimer.control
{
    class GetTasksCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            SlimTimerApi api = apiProxy.Api;
            base.Execute(notification);
            //log("loadTasks");
            try
            {
                taskProxy.Tasks= api.ListTasks(SlimTimerApi.ShowCompletedTask.No, SlimTimerApi.TaskFilters.Owner);
                statusProxy.LoadedTasks = true;
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
            if (settingsProxy.CleanupDuplicates) SendNotification(ApplicationFacade.CLEANUP_DUPLICATES);
            //ui.setTasks(tasks);
            //findCurrentTask();
            SendNotification(ApplicationFacade.SET_CURRENT_TASK, taskProxy.CurrentTask);
        }

    }
}
