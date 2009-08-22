using System;
using System.Collections;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;

namespace SlimTimer
{
	public class PluginUI : UserControl
    {
        private Label labelStatus;
        private Label labelTime;
        private Label labelProject;
        private Button buttonPause;
		private PluginMain pluginMain;
        
		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
			this.pluginMain = pluginMain;
            this.buttonPause.Click += new EventHandler(buttonPause_Click);
		}

        void buttonPause_Click(object sender, EventArgs e)
        {
        }
        public void setPaused(bool paused)
        {
            if(paused){
                buttonPause.Text = "Resume";
            }else{
                buttonPause.Text = "Pause";
            }
        }
        public void setStatusText(String text){
            this.labelStatus.Text = text;
        }
        public void setProjectText(String text)
        {
            labelProject.Text = text;
        }
        public void setTime(TimeSpan time)
        {
            labelTime.Text = time.ToString().Substring(0, 8);
        }
		#region Windows Forms Designer Generated Code

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelProject = new System.Windows.Forms.Label();
            this.buttonPause = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(3, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(35, 13);
            this.labelStatus.TabIndex = 1;
            this.labelStatus.Text = "label1";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(3, 26);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(49, 13);
            this.labelTime.TabIndex = 2;
            this.labelTime.Text = "00:00:00";
            // 
            // labelProject
            // 
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(3, 13);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(0, 13);
            this.labelProject.TabIndex = 3;
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(6, 42);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 4;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonStartStop_Click);
            // 
            // PluginUI
            // 
            //this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.labelProject);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelStatus);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.Load += new System.EventHandler(this.PluginUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private void PluginUI_Load(object sender, EventArgs e)
        {

        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {

        }

				
 	}

}
