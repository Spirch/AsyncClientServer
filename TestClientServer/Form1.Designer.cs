namespace TestClientServer
{
    partial class Form1
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
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.button6 = new System.Windows.Forms.Button();
			this.chkTimer = new System.Windows.Forms.CheckBox();
			this.chkData = new System.Windows.Forms.CheckBox();
			this.chkDisconnect = new System.Windows.Forms.CheckBox();
			this.chkAppend = new System.Windows.Forms.CheckBox();
			this.button7 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(13, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Server";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(546, 14);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "Client";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(627, 14);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "Disconnect";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(3, 43);
			this.textBox1.MaxLength = 5000;
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(875, 230);
			this.textBox1.TabIndex = 3;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(15, 283);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(668, 20);
			this.textBox2.TabIndex = 4;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(94, 14);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 5;
			this.button4.Text = "Disconnect";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(708, 14);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 6;
			this.button5.Text = "Flood";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(689, 286);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(175, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "<--- type something then press enter";
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(789, 14);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 8;
			this.button6.Text = "Client ID?";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// chkTimer
			// 
			this.chkTimer.AutoSize = true;
			this.chkTimer.Location = new System.Drawing.Point(184, 14);
			this.chkTimer.Name = "chkTimer";
			this.chkTimer.Size = new System.Drawing.Size(58, 17);
			this.chkTimer.TabIndex = 9;
			this.chkTimer.Text = "Timer?";
			this.chkTimer.UseVisualStyleBackColor = true;
			this.chkTimer.CheckedChanged += new System.EventHandler(this.chkTimer_CheckedChanged);
			// 
			// chkData
			// 
			this.chkData.AutoSize = true;
			this.chkData.Location = new System.Drawing.Point(248, 14);
			this.chkData.Name = "chkData";
			this.chkData.Size = new System.Drawing.Size(81, 17);
			this.chkData.TabIndex = 10;
			this.chkData.Text = "Send data?";
			this.chkData.UseVisualStyleBackColor = true;
			// 
			// chkDisconnect
			// 
			this.chkDisconnect.AutoSize = true;
			this.chkDisconnect.Location = new System.Drawing.Point(417, 13);
			this.chkDisconnect.Name = "chkDisconnect";
			this.chkDisconnect.Size = new System.Drawing.Size(123, 17);
			this.chkDisconnect.TabIndex = 11;
			this.chkDisconnect.Text = "Random Disconnect";
			this.chkDisconnect.UseVisualStyleBackColor = true;
			// 
			// chkAppend
			// 
			this.chkAppend.AutoSize = true;
			this.chkAppend.Location = new System.Drawing.Point(336, 13);
			this.chkAppend.Name = "chkAppend";
			this.chkAppend.Size = new System.Drawing.Size(63, 17);
			this.chkAppend.TabIndex = 12;
			this.chkAppend.Text = "Append";
			this.chkAppend.UseVisualStyleBackColor = true;
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(561, 187);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 23);
			this.button7.TabIndex = 13;
			this.button7.Text = "button7";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(683, 75);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(147, 134);
			this.listBox1.TabIndex = 14;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(882, 310);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.chkAppend);
			this.Controls.Add(this.chkDisconnect);
			this.Controls.Add(this.chkData);
			this.Controls.Add(this.chkTimer);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox chkTimer;
        private System.Windows.Forms.CheckBox chkData;
        private System.Windows.Forms.CheckBox chkDisconnect;
		private System.Windows.Forms.CheckBox chkAppend;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.ListBox listBox1;
    }
}