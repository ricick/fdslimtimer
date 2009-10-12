using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;

namespace SlimTimer.view
{
    class PluginMainMediator : Mediator
    {
        public static new String NAME = "PluginMainMediator";
        public PluginMainMediator(PluginMain viewComponent):base(NAME,viewComponent)
        {
            pluginMain.dispose += new EventHandler(pluginMain_dispose);
            pluginMain.interaction += new EventHandler(pluginMain_interaction);
        }

        void pluginMain_interaction(object sender, EventArgs e)
        {
            SendNotification(ApplicationFacade.RESET_TIMEOUT);
        }

        void pluginMain_dispose(object sender, EventArgs e)
        {
            SendNotification(ApplicationFacade.APPLICATION_SHUTDOWN);
        }
        public override IList<string> ListNotificationInterests()
        {
            return base.ListNotificationInterests();
        }
        private PluginMain pluginMain{
            get
            {
                return ViewComponent as PluginMain;
            }
        }
        public override void OnRegister()
        {
            base.OnRegister();
            Facade.RegisterMediator(new PluginUIMediator(pluginMain.Ui));
        }
    }
}
