using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using Inikus.SlimTimer;

namespace SlimTimer.control
{
    class LoginCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            APIProxy apiProxy = Facade.RetrieveProxy(APIProxy.NAME) as APIProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TimerProxy timerProxy = Facade.RetrieveProxy(TimerProxy.NAME) as TimerProxy;
            String username = settingsProxy.Username;
            String password = settingsProxy.Password;
            statusProxy.LoggedIn = false;
            //stop all timers
            timerProxy.SubmitTimer.Stop();
            timerProxy.Timer.Stop();
            statusProxy.Time = (new TimeSpan(0));
            timerProxy.TimeoutTimer.Stop();
            if (username == "Username" || username == "username" || username == "" || password == "")
            {
                ////log("No user or pass");
                statusProxy.StatusText = ("Notlogged in");
                return;
            }
            apiProxy.Api = new SlimTimerApi(username, password, apiProxy.ApiKey);
            ////log("apiKey " + apiKey);
            try
            {
                statusProxy.LoggedIn = apiProxy.Api.Logon();
            }
            catch
            {
                ////log("couldn't log in as " + username);
                statusProxy.StatusText = ("Cannot log in as " + username + " with supplied password");
                return;
            }
            ////log("loggedIn " + loggedIn);
            statusProxy.StatusText = ("Logged in as " + username);
            if (!statusProxy.LoggedIn) return;
            //loadTasks();
            SendNotification(ApplicationFacade.GET_TASKS);
        }

    }
}
