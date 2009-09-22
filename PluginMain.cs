using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using SlimTimer.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace SlimTimer
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "SlimTimer";
        private String pluginGuid = "f80042e0-7525-11de-800b-0002a5d5c51b";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "SlimTimer plugin for FlashDevelop 3.";
        private String pluginAuth = "Phil Douglas";
        private String apiKey = "597e15b6247461868e41b076e49a29";
        #region Setting Properties
        private String username = "";
        private String password = "";
        private int idleTimeout = 5;
        private bool fileComments = true;
        private int minimumTime = 5;
        private int timeoutDuration = 300000;
        private int autoSubmitDuration = 300000;
        private bool cleanupDuplicates = true;
        private bool askIgnoreProject = true;
        private string[] trackedProjects = new string[] { };
        private string[] ignoredProjects = new string[] { };
        private string[] projectMap = new string[] { };
        #endregion
        private String settingFilename;
        private SlimtimerSettings settingObject;
        private DockContent pluginPanel;
        private PluginUI pluginUI;
        private Image pluginImage;
        private SlimTimerApi api;
        private bool loggedIn;
        private Collection<Task> tasks;
        private bool loadedTasks;
        private Task currentTask;
        private TimeEntry timeEntry;
        private Timer timer;
        private Timer submitTimer;
        private Timer timeoutTimer;
        private bool idle;
        private bool paused;
        private bool ignoredProject;
        private string comments;
	    #region Required Properties

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
		{
			get { return this.pluginName; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
		{
			get { return this.pluginGuid; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
		{
			get { return this.pluginAuth; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
		{
			get { return this.pluginDesc; }
		}

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
		{
			get { return this.pluginHelp; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }
		
		#endregion
		
		#region Required Methods
		
		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
            this.InitLocalization();
            this.CreatePluginPanel();
            this.CreateMenuItem();
            this.SetupApi();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            //
            /*
            IProject project = PluginBase.CurrentProject;
            if (project == null)
            {
                log("No Project");
            }
            else
            {
                log("Is open " + project.Name);
            }
            */
            if (e.Type != EventType.UIClosing)log("HandleEvent " + e.Type.ToString());
            //reset timeout on all events
            resetTimeOut();
            String type = e.Type.ToString();
            switch (e.Type)
            {
                // Catches FileSwitch event and displays the filename it in the PluginUI.
                case EventType.FileOpen:
                   // string fileName = PluginBase.MainForm.CurrentDocument.FileName;
                    //pluginUI.Output.Text += fileName + "\r\n";
                   log("Switched file "); // tracing to output panel
                    onChangeFile();
                    break;

                // Catches Project change event and display the active project path
                case EventType.Command:
                    string cmd = (e as DataEvent).Action;
                    String comandType = cmd.ToString();
                   //log("cmd " + cmd);
                    if (cmd == "ProjectManager.Project")
                    {
                        onChangeProject();
                    }
                    break;
                case EventType.FileClose:
                    if (PluginBase.MainForm.ClosingEntirely) onClosingApp();
                    break;
            }
		}
		
		#endregion

        #region Load/Save settings

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new SlimtimerSettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (SlimtimerSettings)obj;
            }
            settingObject.Changed += SettingChanged;
            username = settingObject.Username;
            password = settingObject.Password;
            idleTimeout = settingObject.IdleTimeout;
            fileComments = settingObject.FileComments;
            minimumTime = settingObject.MinimumTime;
            cleanupDuplicates = settingObject.CleanupDuplicates;
            askIgnoreProject = settingObject.AskIgnoreProject;
            trackedProjects = settingObject.TrackedProjects;
            ignoredProjects = settingObject.IgnoredProjects;
            //projectMap = settingObject.ProjectMap;
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            settingObject.Changed -= SettingChanged;
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        private void SettingChanged(string setting)
        {
            log("Settings changed");
            switch (setting)
            {
                case "username":
                    username = settingObject.Username;
                    log("username " + username);
                    login();
                    break;
                case "password":
                    password = settingObject.Password;
                    login();
                    break;
                case "idleTimeout":
                    idleTimeout = settingObject.IdleTimeout;
                    timeoutDuration = idleTimeout * 60000;
                    if (timeoutDuration == 0) timeoutDuration = 300000;
                    timeoutTimer.Interval = timeoutDuration;
                    break;
                case "fileComments":
                    fileComments = settingObject.FileComments;
                    break;
                case "minimumTime":
                    minimumTime = settingObject.MinimumTime;
                    break;
                case "cleanupDuplicates":
                    cleanupDuplicates = settingObject.CleanupDuplicates;
                    if (cleanupDuplicates) doCleanDuplicates();
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
        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "SlimTimer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("123");
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            viewMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Label.ViewMenuItem"), this.pluginImage, new EventHandler(this.OpenPanel)));
        }

        /// <summary>
        /// Adds the slimtimer API
        /// </summary> 
        public void SetupApi()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(onTimerTick);
            submitTimer = new Timer();
            submitTimer.Interval = autoSubmitDuration;
            submitTimer.Tick += new EventHandler(onSubmitTimerTick);
            submitTimer.Start();
            timeoutDuration = idleTimeout * 60000;
            if (timeoutDuration == 0) timeoutDuration = 300000;
            log("timeoutDuration " + timeoutDuration);
            timeoutTimer = new Timer();
            timeoutTimer.Interval = timeoutDuration;
            timeoutTimer.Tick += new EventHandler(onTimeoutTimerTick);
            timeoutTimer.Start();
           login();
        }
        private void onClosingApp()
        {
            submitTimeEntry();
        }
        private void resetTimeOut()
        {
            if (idle)
            {
                if (loggedIn)
                {
                    pluginUI.setStatusText("Logged in as " + username);
                    createNewTimeEntry();
                }
                else
                {
                    pluginUI.setStatusText("Not logged in");
                }
                submitTimer.Start();
                timer.Start();
            }
            timeoutTimer.Stop();
            timeoutTimer.Start();
            idle = false;
        }
        private void login()
        {
            loggedIn = false;
            //stop all timers
            submitTimer.Stop();
            timer.Stop();
            pluginUI.setTime(new TimeSpan(0));
            timeoutTimer.Stop();
            if (username == "Username" || username == "username" || username == "" || password == "")
            {
               log("No user or pass");
                pluginUI.setStatusText("Notlogged in");
                return;
            }
            api = new SlimTimerApi(username, password, apiKey);
           log("apiKey " + apiKey);
            try
            {
                loggedIn = api.Logon();
            }
            catch
            {
               log("couldn't log in as " + username);
                pluginUI.setStatusText("Cannot log in as " + username+" with supplied password");
                return;
            }
           log("loggedIn " +loggedIn);
            pluginUI.setStatusText("Logged in as " + username);
            if (!loggedIn) return;
            loadTasks();
        }
        void onTimeoutTimerTick(object sender, EventArgs e)
        {
           log("onTimeoutTimerTick");
            if (!loggedIn) return;
            submitTimeEntry();
            timeoutTimer.Stop();
            submitTimer.Stop();
            timer.Stop();
            idle = true;
            pluginUI.setStatusText("Currently idle");
        }
        void onSubmitTimerTick(object sender, EventArgs e)
        {
           log("onSubmitTimerTick");
            if (!loggedIn) return;
            submitTimeEntry();
        }
        void onTimerTick(object sender, EventArgs e)
        {
            if (!loggedIn) return;
            if (timeEntry == null) return;
            timeEntry.EndTime = DateTime.Now;
            TimeSpan duration = DateTime.Now.Subtract(timeEntry.StartTime);
            //log("duration = " + duration);
            pluginUI.setTime(duration);
        }
        private void loadTasks()
        {
           log("loadTasks");
           try
           {
               tasks = api.ListTasks(SlimTimerApi.ShowCompletedTask.No, SlimTimerApi.TaskFilters.Owner);
               loadedTasks = true;
           }
           catch (Exception exception)
           {
               pluginUI.setStatusText("Error loading tasks for" + username);
               log("Error loading tasks for" + username + " : " + exception.Message);
           }
           if (cleanupDuplicates) doCleanDuplicates();
           findCurrentTask();
        }
        private void doCleanDuplicates()
        {
            log("doCleanDuplicates");
            ArrayList checkedProjects = new ArrayList();
            Task originalTask = new Task("null");
            foreach (Task checkTask in tasks)
            {
                bool found = false;
                foreach (Task compareTask in checkedProjects)
                {
                    if (compareTask.Name == checkTask.Name)
                    {
                        found = true;
                        originalTask = compareTask;
                        break;
                    }
                }
                if (found)
                {
                    //move entries to original and delete duplicate
                    //api;
                    log("Duplicate project found :"+checkTask.Name+" hours = "+checkTask.Hours);
                    Collection<TimeEntry> potentialEntries = api.ListTimeEntries(checkTask.CreatedTime, checkTask.UpdatedTime);
                    foreach (TimeEntry checkEntry in potentialEntries)
                    {
                        if (checkEntry.RelatedTask.Id == checkTask.Id)
                        {
                            log("Entry in duplicate project found :" + checkEntry.StartTime);
                            //set the timeentry to be associated with the original task and save it
                            checkEntry.RelatedTask = originalTask;
                            api.UpdateTimeEntry(checkEntry);
                        }
                    }
                    //delete the task
                    api.DeleteTask(checkTask.Id);
                    
                }
                else
                {
                    checkedProjects.Add(checkTask);
                }
            }
        }
        private void onChangeFile()
        {
           log("onChangeFile");
           if (ignoredProject)
           {
               log("ignoredProject");
               return;
           }
            if (timeEntry == null)
            {
               log("no timeentry");
                return;
            }
            if (fileComments)
            {
                //string fileName = PathHelper.GetShortPathName(PluginBase.MainForm.CurrentDocument.FileName);
                string fileName = PluginBase.MainForm.CurrentDocument.FileName;
                //fileName = fileName.Substring(fileName.LastIndexOf("\\"),fileName.Length);
                log("fileName " + fileName);
                if (comments.IndexOf(fileName) != -1)
                {
                    log("already contains task");
                    return;
                }
                comments = comments + fileName + "\n\r";
            }
            /*
            if (timeEntry.Tags.Contains(fileName))
            {
               log("already contains task");
                return;
            }
            timeEntry.Tags.Add(fileName);
             */
        }
        private void checkTags()
        {
        }
        private void onChangeProject()
        {
           log("onChangeProject");
            IProject project = PluginBase.CurrentProject;
            timer.Stop();
            if (project == null)
            {
               log("Project closed");
                currentTask = null;
                timeEntry = null;
                pluginUI.setProjectText("No project open");
            }
            else
            {
                pluginUI.setProjectText(project.Name);
                findCurrentTask();
            }
        }
        private void findCurrentTask()
        {
            log("findCurrentTask");
            if (!loggedIn) return;
            IProject project = PluginBase.CurrentProject;
            if (project == null)
            {
               log("No project");
                return;
            }
            if (tasks == null || !loadedTasks)
            {
               log("No tasks");
                return;
            }

           log("Project open: " + project.Name);
           foreach (string ignoredProjectName in ignoredProjects)
            {
                if (ignoredProjectName == project.Name)
                {
                    ignoredProject = true;
                    log("ignoring project: " + project.Name);
                    pluginUI.setTime(new TimeSpan(0));
                    pluginUI.setProjectText("Not tracking " + project.Name);
                    return;
                }
            }
           log("ignoredProject: " + ignoredProject);
            bool inTrackList = false;
            foreach (string trackedProjectName in trackedProjects)
            {
                if (trackedProjectName == project.Name)
                {
                    inTrackList = true;
                    break;
                }
            }
            log("inTrackList: " + inTrackList);
            if (!inTrackList)
            {
                if (askIgnoreProject)
                {
                    if (MessageBox.Show("Do you want to track the project " + project.Name + " with slimtimer?", "Untracked project", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        trackProject(project, inTrackList);
                    }
                    else
                    {
                        ignoreProject(project);
                    }
                }
                else
                {
                    trackProject(project, inTrackList);
                }
            }
            else
            {
                trackProject(project, inTrackList);
            }
        }
        private void ignoreProject(IProject project)
        {
            log("ignoreProject " + project.Name);
            string[] tempIgnoredProjects = new string[ignoredProjects.Length + 1];
            ignoredProjects.CopyTo(tempIgnoredProjects, 0);
            tempIgnoredProjects.SetValue(project.Name, ignoredProjects.Length);
            ignoredProjects = tempIgnoredProjects;
            settingObject.IgnoredProjects = ignoredProjects;
            pluginUI.setTime(new TimeSpan(0));
            pluginUI.setProjectText("Not tracking " + project.Name);
        }
        private void trackProject(IProject project, bool inTrackList)
        {
            log("trackProject " + project.Name + " inTrackList " + inTrackList);
            if (!inTrackList)
            {
                string[] tempTrackedProjects = new string[trackedProjects.Length + 1];
                trackedProjects.CopyTo(tempTrackedProjects, 0);
                tempTrackedProjects.SetValue(project.Name, trackedProjects.Length);
                trackedProjects = tempTrackedProjects;
                settingObject.TrackedProjects = trackedProjects;
                //trackedProjects = project.Name;
            }
            //look for task with project name
            bool inRemoteTasks = false;
            foreach (Task task in tasks)
            {
               log("checking task.Name " + task.Name);
                if (task.Name == project.Name)
                {
                   log("Found matching task " + task.Name);
                    currentTask = task;
                    inRemoteTasks = true;
                }
            }
            if (!inRemoteTasks)
            {
               log("Creating new task");
                currentTask = new Task(project.Name);
                try
                {
                    currentTask = api.UpdateTask(currentTask);
                }
                catch (Exception exception)
                {
                    pluginUI.setStatusText("Error creating task for " + username);
                    log("Error creating task for " + username + " : " + exception.Message);
                }
            }
            if (timeEntry != null)
            {
                submitTimeEntry();
            }
            createNewTimeEntry();
            timer.Start();
        }

        private void submitTimeEntry()
        {
           log("submitTimeEntry");
            if (!loggedIn)
            {
               log("notlogged in");
                return;
            }
            if (timeEntry == null)
            {
               log("no time entry");
                return;
            }
            if (timeEntry.RelatedTask == null || timeEntry.RelatedTask.Id == null || timeEntry.RelatedTask.Id.Length == 0)
            {
               log("no task to submit to");
                return;
            }
            DateTime startTime = timeEntry.StartTime;
            timeEntry.EndTime = DateTime.Now;
            timeEntry.Comments = comments;
           log("timeEntry.EndTime = " + timeEntry.EndTime);
            TimeSpan duration = timeEntry.EndTime.Subtract(timeEntry.StartTime);
           log("timeEntry.StartTime = " + timeEntry.StartTime);
           log("duration = " + duration);
            //int minimumTime = minimumTime;
            if (minimumTime < 1) minimumTime = 1;
            timeEntry.Duration = Convert.ToInt32(Math.Floor(duration.TotalSeconds));
            if (duration.TotalSeconds < minimumTime)
            {
                log("not enough seconds to submit");
                return;
            }
            try
            {
                timeEntry = api.UpdateTimeEntry(timeEntry);
                //replace timeentry with result
                log("timeEntry submitted " + timeEntry.Id);
            }
            catch (Exception exception)
            {
                pluginUI.setStatusText("Error submitting time entry for " + username);
                log("Error submitting time entry for " + username + " : " + exception.Message);
            }
            //set start time back to cached value (submitting returns server time)
            timeEntry.StartTime = startTime;
        }
        private void createNewTimeEntry()
        {
            comments = "";
            //The required fields are StartTime, EndTime, Duration, and RelatedTask.
            timeEntry = new TimeEntry(currentTask);
            timeEntry.StartTime = DateTime.Now;
        }
        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            // Set events you want to listen (combine as flags)
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.Command | EventType.FileClose | EventType.UIRefresh);
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                /*
                case LocaleVersion.fi_FI : 
                    // We have Finnish available... or not. :)
                    LocaleHelper.Initialize(LocaleVersion.fi_FI);
                    break;
                */
                default : 
                    // Plugins should default to English...
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
            this.pluginDesc = LocaleHelper.GetString("Info.Description");
        }
        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = LocaleHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockRight);
            //pluginUI.Show();
            pluginUI.PlayPause += onPlayPause;
        }
        private void onPlayPause(object sender, EventArgs e)
        {
            if (paused)
            {
                paused = false;
            }
            else
            {
                paused = true;
            }
            pluginUI.setPaused(paused);
        }
        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel.Show();
        }

        #endregion

        private void log(string message)
        {
            System.Console.WriteLine(message);
            //TraceManager.Add(message);
        }

	}
	
}
