namespace TestClientServer
{
    partial class Form2
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
			this.chkStartServer = new System.Windows.Forms.CheckBox();
			this.chkStartClient = new System.Windows.Forms.CheckBox();
			this.nudClient = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.chkServerSendData = new System.Windows.Forms.CheckBox();
			this.chkClientSendData = new System.Windows.Forms.CheckBox();
			this.chkClientDisconnect = new System.Windows.Forms.CheckBox();
			this.chkServerAppend = new System.Windows.Forms.CheckBox();
			this.chkClientAppend = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudClient)).BeginInit();
			this.SuspendLayout();
			// 
			// chkStartServer
			// 
			this.chkStartServer.AutoSize = true;
			this.chkStartServer.Checked = true;
			this.chkStartServer.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkStartServer.Location = new System.Drawing.Point(12, 12);
			this.chkStartServer.Name = "chkStartServer";
			this.chkStartServer.Size = new System.Drawing.Size(82, 17);
			this.chkStartServer.TabIndex = 0;
			this.chkStartServer.Text = "Start Server";
			this.chkStartServer.UseVisualStyleBackColor = true;
			// 
			// chkStartClient
			// 
			this.chkStartClient.AutoSize = true;
			this.chkStartClient.Checked = true;
			this.chkStartClient.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkStartClient.Location = new System.Drawing.Point(12, 46);
			this.chkStartClient.Name = "chkStartClient";
			this.chkStartClient.Size = new System.Drawing.Size(82, 17);
			this.chkStartClient.TabIndex = 1;
			this.chkStartClient.Text = "Start Clients";
			this.chkStartClient.UseVisualStyleBackColor = true;
			// 
			// nudClient
			// 
			this.nudClient.Location = new System.Drawing.Point(12, 69);
			this.nudClient.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.nudClient.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudClient.Name = "nudClient";
			this.nudClient.Size = new System.Drawing.Size(39, 20);
			this.nudClient.TabIndex = 2;
			this.nudClient.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(57, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "clients";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 109);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "Start";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// chkServerSendData
			// 
			this.chkServerSendData.AutoSize = true;
			this.chkServerSendData.Checked = true;
			this.chkServerSendData.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkServerSendData.Location = new System.Drawing.Point(100, 12);
			this.chkServerSendData.Name = "chkServerSendData";
			this.chkServerSendData.Size = new System.Drawing.Size(120, 17);
			this.chkServerSendData.TabIndex = 5;
			this.chkServerSendData.Text = "Send Random Data";
			this.chkServerSendData.UseVisualStyleBackColor = true;
			// 
			// chkClientSendData
			// 
			this.chkClientSendData.AutoSize = true;
			this.chkClientSendData.Checked = true;
			this.chkClientSendData.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkClientSendData.Location = new System.Drawing.Point(100, 46);
			this.chkClientSendData.Name = "chkClientSendData";
			this.chkClientSendData.Size = new System.Drawing.Size(120, 17);
			this.chkClientSendData.TabIndex = 6;
			this.chkClientSendData.Text = "Send Random Data";
			this.chkClientSendData.UseVisualStyleBackColor = true;
			// 
			// chkClientDisconnect
			// 
			this.chkClientDisconnect.AutoSize = true;
			this.chkClientDisconnect.Checked = true;
			this.chkClientDisconnect.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkClientDisconnect.Location = new System.Drawing.Point(100, 69);
			this.chkClientDisconnect.Name = "chkClientDisconnect";
			this.chkClientDisconnect.Size = new System.Drawing.Size(86, 17);
			this.chkClientDisconnect.TabIndex = 7;
			this.chkClientDisconnect.Text = "Disconnect?";
			this.chkClientDisconnect.UseVisualStyleBackColor = true;
			// 
			// chkServerAppend
			// 
			this.chkServerAppend.AutoSize = true;
			this.chkServerAppend.Checked = true;
			this.chkServerAppend.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkServerAppend.Location = new System.Drawing.Point(226, 12);
			this.chkServerAppend.Name = "chkServerAppend";
			this.chkServerAppend.Size = new System.Drawing.Size(112, 17);
			this.chkServerAppend.TabIndex = 8;
			this.chkServerAppend.Text = "Append New Text";
			this.chkServerAppend.UseVisualStyleBackColor = true;
			// 
			// chkClientAppend
			// 
			this.chkClientAppend.AutoSize = true;
			this.chkClientAppend.Checked = true;
			this.chkClientAppend.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkClientAppend.Location = new System.Drawing.Point(226, 46);
			this.chkClientAppend.Name = "chkClientAppend";
			this.chkClientAppend.Size = new System.Drawing.Size(112, 17);
			this.chkClientAppend.TabIndex = 9;
			this.chkClientAppend.Text = "Append New Text";
			this.chkClientAppend.UseVisualStyleBackColor = true;
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(335, 144);
			this.Controls.Add(this.chkClientAppend);
			this.Controls.Add(this.chkServerAppend);
			this.Controls.Add(this.chkClientDisconnect);
			this.Controls.Add(this.chkClientSendData);
			this.Controls.Add(this.chkServerSendData);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudClient);
			this.Controls.Add(this.chkStartClient);
			this.Controls.Add(this.chkStartServer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Form2";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form2";
			this.Load += new System.EventHandler(this.Form2_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudClient)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkStartServer;
        private System.Windows.Forms.CheckBox chkStartClient;
        private System.Windows.Forms.NumericUpDown nudClient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkServerSendData;
        private System.Windows.Forms.CheckBox chkClientSendData;
        private System.Windows.Forms.CheckBox chkClientDisconnect;
		private System.Windows.Forms.CheckBox chkServerAppend;
		private System.Windows.Forms.CheckBox chkClientAppend;
    }
}