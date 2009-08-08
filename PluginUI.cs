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
		private PluginMain pluginMain;
        
		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
			this.pluginMain = pluginMain;
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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(3, 0);
            this.labelStatus.Name = "label1";
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
            this.labelProject.Size = new System.Drawing.Size(69, 13);
            this.labelProject.TabIndex = 3;
            this.labelProject.Text = "";
            // 
            // PluginUI
            // 
            this.Controls.Add(this.labelProject);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelStatus);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

				
 	}

}
