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

        public String ApiKey
        {
            get { return apiKey; }
        }

        public APIProxy(): base(NAME)
        {
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;

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
    }
}
