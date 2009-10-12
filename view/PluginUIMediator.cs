using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;

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
    }
}
