using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace SlimTimer
{
    public delegate void SettingChangeHandler(string setting);

    [Serializable]
    public class SlimtimerSettings
    {
        public event SettingChangeHandler Changed;

        private String username = "username";
        private String password = "password";
        private int idleTimeout = 5;
        private bool fileComments = true;
        private int minimumTime = 5;
        private bool cleanupDuplicates = true;
        private bool askIgnoreProject = true;
        private string[] trackedProjects = new string[] { };
        private string[] ignoredProjects = new string[] { };
        //private string[] projectMap = new string[] { };

        /// <summary> 
        /// Get and sets the password
        /// </summary>
        [Description("Slimtimer password."), DefaultValue("password")]
        public String Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                FireChanged("password");
            }
        }

        /// <summary> 
        /// Get and sets the username
        /// </summary>
        [Description("Slimtimer username."), DefaultValue("username")]
        public String Username
        {
            get { return this.username; }
            set
            {
                this.username = value;
                FireChanged("username");
            }
        }
        /// <summary> 
        /// Get and sets the idleTimeout
        /// </summary>
        [Description("Idle time in minutes."), DefaultValue(5)]
        public int IdleTimeout
        {
            get { return this.idleTimeout; }
            set
            {
                this.idleTimeout = value;
                FireChanged("idleTimeout");
            }
        }
        /// <summary> 
        /// Get and sets the fileComments
        /// </summary>
        [Description("Save the opened files in time entry comments."), DefaultValue(true)]
        public bool FileComments
        {
            get { return this.fileComments; }
            set
            {
                this.fileComments = value;
                FireChanged("fileComments");
            }
        }
        /// <summary> 
        /// Get and sets the minimumTime
        /// </summary>
        [Description("Minimum time to submit in seconds."), DefaultValue(5)]
        public int MinimumTime
        {
            get { return this.minimumTime; }
            set
            {
                this.minimumTime = value;
                FireChanged("minimumTime");
            }
        }
        /// <summary> 
        /// Get and sets the cleanupDuplicates
        /// </summary>
        [Description("Clean up duplicate projects on launch."), DefaultValue(true)]
        public bool CleanupDuplicates
        {
            get { return this.cleanupDuplicates; }
            set
            {
                this.cleanupDuplicates = value;
                FireChanged("cleanupDuplicates");
            }
        }
        /// <summary> 
        /// Get and sets the askIgnoreProject
        /// </summary>
        [Description("Ask about ignoring a previously unopened project."), DefaultValue(true)]
        public bool AskIgnoreProject
        {
            get { return this.askIgnoreProject; }
            set
            {
                this.askIgnoreProject = value;
                FireChanged("askIgnoreProject");
            }
        }
        /// <summary> 
        /// Get and sets the trackedProjects
        /// </summary>
        [DisplayName("Tracked Projects")]
        public string[] TrackedProjects
        {
            get { return this.trackedProjects; }
            set
            {
                this.trackedProjects = value;
                FireChanged("trackedProjects");
            }
        }
        /// <summary> 
        /// Get and sets the ignoredProjects
        /// </summary>
        [DisplayName("Ignored Projects")]
        public string[] IgnoredProjects
        {
            get { return this.ignoredProjects; }
            set
            {
                this.ignoredProjects = value;
                FireChanged("ignoredProjects");
            }
        }
        /*
        /// <summary> 
        /// Get and sets the projectMap
        /// </summary>
        [DisplayName("Project Map")]
        public string[] ProjectMap
        {
            get { return this.projectMap; }
            set
            {
                this.projectMap = value;
                FireChanged("projectMap");
            }
        }
        */
        private void FireChanged(string setting)
        {
            if (Changed != null)
                Changed(setting);
        }

    }

}
