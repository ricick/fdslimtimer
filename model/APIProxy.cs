using System;
using System.Collections.Generic;
using System.Text;
using Inikus.SlimTimer;
using PureMVC.Patterns;

namespace SlimTimer.model
{
    class APIProxy : Proxy
    {
        public static new String NAME = "APIProxy";
        public static String CHANGE_API = "CHANGE_API";
        public APIProxy(): base(NAME)
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(onTimerTick);
            submitTimer = new Timer();
            submitTimer.Interval = autoSubmitDuration;
            submitTimer.Tick += new EventHandler(onSubmitTimerTick);
            submitTimer.Start();
            timeoutDuration = idleTimeout * 60000;
            if (timeoutDuration == 0) timeoutDuration = 300000;
            //log("timeoutDuration " + timeoutDuration);
            timeoutTimer = new Timer();
            timeoutTimer.Interval = timeoutDuration;
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
            String username = settingsProxy.Username;
            String password = settingsProxy.Password;
            loggedIn = false;
            //stop all timers
            submitTimer.Stop();
            timer.Stop();
            ui.setTime(new TimeSpan(0));
            timeoutTimer.Stop();
            if (username == "Username" || username == "username" || username == "" || password == "")
            {
                //log("No user or pass");
                ui.setStatusText("Notlogged in");
                return;
            }
            api = new SlimTimerApi(username, password, apiKey);
            //log("apiKey " + apiKey);
            try
            {
                loggedIn = api.Logon();
            }
            catch
            {
                //log("couldn't log in as " + username);
                ui.setStatusText("Cannot log in as " + username + " with supplied password");
                return;
            }
            //log("loggedIn " + loggedIn);
            ui.setStatusText("Logged in as " + username);
            if (!loggedIn) return;
            loadTasks();
        }
        public void ResetTimeOut()
        {
            if (idle)
            {
                if (loggedIn)
                {
                    ui.setStatusText("Logged in as " + username);
                    createNewTimeEntry();
                }
                else
                {
                    ui.setStatusText("Not logged in");
                }
                submitTimer.Start();
                timer.Start();
            }
            timeoutTimer.Stop();
            timeoutTimer.Start();
            idle = false;
        }
    }
}
