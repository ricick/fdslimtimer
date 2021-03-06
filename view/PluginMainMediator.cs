﻿using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.model;
using SlimTimer.vo;

namespace SlimTimer.view
{
    class PluginMainMediator : Mediator
    {
        public static new String NAME = "PluginMainMediator";
        public PluginMainMediator(PluginMain viewComponent):base(NAME,viewComponent)
        {
            pluginMain.dispose += new EventHandler(pluginMain_dispose);
            pluginMain.interaction += new EventHandler(pluginMain_interaction);
            pluginMain.changeProject += new PluginMain.ChangeProjectEventHandler(pluginMain_changeProject);
            pluginMain.changeFile += new PluginMain.ChangeFileEventHandler(pluginMain_changeFile);
        }

        void pluginMain_changeFile(object sender, ChangeFileEventArgs e)
        {
            SendNotification(ApplicationFacade.CHANGE_FILE, e.file);
        }

        void pluginMain_changeProject(object sender, ChangeProjectEventArgs e)
        {
            SendNotification(ApplicationFacade.CHANGE_PROJECT, e.project);
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
            return new List<string>(new string[] { SettingsProxy.CHANGE_SETTING_OBJECT });
        }
        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            base.HandleNotification(notification);

            switch (notification.Name)
            {
                case SettingsProxy.CHANGE_SETTING_OBJECT:
                    pluginMain.setSettingObject(notification.Body as SlimtimerSettings);
                    break;
            }
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
