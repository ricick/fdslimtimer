using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using PluginCore;

namespace SlimTimer.control
{
    class ChangeProjectCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            log("onChangeProject");
            //IProject project = PluginBase.CurrentProject;
            IProject project = notification.Body as IProject;
            timer.Stop();
            if (project == null)
            {
                log("Project closed");
                currentTask = null;
                timeEntry = null;
                ui.setProjectText("No project open");
            }
            else
            {
                ui.setProjectText(project.Name);
                findCurrentTask();
            }
        }

    }
}
