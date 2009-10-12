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
        public static String CHANGE_STATUS_TEXT = "CHANGE_STATUS_TEXT";
        public static String CHANGE_LOADED_TASKS = "CHANGE_LOADED_TASKS";
        public static String CHANGE_IDLE = "CHANGE_IDLE";
        public static String CHANGE_TIME = "CHANGE_TIME";
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
    }
}
