using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.model;
using System.Windows.Forms;

namespace SlimTimer.view
{
    class PluginUIMediator : Mediator
    {
        public static new String NAME = "PluginUIMediator";
        public PluginUIMediator(PluginUI viewComponent)
            : base(NAME, viewComponent)
        {
            pluginUI.OnPause += new EventHandler(pluginUI_OnPause);
            pluginUI.OnResume += new EventHandler(pluginUI_OnResume);
        }

        void pluginUI_OnResume(object sender, EventArgs e)
        {
            Console.WriteLine("pluginUI_OnResume");
        }

        void pluginUI_OnPause(object sender, EventArgs e)
        {
            Console.WriteLine("pluginUI_OnPause");
        }
        private PluginUI pluginUI
        {
            get
            {
                return ViewComponent as PluginUI;
            }
        }
        public override IList<string> ListNotificationInterests()
        {
            return new List<string>(new string[] { StatusProxy.CHANGE_STATUS_TEXT, StatusProxy.CHANGE_PROJECT_TEXT, StatusProxy.CHANGE_TIME });
        }
        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            base.HandleNotification(notification);
            switch (notification.Name)
            {
                case StatusProxy.CHANGE_STATUS_TEXT:
                    pluginUI.Invoke((MethodInvoker)delegate
                    {
                        pluginUI.setStatusText(notification.Body as String);
                    });
                    break;
                case StatusProxy.CHANGE_PROJECT_TEXT:
                    pluginUI.Invoke((MethodInvoker)delegate
                    {
                        pluginUI.setProjectText(notification.Body as String);
                    });
                    break;
                case StatusProxy.CHANGE_TIME:
                    //Console.WriteLine("HandleNotification TimerProxy.CHANGE_TIMER");
                    pluginUI.Invoke((MethodInvoker)delegate
                    {
                        pluginUI.setTime((TimeSpan)notification.Body);
                    });
                    break;
            }
        }
    }
}
