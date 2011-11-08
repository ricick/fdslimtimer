using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using SlimTimer.Resources;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using WeifenLuo.WinFormsUI.Docking;
using SlimTimer.view;
using SlimTimer.vo;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Localization;

namespace SlimTimer.view
{
	public class PluginMain : IPlugin
	{
        public delegate void ChangeFileEventHandler (object sender, ChangeFileEventArgs e);
        public delegate void ChangeProjectEventHandler(object sender, ChangeProjectEventArgs e);
        //event delegates
        public event EventHandler dispose;
        public event EventHandler interaction;
        public event ChangeFileEventHandler changeFile;
        public event ChangeProjectEventHandler changeProject;

        private String pluginName = "SlimTimer";
        private String pluginGuid = "f80042e0-7525-11de-800b-0002a5d5c51b";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "SlimTimer plugin for FlashDevelop 3.";
        private String pluginAuth = "Phil Douglas";
        private String settingFilename;
        private SlimtimerSettings settingObject;
        private DockContent pluginPanel;
        private PluginUI ui;

        public PluginUI Ui
        {
            get { return ui; }
            set { ui = value; }
        }
        private Image pluginImage;

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
            this.AddEventHandlers();
            this.InitLocalization();
            this.CreatePluginPanel();
            this.CreateMenuItem();
            this.StartupPureMVC();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            if (dispose != null) dispose(this, new EventArgs());
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            if (interaction != null) interaction(this, new EventArgs());
            switch (e.Type)
            {
                case EventType.FileOpen:
                    if (changeFile != null) changeFile(this, new ChangeFileEventArgs(PluginBase.MainForm.CurrentDocument));
                    break;

                case EventType.Command:
                    string cmd = (e as DataEvent).Action;
                    String comandType = cmd.ToString();
                    if (cmd == "ProjectManager.Project")
                    {
                        if (changeProject != null) changeProject(this, new ChangeProjectEventArgs(PluginBase.CurrentProject));
                    }
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
        /// Startup PUREMVC app
        /// </summary>
        public void StartupPureMVC()
        {
            ApplicationFacade.getInstance().SendNotification(ApplicationFacade.APPLICATION_STARTUP, this);
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
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.Command | EventType.UIRefresh);
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
            this.ui = new PluginUI(this);
            this.ui.Text = LocaleHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.ui, this.pluginGuid, this.pluginImage, DockState.DockRight);
            //pluginUI.Show();
        }
        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel.Show();
        }

        #endregion


        public int Api
        {
            get { return 1; }
        }

        internal void setSettingObject(SlimtimerSettings slimtimerSettings)
        {
            settingObject = slimtimerSettings;
        }
    }
    public class ChangeFileEventArgs : EventArgs
    {
        public ITabbedDocument file;
        public ChangeFileEventArgs(ITabbedDocument file)
            : base()
        {
            this.file = file;
        }
    }
    public class ChangeProjectEventArgs : EventArgs
    {
        public IProject project;
        public ChangeProjectEventArgs(IProject project)
            : base()
        {
            this.project = project;
        }
    }
}
