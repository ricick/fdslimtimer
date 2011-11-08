using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using System.IO;
using SlimTimer.vo;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace SlimTimer.model
{
    class SettingsProxy : Proxy
    {
        public static new String NAME = "SettingsProxy";
        public const String CHANGE_USERNAME = "CHANGE_USERNAME";
        public const String CHANGE_PASSWORD = "CHANGE_PASSWORD";
        public const String CHANGE_IDLE_TIMEOUT = "CHANGE_IDLE_TIMEOUT";
        public const String CHANGE_FILE_COMMENTS = "CHANGE_FILE_COMMENTS";
        public const String CHANGE_MINIMUM_TIME = "CHANGE_MINIMUM_TIME";
        public const String CHANGE_TIMEOUT_DURATION = "CHANGE_TIMEOUT_DURATION";
        public const String CHANGE_AUTO_SUBMIT_DURATION = "CHANGE_AUTO_SUBMIT_DURATION";
        public const String CHANGE_CLEANUP_DUPLICATES = "CHANGE_CLEANUP_DUPLICATES";
        public const String CHANGE_ASK_IGNORE_PROJECT = "CHANGE_ASK_IGNORE_PROJECT";
        public const String CHANGE_TRACKED_PROJECTS = "CHANGE_TRACKED_PROJECTS";
        public const String CHANGE_IGNORED_PROJECTS = "CHANGE_IGNORED_PROJECTS";
        public const String CHANGE_PROJECT_MAP = "CHANGE_PROJECT_MAP";
        public const String CHANGE_SETTING_OBJECT = "CHANGE_SETTING_OBJECT";
        private String username = "";

        public String Username
        {
            get { return username; }
            set { 
                username = value;
                SendNotification(CHANGE_USERNAME, value);
            }
        }
        private String password = "";

        public String Password
        {
            get { return password; }
            set
            {
                password = value;
                SendNotification(CHANGE_PASSWORD, value);
            }
        }
        private int idleTimeout = 5;

        public int IdleTimeout
        {
            get { return idleTimeout; }
            set
            {
                idleTimeout = value;
                SendNotification(CHANGE_IDLE_TIMEOUT, value);
            }
        }
        private bool fileComments = true;

        public bool FileComments
        {
            get { return fileComments; }
            set
            {
                fileComments = value;
                SendNotification(CHANGE_FILE_COMMENTS, value);
            }
        }
        private int minimumTime = 5;

        public int MinimumTime
        {
            get { return minimumTime; }
            set
            {
                minimumTime = value;
                SendNotification(CHANGE_MINIMUM_TIME, value);
            }
        }
        private int timeoutDuration = 300000;

        public int TimeoutDuration
        {
            get { return timeoutDuration; }
            set
            {
                timeoutDuration = value;
                SendNotification(CHANGE_TIMEOUT_DURATION, value);
            }
        }
        private int autoSubmitDuration = 300000;

        public int AutoSubmitDuration
        {
            get { return autoSubmitDuration; }
            set
            {
                autoSubmitDuration = value;
                SendNotification(CHANGE_AUTO_SUBMIT_DURATION, value);
            }
        }
        private bool cleanupDuplicates = true;

        public bool CleanupDuplicates
        {
            get { return cleanupDuplicates; }
            set
            {
                cleanupDuplicates = value;
                SendNotification(CHANGE_CLEANUP_DUPLICATES, value);
            }
        }
        private bool askIgnoreProject = true;

        public bool AskIgnoreProject
        {
            get { return askIgnoreProject; }
            set
            {
                askIgnoreProject = value;
                SendNotification(CHANGE_ASK_IGNORE_PROJECT, value);
            }
        }
        private string[] trackedProjects = new string[] { };

        public string[] TrackedProjects
        {
            get { return trackedProjects; }
            set
            {
                trackedProjects = value;
                SendNotification(CHANGE_TRACKED_PROJECTS, value);
            }
        }
        private string[] ignoredProjects = new string[] { };

        public string[] IgnoredProjects
        {
            get { return ignoredProjects; }
            set
            {
                ignoredProjects = value;
                SendNotification(CHANGE_IGNORED_PROJECTS, value);
            }
        }
        private string[] projectMap = new string[] { };

        public string[] ProjectMap
        {
            get { return projectMap; }
            set
            {
                projectMap = value;
                SendNotification(CHANGE_PROJECT_MAP, value);
            }
        }
        private String settingFilename;
        private SlimtimerSettings settingObject;
        public SlimtimerSettings SettingObject
        {
            get { return settingObject; }
            set
            {
                settingObject = value;
                SendNotification(CHANGE_SETTING_OBJECT, value);
            }
        }

        public SettingsProxy()
            : base(NAME)
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "SlimTimer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }
        public void LoadSettings()
        {
            Console.WriteLine("LoadSettings");
            this.SettingObject = new SlimtimerSettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.SettingObject = (SlimtimerSettings)obj;
            }
            /*
            */
            SettingObject.Changed += SettingChanged;
            Username = settingObject.Username;
            Console.WriteLine("Username " + Username);
            Password = settingObject.Password;
            Console.WriteLine("Password " + Password);
            IdleTimeout = SettingObject.IdleTimeout;
            FileComments = SettingObject.FileComments;
            MinimumTime = SettingObject.MinimumTime;
            CleanupDuplicates = SettingObject.CleanupDuplicates;
            AskIgnoreProject = SettingObject.AskIgnoreProject;
            TrackedProjects = SettingObject.TrackedProjects;
            IgnoredProjects = SettingObject.IgnoredProjects;
            //this.SaveSettings();
        }
        public void SaveSettings()
        {
            Console.WriteLine("SaveSettings in " + this.settingFilename);
            settingObject.Changed -= SettingChanged;
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }
        private void SettingChanged(string setting)
        {
            switch (setting)
            {
                case "username":
                    username = settingObject.Username;
                    //login();
                    SendNotification(ApplicationFacade.LOGIN);
                    break;
                case "password":
                    password = settingObject.Password;
                    //login();
                    SendNotification(ApplicationFacade.LOGIN);
                    break;
                case "idleTimeout":
                    idleTimeout = settingObject.IdleTimeout;
                    timeoutDuration = idleTimeout * 60000;
                    if (timeoutDuration == 0) timeoutDuration = 300000;
                    //timeoutTimer.Interval = timeoutDuration;
                    break;
                case "fileComments":
                    fileComments = settingObject.FileComments;
                    break;
                case "minimumTime":
                    minimumTime = settingObject.MinimumTime;
                    break;
                case "cleanupDuplicates":
                    cleanupDuplicates = settingObject.CleanupDuplicates;
                    //if (cleanupDuplicates) doCleanDuplicates();
                    break;
                case "askIgnoreProject":
                    askIgnoreProject = settingObject.AskIgnoreProject;
                    break;
                case "trackedProjects":
                    trackedProjects = settingObject.TrackedProjects;
                    break;
                case "ignoredProjects":
                    ignoredProjects = settingObject.IgnoredProjects;
                    break;
            }
        }
    }
}
