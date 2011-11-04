using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;
using System.Windows.Forms;
using PluginCore;

namespace SlimTimer.control
{
    class ChangeProjectCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Console.WriteLine("ChangeProjectCommand.Execute");
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            TimerProxy timerProxy = Facade.RetrieveProxy(TimerProxy.NAME) as TimerProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            Console.WriteLine("onChangeProject");
            //IProject project = PluginBase.CurrentProject;
            IProject project = notification.Body as IProject;
            timerProxy.Timer.Stop();
            if (project == null)
            {
                Console.WriteLine("Project closed");
                taskProxy.CurrentTask = null;
                taskProxy.CurrentTimeEntry = null;
                statusProxy.ProjectText = ("No project open");
            }
            else
            {
                statusProxy.ProjectText = (project.Name);
                findCurrentTask();
            }
        }
        private void findCurrentTask()
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            Console.WriteLine("findCurrentTask");
            if (!statusProxy.LoggedIn) return;
            IProject project = PluginBase.CurrentProject;
            if (project == null)
            {
                Console.WriteLine("No project");
                return;
            }
            if (taskProxy.Tasks == null || !statusProxy.LoadedTasks)
            {
                Console.WriteLine("No tasks");
                return;
            }

            Console.WriteLine("Project open: " + project.Name);
            foreach (string ignoredProjectName in settingsProxy.IgnoredProjects)
            {
                if (ignoredProjectName == project.Name)
                {
                    statusProxy.IgnoredProject = true;
                    Console.WriteLine("ignoring project: " + project.Name);
                    statusProxy.Time = (new TimeSpan(0));
                    statusProxy.ProjectText = ("Not tracking " + project.Name);
                    statusProxy.Tracking = false;
                    return;
                }
            }
            Console.WriteLine("statusProxy.IgnoredProject: " + statusProxy.IgnoredProject);
            bool inTrackList = false;
            foreach (string trackedProjectName in settingsProxy.TrackedProjects)
            {
                if (trackedProjectName == project.Name)
                {
                    inTrackList = true;
                    break;
                }
            }
            Console.WriteLine("inTrackList: " + inTrackList);
            if (!inTrackList)
            {
                if (settingsProxy.AskIgnoreProject)
                {
                    if (MessageBox.Show("Do you want to track the project " + project.Name + " with slimtimer?", "Untracked project", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        trackProject(project, inTrackList);
                    }
                    else
                    {
                        ignoreProject(project);
                    }
                }
                else
                {
                    trackProject(project, inTrackList);
                }
            }
            else
            {
                trackProject(project, inTrackList);
            }
        }
        private void ignoreProject(IProject project)
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            Console.WriteLine("ignoreProject " + project.Name);
            string[] tempIgnoredProjects = new string[settingsProxy.IgnoredProjects.Length + 1];
            settingsProxy.IgnoredProjects.CopyTo(tempIgnoredProjects, 0);
            tempIgnoredProjects.SetValue(project.Name, settingsProxy.IgnoredProjects.Length);
            settingsProxy.IgnoredProjects = tempIgnoredProjects;
            statusProxy.Time = (new TimeSpan(0));
            statusProxy.ProjectText = ("Not tracking " + project.Name);
        }
        private void trackProject(IProject project, bool inTrackList)
        {
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;
            TimerProxy timerProxy = Facade.RetrieveProxy(TimerProxy.NAME) as TimerProxy;
            Console.WriteLine("trackProject " + project.Name + " inTrackList " + inTrackList);
            if (!inTrackList)
            {
                string[] tempTrackedProjects = new string[settingsProxy.TrackedProjects.Length + 1];
                settingsProxy.TrackedProjects.CopyTo(tempTrackedProjects, 0);
                tempTrackedProjects.SetValue(project.Name, settingsProxy.TrackedProjects.Length);
                settingsProxy.TrackedProjects = tempTrackedProjects;
                //settingObject.TrackedProjects = settingsProxy.TrackedProjects;
                //trackedProjects = project.Name;
            }
            //look for task with project name
            bool inRemoteTasks = false;
            foreach (Task task in taskProxy.Tasks)
            {
                Console.WriteLine("checking task.Name " + task.Name);
                if (task.Name == project.Name)
                {
                    Console.WriteLine("Found matching task " + task.Name);
                    taskProxy.CurrentTask = task;
                    inRemoteTasks = true;
                    break;
                }
            }
            if (!inRemoteTasks)
            {
                Console.WriteLine("Creating new task");
                taskProxy.CurrentTask = new Task(project.Name);
                try
                {
                    taskProxy.CurrentTask = apiProxy.Api.UpdateTask(taskProxy.CurrentTask);
                }
                catch (Exception exception)
                {
                    statusProxy.StatusText = ("Error creating task for " + settingsProxy.Username);
                    Console.WriteLine("exception.Source : " + exception.Source);
                    Console.WriteLine("exception.StackTrace : " + exception.StackTrace);
                    Console.WriteLine("exception.Message : " + exception.Message);
                    Console.WriteLine("exception.Data : " + exception.Data);
                    Console.WriteLine("Error creating task for " + settingsProxy.Username + " : " + exception.Message);
                }
            }
            if (taskProxy.CurrentTimeEntry != null)
            {
                SendNotification(ApplicationFacade.SAVE_TIME_ENTRY);
            }
            statusProxy.Tracking = false;
            SendNotification(ApplicationFacade.NEW_TIME_ENTRY);
            timerProxy.Timer.Start();
        }

    }
}
