﻿using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace SlimTimer.model
{
    class TimerProxy : Proxy
    {
        public static new String NAME = "TimerProxy";
        public const String CHANGE_TIMER = "CHANGE_TIMER";
        public const String CHANGE_SUBMIT_TIMER = "CHANGE_SUBMIT_TIMER";
        public const String CHANGE_TIMEOUT_TIMER = "CHANGE_TIMEOUT_TIMER";
        public TimerProxy()
            : base(NAME)
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(onTimerTick);
            timer.Start();
            submitTimer = new Timer();
            submitTimer.Interval = settingsProxy.AutoSubmitDuration;
            submitTimer.Tick += new EventHandler(onSubmitTimerTick);
            submitTimer.Start();
            Console.WriteLine("created submit timer");
            Console.WriteLine(submitTimer);
            Console.WriteLine(submitTimer.Enabled);
            settingsProxy.TimeoutDuration = settingsProxy.IdleTimeout * 60000;
            if (settingsProxy.TimeoutDuration == 0) settingsProxy.TimeoutDuration = 300000;
            ////log("timeoutDuration " + timeoutDuration);
            timeoutTimer = new Timer();
            timeoutTimer.Interval = settingsProxy.TimeoutDuration;
            timeoutTimer.Tick += new EventHandler(onTimeoutTimerTick);
            timeoutTimer.Start();
        }
        private Timer timer;

        public Timer Timer
        {
            get { return timer; }
            set
            {
                timer = value;
                SendNotification(CHANGE_TIMER, value);
            }
        }
        private Timer submitTimer;

        public Timer SubmitTimer
        {
            get { return submitTimer; }
            set
            {
                submitTimer = value;
                SendNotification(CHANGE_SUBMIT_TIMER, value);
            }
        }
        private Timer timeoutTimer;

        public Timer TimeoutTimer
        {
            get { return timeoutTimer; }
            set
            {
                timeoutTimer = value;
                SendNotification(CHANGE_TIMEOUT_TIMER, value);
            }
        }


        public void ResetTimeOut()
        {
            //Console.WriteLine("ResetTimeOut");
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            if (statusProxy.Idle)
            {
                if (statusProxy.LoggedIn)
                {
                    statusProxy.StatusText = ("Logged in as " + settingsProxy.Username);
                    SendNotification(ApplicationFacade.NEW_TIME_ENTRY);
                }
                else
                {
                    statusProxy.StatusText = ("Not logged in");
                }
                SubmitTimer.Start();
                Timer.Start();
            }
            TimeoutTimer.Stop();
            TimeoutTimer.Start();
            statusProxy.Idle = false;
        }


        void onTimeoutTimerTick(object sender, EventArgs e)
        {
            Console.WriteLine("onTimeoutTimerTick");
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            //log("onTimeoutTimerTick");
            if (!statusProxy.LoggedIn) return;
            SendNotification(ApplicationFacade.SAVE_TIME_ENTRY);
            Console.WriteLine("stopping timers");
            timeoutTimer.Stop();
            submitTimer.Stop();
            timer.Stop();
            statusProxy.Idle = true;
            statusProxy.StatusText = ("Currently statusProxy.Idle");
        }
        void onSubmitTimerTick(object sender, EventArgs e)
        {
            Console.WriteLine("onSubmitTimerTick");
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            //log("onSubmitTimerTick");
            if (!statusProxy.LoggedIn) return;
            SendNotification(ApplicationFacade.SAVE_TIME_ENTRY);
        }
        void onTimerTick(object sender, EventArgs e)
        {
           // Console.WriteLine("onTimerTick");
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;
            TimeEntry timeEntry = taskProxy.CurrentTimeEntry;
           // Console.WriteLine("statusProxy.LoggedIn " + statusProxy.LoggedIn);
            //Console.WriteLine("timeEntry " + timeEntry);
            if (!statusProxy.LoggedIn) return;
            if (timeEntry == null) return;
            timeEntry.EndTime = DateTime.Now;
            TimeSpan duration = DateTime.Now.Subtract(timeEntry.StartTime);
            //Console.WriteLine("duration = " + duration);
            statusProxy.Time = (duration);
        }

    }
}
