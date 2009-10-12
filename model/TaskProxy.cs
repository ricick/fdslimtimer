using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;

namespace SlimTimer.model
{
    class TaskProxy : Proxy
    {
        public static new String NAME = "TaskProxy";
        public static String CHANGE_TASKS = "CHANGE_TASKS";
        public static String CHANGE_CURRENT_TASK = "CHANGE_CURRENT_TASK";
        public static String CHANGE_CURRENT_TIME_ENTRY = "CHANGE_CURRENT_TIME_ENTRY";
        public TaskProxy():base(NAME)
        {
        }
        private Collection<Task> tasks;

        public Collection<Task> Tasks
        {
            get { return tasks; }
            set {
                tasks = value;
                SendNotification(CHANGE_TASKS, value);
            }
        }
        private TimeEntry currentTimeEntry;

        public TimeEntry CurrentTimeEntry
        {
            get { return currentTimeEntry; }
            set {
                currentTimeEntry = value;
                SendNotification(CHANGE_CURRENT_TIME_ENTRY, value);
            }
        }
        private Task currentTask;

        public Task CurrentTask
        {
            get { return currentTask; }
            set {
                currentTask = value;
                SendNotification(CHANGE_CURRENT_TASK, value);
            }
        }
    }
}
