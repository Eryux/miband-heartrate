namespace MiBand_Heartrate
{
    partial class ControlFrame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlFrame));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.authButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.heartrateLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.realtimeFileCheck = new System.Windows.Forms.CheckBox();
            this.saveCSVCheckbox = new System.Windows.Forms.CheckBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.continuousModeCheck = new System.Windows.Forms.CheckBox();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.authButton,
            this.toolStripSeparator2,
            this.connectionStatusLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(284, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // authButton
            // 
            this.authButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.authButton.Image = ((System.Drawing.Image)(resources.GetObject("authButton.Image")));
            this.authButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.authButton.Name = "authButton";
            this.authButton.Size = new System.Drawing.Size(23, 22);
            this.authButton.Text = "Authenticate device";
            this.authButton.ToolTipText = "Auth device";
            this.authButton.Click += new System.EventHandler(this.authButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(123, 22);
            this.connectionStatusLabel.Text = "Connected | Not Auth";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.heartrateLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 86);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Heartrate";
            // 
            // heartrateLabel
            // 
            this.heartrateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heartrateLabel.Location = new System.Drawing.Point(6, 16);
            this.heartrateLabel.Name = "heartrateLabel";
            this.heartrateLabel.Size = new System.Drawing.Size(248, 67);
            this.heartrateLabel.TabIndex = 0;
            this.heartrateLabel.Text = "--";
            this.heartrateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.continuousModeCheck);
            this.groupBox2.Controls.Add(this.realtimeFileCheck);
            this.groupBox2.Controls.Add(this.saveCSVCheckbox);
            this.groupBox2.Location = new System.Drawing.Point(12, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 93);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // realtimeFileCheck
            // 
            this.realtimeFileCheck.AutoSize = true;
            this.realtimeFileCheck.Location = new System.Drawing.Point(7, 44);
            this.realtimeFileCheck.Name = "realtimeFileCheck";
            this.realtimeFileCheck.Size = new System.Drawing.Size(152, 17);
            this.realtimeFileCheck.TabIndex = 1;
            this.realtimeFileCheck.Text = "Write heartrate value in file";
            this.realtimeFileCheck.UseVisualStyleBackColor = true;
            this.realtimeFileCheck.CheckedChanged += new System.EventHandler(this.RealtimeFileCheck_CheckedChanged);
            // 
            // saveCSVCheckbox
            // 
            this.saveCSVCheckbox.AutoSize = true;
            this.saveCSVCheckbox.Location = new System.Drawing.Point(7, 20);
            this.saveCSVCheckbox.Name = "saveCSVCheckbox";
            this.saveCSVCheckbox.Size = new System.Drawing.Size(126, 17);
            this.saveCSVCheckbox.TabIndex = 0;
            this.saveCSVCheckbox.Text = "Save data in CSV file";
            this.saveCSVCheckbox.UseVisualStyleBackColor = true;
            this.saveCSVCheckbox.CheckedChanged += new System.EventHandler(this.SaveCSVCheckbox_CheckedChanged);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(12, 224);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(132, 35);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start sensor";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(152, 224);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(120, 35);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // continuousModeCheck
            // 
            this.continuousModeCheck.AutoSize = true;
            this.continuousModeCheck.Location = new System.Drawing.Point(7, 67);
            this.continuousModeCheck.Name = "continuousModeCheck";
            this.continuousModeCheck.Size = new System.Drawing.Size(108, 17);
            this.continuousModeCheck.TabIndex = 2;
            this.continuousModeCheck.Text = "Continuous mode";
            this.continuousModeCheck.UseVisualStyleBackColor = true;
            this.continuousModeCheck.CheckedChanged += new System.EventHandler(this.ContinuousModeCheck_CheckedChanged);
            // 
            // ControlFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 267);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ControlFrame";
            this.ShowIcon = false;
            this.Text = "Mi Band - Heartrate";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlFrame_FormClosing);
            this.Load += new System.EventHandler(this.ControlFrame_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton authButton;
        private System.Windows.Forms.ToolStripLabel connectionStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label heartrateLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox realtimeFileCheck;
        private System.Windows.Forms.CheckBox saveCSVCheckbox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.CheckBox continuousModeCheck;
    }
}