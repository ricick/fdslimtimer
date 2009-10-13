using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;

namespace SlimTimer.model
{
    class StatusProxy : Proxy
    {
        public static new String NAME = "StatusProxy";
        public const String CHANGE_STATUS_TEXT = "CHANGE_STATUS_TEXT";
        public const String CHANGE_LOADED_TASKS = "CHANGE_LOADED_TASKS";
        public const String CHANGE_IDLE = "CHANGE_IDLE";
        public const String CHANGE_TIME = "CHANGE_TIME";
        public const String CHANGE_IGNORED_PROJECT = "CHANGE_IGNORED_PROJECT";
        public const String CHANGE_LOGGED_IN = "CHANGE_LOGGED_IN";
        public const String CHANGE_PROJECT_TEXT = "CHANGE_PROJECT_TEXT";
        public const String CHANGE_TRACKING = "CHANGE_TRACKING";
        public StatusProxy()
            : base(NAME)
        {
        }
        private String statusText;

        public String StatusText
        {
            get { return statusText; }
            set { 
                statusText = value;
                SendNotification(CHANGE_STATUS_TEXT, value);
            }
        }


        private String projectText;

        public String ProjectText
        {
            get { return projectText; }
            set
            {
                projectText = value;
                SendNotification(CHANGE_PROJECT_TEXT, value);
            }
        }

        private Boolean loadedTasks;

        public Boolean LoadedTasks
        {
            get { return loadedTasks; }
            set
            {
                loadedTasks = value;
                SendNotification(CHANGE_LOADED_TASKS, value);
            }
        }

        private Boolean ignoredProject;

        public Boolean IgnoredProject
        {
            get { return ignoredProject; }
            set
            {
                ignoredProject = value;
                SendNotification(CHANGE_IGNORED_PROJECT, value);
            }
        }

        private Boolean loggedIn;

        public Boolean LoggedIn
        {
            get { return loggedIn; }
            set
            {
                loggedIn = value;
                SendNotification(CHANGE_LOGGED_IN, value);
            }
        }
        private Boolean idle;

        public Boolean Idle
        {
            get { return idle; }
            set
            {
                idle = value;
                SendNotification(CHANGE_IDLE, value);
            }
        }

        private TimeSpan time;

        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
                SendNotification(CHANGE_TIME, value);
            }
        }

        private Boolean tracking;

        public Boolean Tracking
        {
            get { return tracking; }
            set
            {
                tracking = value;
                SendNotification(CHANGE_TRACKING, value);
            }
        }
    }
}
