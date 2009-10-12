using System;
using System.Collections.Generic;
using System.Text;
using Inikus.SlimTimer;
using PureMVC.Patterns;
using System.Windows.Forms;

namespace SlimTimer.model
{
    class APIProxy : Proxy
    {
        public static new String NAME = "APIProxy";
        public static String CHANGE_API = "CHANGE_API";

        
        private String apiKey = "597e15b6247461868e41b076e49a29";
        private Timer timer;
        private Timer submitTimer;
        private Timer timeoutTimer;

        public APIProxy(): base(NAME)
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(onTimerTick);
            submitTimer = new Timer();
            submitTimer.Interval = settingsProxy.AutoSubmitDuration;
            submitTimer.Tick += new EventHandler(onSubmitTimerTick);
            submitTimer.Start();
            settingsProxy.TimeoutDuration = settingsProxy.IdleTimeout * 60000;
            if (settingsProxy.TimeoutDuration == 0) settingsProxy.TimeoutDuration = 300000;
            ////log("timeoutDuration " + timeoutDuration);
            timeoutTimer = new Timer();
            timeoutTimer.Interval = settingsProxy.TimeoutDuration;
            timeoutTimer.Tick += new EventHandler(onTimeoutTimerTick);
            timeoutTimer.Start();
        }
        private SlimTimerApi api;

        public SlimTimerApi Api
        {
            get { return api; }
            set { 
                api = value;
                SendNotification(CHANGE_API, value);
            }
        }
        Boolean loggedIn;
        public Boolean LoggedIn
        {
            get { return loggedIn; }
            set { loggedIn = value; }
        }
        public void Login()
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            String username = settingsProxy.Username;
            String password = settingsProxy.Password;
            loggedIn = false;
            //stop all timers
            submitTimer.Stop();
            timer.Stop();
            statusProxy.Time = (new TimeSpan(0));
            timeoutTimer.Stop();
            if (username == "Username" || username == "username" || username == "" || password == "")
            {
                ////log("No user or pass");
                statusProxy.StatusText = ("Notlogged in");
                return;
            }
            api = new SlimTimerApi(username, password, apiKey);
            ////log("apiKey " + apiKey);
            try
            {
                loggedIn = api.Logon();
            }
            catch
            {
                ////log("couldn't log in as " + username);
                statusProxy.StatusText = ("Cannot log in as " + username + " with supplied password");
                return;
            }
            ////log("loggedIn " + loggedIn);
            statusProxy.StatusText = ("Logged in as " + username);
            if (!loggedIn) return;
            loadTasks();
        }
        public void ResetTimeOut()
        {
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            if (statusProxy.Idle)
            {
                if (loggedIn)
                {
                    statusProxy.StatusText = ("Logged in as " + settingsProxy.Username);
                    createNewTimeEntry();
                }
                else
                {
                    statusProxy.StatusText = ("Not logged in");
                }
                submitTimer.Start();
                timer.Start();
            }
            timeoutTimer.Stop();
            timeoutTimer.Start();
            statusProxy.Idle = false;
        }
        public void SubmitTimeEntry(TimeEntry timeEntry)
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            //log("submitTimeEntry");
            if (!loggedIn)
            {
                //log("notlogged in");
                return;
            }
            if (timeEntry == null)
            {
                //log("no time entry");
                return;
            }
            if (timeEntry.RelatedTask == null || timeEntry.RelatedTask.Id == null || timeEntry.RelatedTask.Id.Length == 0)
            {
                //log("no task to submit to");
                return;
            }
            DateTime startTime = timeEntry.StartTime;
            timeEntry.EndTime = DateTime.Now;
            timeEntry.Comments = taskProxy.Comments;
            //log("timeEntry.EndTime = " + timeEntry.EndTime);
            TimeSpan duration = timeEntry.EndTime.Subtract(timeEntry.StartTime);
            //log("timeEntry.StartTime = " + timeEntry.StartTime);
            //log("duration = " + duration);
            //int minimumTime = minimumTime;
            if (settingsProxy.MinimumTime < 1) settingsProxy.MinimumTime = 1;
            timeEntry.Duration = Convert.ToInt32(Math.Floor(duration.TotalSeconds));
            if (duration.TotalSeconds < settingsProxy.MinimumTime)
            {
                //log("not enough seconds to submit");
                return;
            }
            try
            {
                timeEntry = api.UpdateTimeEntry(timeEntry);
                //replace timeentry with result
                //log("timeEntry submitted " + timeEntry.Id);
            }
            catch (Exception exception)
            {
                statusProxy.StatusText = ("Error submitting time entry for " + settingsProxy.Username);
                Console.WriteLine("exception.Source : " + exception.Source);
                Console.WriteLine("exception.StackTrace : " + exception.StackTrace);
                Console.WriteLine("exception.Message : " + exception.Message);
                Console.WriteLine("exception.Data : " + exception.Data);
                Console.WriteLine("Error submitting time entry for " + settingsProxy.Username + " : " + exception.Message);
            }
            //set start time back to cached value (submitting returns server time)
            timeEntry.StartTime = startTime;
        }
        void onTimeoutTimerTick(object sender, EventArgs e)
        {
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            //log("onTimeoutTimerTick");
            if (!loggedIn) return;
            submitTimeEntry();
            timeoutTimer.Stop();
            submitTimer.Stop();
            timer.Stop();
            statusProxy.Idle = true;
            statusProxy.StatusText = ("Currently statusProxy.Idle");
        }
        void onSubmitTimerTick(object sender, EventArgs e)
        {
            //log("onSubmitTimerTick");
            if (!loggedIn) return;
            submitTimeEntry();
        }
        void onTimerTick(object sender, EventArgs e)
        {
            if (!loggedIn) return;
            if (timeEntry == null) return;
            timeEntry.EndTime = DateTime.Now;
            TimeSpan duration = DateTime.Now.Subtract(timeEntry.StartTime);
            ////log("duration = " + duration);
            statusProxy.Time = (duration);
        }
    }
}
