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
        private void FireChanged(string setting)
        {
            if (Changed != null)
                Changed(setting);
        }

    }

}
