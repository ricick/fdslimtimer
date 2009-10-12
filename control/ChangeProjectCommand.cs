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
        private void findCurrentTask()
        {
            log("findCurrentTask");
            if (!loggedIn) return;
            IProject project = PluginBase.CurrentProject;
            if (project == null)
            {
                log("No project");
                return;
            }
            if (tasks == null || !loadedTasks)
            {
                log("No tasks");
                return;
            }

            log("Project open: " + project.Name);
            foreach (string ignoredProjectName in ignoredProjects)
            {
                if (ignoredProjectName == project.Name)
                {
                    ignoredProject = true;
                    log("ignoring project: " + project.Name);
                    ui.setTime(new TimeSpan(0));
                    ui.setProjectText("Not tracking " + project.Name);
                    ui.setTracking(false);
                    return;
                }
            }
            log("ignoredProject: " + ignoredProject);
            bool inTrackList = false;
            foreach (string trackedProjectName in trackedProjects)
            {
                if (trackedProjectName == project.Name)
                {
                    inTrackList = true;
                    break;
                }
            }
            log("inTrackList: " + inTrackList);
            if (!inTrackList)
            {
                if (askIgnoreProject)
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
            log("ignoreProject " + project.Name);
            string[] tempIgnoredProjects = new string[ignoredProjects.Length + 1];
            ignoredProjects.CopyTo(tempIgnoredProjects, 0);
            tempIgnoredProjects.SetValue(project.Name, ignoredProjects.Length);
            ignoredProjects = tempIgnoredProjects;
            settingObject.IgnoredProjects = ignoredProjects;
            ui.setTime(new TimeSpan(0));
            ui.setProjectText("Not tracking " + project.Name);
        }
        private void trackProject(IProject project, bool inTrackList)
        {
            log("trackProject " + project.Name + " inTrackList " + inTrackList);
            if (!inTrackList)
            {
                string[] tempTrackedProjects = new string[trackedProjects.Length + 1];
                trackedProjects.CopyTo(tempTrackedProjects, 0);
                tempTrackedProjects.SetValue(project.Name, trackedProjects.Length);
                trackedProjects = tempTrackedProjects;
                settingObject.TrackedProjects = trackedProjects;
                //trackedProjects = project.Name;
            }
            //look for task with project name
            bool inRemoteTasks = false;
            foreach (Task task in tasks)
            {
                log("checking task.Name " + task.Name);
                if (task.Name == project.Name)
                {
                    log("Found matching task " + task.Name);
                    currentTask = task;
                    inRemoteTasks = true;
                }
            }
            if (!inRemoteTasks)
            {
                log("Creating new task");
                currentTask = new Task(project.Name);
                try
                {
                    currentTask = api.UpdateTask(currentTask);
                }
                catch (Exception exception)
                {
                    ui.setStatusText("Error creating task for " + username);
                    log("exception.Source : " + exception.Source);
                    log("exception.StackTrace : " + exception.StackTrace);
                    log("exception.Message : " + exception.Message);
                    log("exception.Data : " + exception.Data);
                    log("Error creating task for " + username + " : " + exception.Message);
                }
            }
            if (timeEntry != null)
            {
                submitTimeEntry();
            }
            ui.setCurrentTask(currentTask);
            ui.setTracking(true);
            createNewTimeEntry();
            timer.Start();
        }

    }
}
