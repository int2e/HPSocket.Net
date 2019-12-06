namespace TcpPortForwarding
{
    partial class Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtWorkThreadCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtConnectionTimeout = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMaxConnectionCount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTargetPort = new System.Windows.Forms.TextBox();
            this.txtTargetAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLocalPort = new System.Windows.Forms.TextBox();
            this.txtLocalAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnServiceSwitch = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.txtWorkThreadCount);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtConnectionTimeout);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtMaxConnectionCount);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtTargetPort);
            this.panel1.Controls.Add(this.txtTargetAddress);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtLocalPort);
            this.panel1.Controls.Add(this.txtLocalAddress);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.txtLog);
            this.panel1.Controls.Add(this.btnServiceSwitch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1214, 789);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox1.Location = new System.Drawing.Point(34, 166);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1140, 2);
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // txtWorkThreadCount
            // 
            this.txtWorkThreadCount.Location = new System.Drawing.Point(956, 194);
            this.txtWorkThreadCount.Name = "txtWorkThreadCount";
            this.txtWorkThreadCount.Size = new System.Drawing.Size(218, 35);
            this.txtWorkThreadCount.TabIndex = 24;
            this.txtWorkThreadCount.Text = "10";
            this.txtWorkThreadCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(828, 199);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 24);
            this.label9.TabIndex = 23;
            this.label9.Text = "工作线程:";
            // 
            // txtConnectionTimeout
            // 
            this.txtConnectionTimeout.Location = new System.Drawing.Point(554, 194);
            this.txtConnectionTimeout.MaxLength = 6;
            this.txtConnectionTimeout.Name = "txtConnectionTimeout";
            this.txtConnectionTimeout.Size = new System.Drawing.Size(218, 35);
            this.txtConnectionTimeout.TabIndex = 22;
            this.txtConnectionTimeout.Text = "6000";
            this.txtConnectionTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(426, 199);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 24);
            this.label7.TabIndex = 19;
            this.label7.Text = "超时时间:";
            // 
            // txtMaxConnectionCount
            // 
            this.txtMaxConnectionCount.Location = new System.Drawing.Point(158, 194);
            this.txtMaxConnectionCount.MaxLength = 5;
            this.txtMaxConnectionCount.Name = "txtMaxConnectionCount";
            this.txtMaxConnectionCount.Size = new System.Drawing.Size(218, 35);
            this.txtMaxConnectionCount.TabIndex = 18;
            this.txtMaxConnectionCount.Text = "5000";
            this.txtMaxConnectionCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 24);
            this.label6.TabIndex = 17;
            this.label6.Text = "最大连接:";
            // 
            // txtTargetPort
            // 
            this.txtTargetPort.Location = new System.Drawing.Point(681, 105);
            this.txtTargetPort.MaxLength = 5;
            this.txtTargetPort.Name = "txtTargetPort";
            this.txtTargetPort.Size = new System.Drawing.Size(218, 35);
            this.txtTargetPort.TabIndex = 16;
            this.txtTargetPort.Text = "5555";
            this.txtTargetPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtTargetAddress
            // 
            this.txtTargetAddress.Location = new System.Drawing.Point(681, 42);
            this.txtTargetAddress.Name = "txtTargetAddress";
            this.txtTargetAddress.Size = new System.Drawing.Size(218, 35);
            this.txtTargetAddress.TabIndex = 15;
            this.txtTargetAddress.Text = "127.0.0.1";
            this.txtTargetAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(553, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 24);
            this.label4.TabIndex = 14;
            this.label4.Text = "目标端口:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(553, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 24);
            this.label5.TabIndex = 13;
            this.label5.Text = "目标地址:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(426, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 24);
            this.label3.TabIndex = 12;
            this.label3.Text = "转发到";
            // 
            // txtLocalPort
            // 
            this.txtLocalPort.Location = new System.Drawing.Point(158, 105);
            this.txtLocalPort.MaxLength = 5;
            this.txtLocalPort.Name = "txtLocalPort";
            this.txtLocalPort.Size = new System.Drawing.Size(218, 35);
            this.txtLocalPort.TabIndex = 11;
            this.txtLocalPort.Text = "7000";
            this.txtLocalPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLocalPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberTextBoxKeyPress);
            // 
            // txtLocalAddress
            // 
            this.txtLocalAddress.Location = new System.Drawing.Point(158, 42);
            this.txtLocalAddress.Name = "txtLocalAddress";
            this.txtLocalAddress.Size = new System.Drawing.Size(218, 35);
            this.txtLocalAddress.TabIndex = 10;
            this.txtLocalAddress.Text = "0.0.0.0";
            this.txtLocalAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 24);
            this.label2.TabIndex = 9;
            this.label2.Text = "本地端口:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 24);
            this.label1.TabIndex = 8;
            this.label1.Text = "本地地址:";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClear.Location = new System.Drawing.Point(1012, 252);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(162, 54);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Location = new System.Drawing.Point(34, 313);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1140, 442);
            this.txtLog.TabIndex = 6;
            // 
            // btnServiceSwitch
            // 
            this.btnServiceSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnServiceSwitch.Location = new System.Drawing.Point(954, 58);
            this.btnServiceSwitch.Margin = new System.Windows.Forms.Padding(4);
            this.btnServiceSwitch.Name = "btnServiceSwitch";
            this.btnServiceSwitch.Size = new System.Drawing.Size(220, 70);
            this.btnServiceSwitch.TabIndex = 5;
            this.btnServiceSwitch.Text = "启动";
            this.btnServiceSwitch.UseVisualStyleBackColor = true;
            this.btnServiceSwitch.Click += new System.EventHandler(this.BtnConnectSwitch_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 267);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(958, 24);
            this.label8.TabIndex = 26;
            this.label8.Text = "提示: 工作线程数是组件内部server和agent的每组件的工作线程数, 设置为10就是各10个";
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1214, 789);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TcpPortForwarding";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAgent_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnServiceSwitch;
        private System.Windows.Forms.TextBox txtTargetPort;
        private System.Windows.Forms.TextBox txtTargetAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLocalPort;
        private System.Windows.Forms.TextBox txtLocalAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMaxConnectionCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtWorkThreadCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtConnectionTimeout;
        private System.Windows.Forms.Label label8;
    }
}

