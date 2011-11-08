using System;
using System.Collections;
using System.Windows.Forms;
using Inikus.SlimTimer;
using System.Collections.ObjectModel;

namespace SlimTimer.view
{
	public class PluginUI : UserControl
    {
        public event EventHandler OnPause;
        public event EventHandler OnResume;
        private TableLayoutPanel tableLayoutPanel1;
        private Label labelStatus;
        private Label labelProject;
        private Label labelTime;
        private Label label4;
        private Label label5;
        private Label label6;
        private Button buttonPause;
        private Button buttonNoTimeout;
        private ComboBox comboBoxProjects;
        private Label label1;
		private PluginMain pluginMain;
        private bool _paused;
		public PluginUI(PluginMain pluginMain)
		{
            this.InitializeComponent();
            labelStatus.Text = "";
            labelProject.Text = "";
            labelTime.Text = "";
            buttonPause.Text = "Pause";
            buttonPause.Enabled = false;
            buttonPause.Visible = false;
			this.pluginMain = pluginMain;
            this.buttonPause.Click += new EventHandler(buttonPause_Click);
            buttonNoTimeout.Visible = false;
            comboBoxProjects.Visible = false;
		}

        void buttonPause_Click(object sender, EventArgs e)
        {
            if (_paused)
            {
                OnResume(this, new EventArgs());
            }
            else
            {
                OnPause(this, new EventArgs());
            }
        }
        public void setPaused(bool paused)
        {
            _paused = paused;
            if(paused){
                buttonPause.Text = "Resume";
            }else{
                buttonPause.Text = "Pause";
            }
        }
        public void setStatusText(String text){
            labelStatus.Text = text;
        }
        public void setProjectText(String text)
        {
            labelProject.Text = text;
        }
        public void setTime(TimeSpan time)
        {
            labelTime.Text = time.ToString().Substring(0, 8);
        }
        public void setCurrentTask(Task task)
        {
            comboBoxProjects.SelectedItem = task;
        }
        public void setTasks(Collection<Task> tasks)
        {
            comboBoxProjects.DataSource = tasks;
        }
        public void setTracking(bool tracking)
        {
            comboBoxProjects.Enabled = tracking;
        }
		#region Windows Forms Designer Generated Code

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelProject = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxProjects = new System.Windows.Forms.ComboBox();
            this.buttonNoTimeout = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.14382F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.85618F));
            this.tableLayoutPanel1.Controls.Add(this.labelStatus, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelProject, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelTime, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxProjects, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonNoTimeout, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonPause, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.42308F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.42308F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.42308F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.91026F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.91026F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.91026F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(277, 145);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(64, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(35, 13);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "label1";
            this.labelStatus.Click += new System.EventHandler(this.labelStatus_Click);
            // 
            // labelProject
            // 
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(64, 20);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(35, 13);
            this.labelProject.TabIndex = 1;
            this.labelProject.Text = "label2";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(64, 40);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(35, 13);
            this.labelTime.TabIndex = 2;
            this.labelTime.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Status:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Project:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Time:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Track as";
            this.label1.Visible = false;
            // 
            // comboBoxProjects
            // 
            this.comboBoxProjects.DisplayMember = "Name";
            this.comboBoxProjects.Enabled = false;
            this.comboBoxProjects.FormattingEnabled = true;
            this.comboBoxProjects.Location = new System.Drawing.Point(64, 63);
            this.comboBoxProjects.Name = "comboBoxProjects";
            this.comboBoxProjects.Size = new System.Drawing.Size(191, 21);
            this.comboBoxProjects.TabIndex = 8;
            this.comboBoxProjects.ValueMember = "Name";
            this.comboBoxProjects.Visible = false;
            // 
            // buttonNoTimeout
            // 
            this.buttonNoTimeout.Location = new System.Drawing.Point(64, 117);
            this.buttonNoTimeout.Name = "buttonNoTimeout";
            this.buttonNoTimeout.Size = new System.Drawing.Size(75, 23);
            this.buttonNoTimeout.TabIndex = 7;
            this.buttonNoTimeout.Text = "button1";
            this.buttonNoTimeout.UseVisualStyleBackColor = true;
            this.buttonNoTimeout.Visible = false;
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(64, 90);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 21);
            this.buttonPause.TabIndex = 6;
            this.buttonPause.Text = "button1";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Visible = false;
            // 
            // PluginUI
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.Load += new System.EventHandler(this.PluginUI_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private void PluginUI_Load(object sender, EventArgs e)
        {

        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void labelStatus_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

				
 	}

}
