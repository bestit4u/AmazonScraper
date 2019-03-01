namespace PCMSnotification
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupSetting = new System.Windows.Forms.GroupBox();
            this.lbltime = new System.Windows.Forms.Label();
            this.btn_proxy = new System.Windows.Forms.Button();
            this.txt_proxypath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnboth = new System.Windows.Forms.RadioButton();
            this.rbtnproxy = new System.Windows.Forms.RadioButton();
            this.rbtnno = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnseller = new System.Windows.Forms.RadioButton();
            this.rbtnname = new System.Windows.Forms.RadioButton();
            this.cmbThread = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbDelay = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblpercent = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.proBar = new System.Windows.Forms.ProgressBar();
            this.btnSavePath = new System.Windows.Forms.Button();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnLoadExcel = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timerDaily = new System.Windows.Forms.Timer(this.components);
            this.btn_proxyusage = new System.Windows.Forms.Button();
            this.txt_proxyusage = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupSetting.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupSetting
            // 
            this.groupSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupSetting.Controls.Add(this.btn_proxyusage);
            this.groupSetting.Controls.Add(this.txt_proxyusage);
            this.groupSetting.Controls.Add(this.label9);
            this.groupSetting.Controls.Add(this.lbltime);
            this.groupSetting.Controls.Add(this.btn_proxy);
            this.groupSetting.Controls.Add(this.txt_proxypath);
            this.groupSetting.Controls.Add(this.label2);
            this.groupSetting.Controls.Add(this.groupBox2);
            this.groupSetting.Controls.Add(this.groupBox1);
            this.groupSetting.Controls.Add(this.cmbThread);
            this.groupSetting.Controls.Add(this.label8);
            this.groupSetting.Controls.Add(this.label7);
            this.groupSetting.Controls.Add(this.cmbDelay);
            this.groupSetting.Controls.Add(this.label6);
            this.groupSetting.Controls.Add(this.label5);
            this.groupSetting.Controls.Add(this.lblpercent);
            this.groupSetting.Controls.Add(this.label4);
            this.groupSetting.Controls.Add(this.proBar);
            this.groupSetting.Controls.Add(this.btnSavePath);
            this.groupSetting.Controls.Add(this.txtSavePath);
            this.groupSetting.Controls.Add(this.label1);
            this.groupSetting.Controls.Add(this.btnClose);
            this.groupSetting.Controls.Add(this.btnStart);
            this.groupSetting.Controls.Add(this.lblStatus);
            this.groupSetting.Controls.Add(this.btnLoadExcel);
            this.groupSetting.Controls.Add(this.txtFileName);
            this.groupSetting.Controls.Add(this.label3);
            this.groupSetting.Location = new System.Drawing.Point(12, 12);
            this.groupSetting.Name = "groupSetting";
            this.groupSetting.Size = new System.Drawing.Size(723, 441);
            this.groupSetting.TabIndex = 3;
            this.groupSetting.TabStop = false;
            // 
            // lbltime
            // 
            this.lbltime.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lbltime.AutoSize = true;
            this.lbltime.Location = new System.Drawing.Point(500, 327);
            this.lbltime.Name = "lbltime";
            this.lbltime.Size = new System.Drawing.Size(0, 13);
            this.lbltime.TabIndex = 43;
            // 
            // btn_proxy
            // 
            this.btn_proxy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_proxy.Location = new System.Drawing.Point(686, 141);
            this.btn_proxy.Name = "btn_proxy";
            this.btn_proxy.Size = new System.Drawing.Size(26, 23);
            this.btn_proxy.TabIndex = 4;
            this.btn_proxy.Text = "...";
            this.btn_proxy.UseVisualStyleBackColor = true;
            this.btn_proxy.Click += new System.EventHandler(this.btn_proxy_Click);
            // 
            // txt_proxypath
            // 
            this.txt_proxypath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_proxypath.Location = new System.Drawing.Point(356, 143);
            this.txt_proxypath.Name = "txt_proxypath";
            this.txt_proxypath.ReadOnly = true;
            this.txt_proxypath.Size = new System.Drawing.Size(324, 20);
            this.txt_proxypath.TabIndex = 53;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(265, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Proxy List Path";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnboth);
            this.groupBox2.Controls.Add(this.rbtnproxy);
            this.groupBox2.Controls.Add(this.rbtnno);
            this.groupBox2.Location = new System.Drawing.Point(14, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 106);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proxy Usage Mode";
            // 
            // rbtnboth
            // 
            this.rbtnboth.AutoSize = true;
            this.rbtnboth.Location = new System.Drawing.Point(18, 75);
            this.rbtnboth.Name = "rbtnboth";
            this.rbtnboth.Size = new System.Drawing.Size(149, 17);
            this.rbtnboth.TabIndex = 2;
            this.rbtnboth.TabStop = true;
            this.rbtnboth.Text = "Use Proxies and Local IPs";
            this.rbtnboth.UseVisualStyleBackColor = true;
            this.rbtnboth.CheckedChanged += new System.EventHandler(this.rbtnboth_CheckedChanged);
            // 
            // rbtnproxy
            // 
            this.rbtnproxy.AutoSize = true;
            this.rbtnproxy.Location = new System.Drawing.Point(18, 47);
            this.rbtnproxy.Name = "rbtnproxy";
            this.rbtnproxy.Size = new System.Drawing.Size(105, 17);
            this.rbtnproxy.TabIndex = 1;
            this.rbtnproxy.TabStop = true;
            this.rbtnproxy.Text = "Use Only Proxies";
            this.rbtnproxy.UseVisualStyleBackColor = true;
            this.rbtnproxy.CheckedChanged += new System.EventHandler(this.rbtnproxy_CheckedChanged);
            // 
            // rbtnno
            // 
            this.rbtnno.AutoSize = true;
            this.rbtnno.Location = new System.Drawing.Point(18, 20);
            this.rbtnno.Name = "rbtnno";
            this.rbtnno.Size = new System.Drawing.Size(101, 17);
            this.rbtnno.TabIndex = 0;
            this.rbtnno.TabStop = true;
            this.rbtnno.Text = "Not Use Proxies";
            this.rbtnno.UseVisualStyleBackColor = true;
            this.rbtnno.CheckedChanged += new System.EventHandler(this.rbtnno_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnseller);
            this.groupBox1.Controls.Add(this.rbtnname);
            this.groupBox1.Location = new System.Drawing.Point(14, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 85);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scrap Mode";
            // 
            // rbtnseller
            // 
            this.rbtnseller.AutoSize = true;
            this.rbtnseller.Location = new System.Drawing.Point(18, 51);
            this.rbtnseller.Name = "rbtnseller";
            this.rbtnseller.Size = new System.Drawing.Size(160, 17);
            this.rbtnseller.TabIndex = 1;
            this.rbtnseller.TabStop = true;
            this.rbtnseller.Text = "Scrap By AmazonSellerLinks";
            this.rbtnseller.UseVisualStyleBackColor = true;
            this.rbtnseller.CheckedChanged += new System.EventHandler(this.rbtnseller_CheckedChanged);
            // 
            // rbtnname
            // 
            this.rbtnname.AutoSize = true;
            this.rbtnname.Location = new System.Drawing.Point(18, 25);
            this.rbtnname.Name = "rbtnname";
            this.rbtnname.Size = new System.Drawing.Size(168, 17);
            this.rbtnname.TabIndex = 0;
            this.rbtnname.TabStop = true;
            this.rbtnname.Text = "Scrap By AmazonSearchLinks";
            this.rbtnname.UseVisualStyleBackColor = true;
            this.rbtnname.CheckedChanged += new System.EventHandler(this.rbtnname_CheckedChanged);
            // 
            // cmbThread
            // 
            this.cmbThread.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmbThread.FormattingEnabled = true;
            this.cmbThread.ItemHeight = 13;
            this.cmbThread.Location = new System.Drawing.Point(520, 355);
            this.cmbThread.Name = "cmbThread";
            this.cmbThread.Size = new System.Drawing.Size(46, 21);
            this.cmbThread.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(385, 356);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(132, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Number of Scrap Threads:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(121, 356);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "S";
            // 
            // cmbDelay
            // 
            this.cmbDelay.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmbDelay.FormattingEnabled = true;
            this.cmbDelay.ItemHeight = 13;
            this.cmbDelay.Location = new System.Drawing.Point(78, 355);
            this.cmbDelay.Name = "cmbDelay";
            this.cmbDelay.Size = new System.Drawing.Size(40, 21);
            this.cmbDelay.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 356);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Delay Time:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(384, 327);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Elapsed/Remain Time:";
            // 
            // lblpercent
            // 
            this.lblpercent.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblpercent.AutoSize = true;
            this.lblpercent.Location = new System.Drawing.Point(86, 328);
            this.lblpercent.Name = "lblpercent";
            this.lblpercent.Size = new System.Drawing.Size(0, 13);
            this.lblpercent.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 327);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Percentage:";
            // 
            // proBar
            // 
            this.proBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.proBar.Location = new System.Drawing.Point(14, 287);
            this.proBar.Name = "proBar";
            this.proBar.Size = new System.Drawing.Size(698, 23);
            this.proBar.TabIndex = 25;
            // 
            // btnSavePath
            // 
            this.btnSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePath.Location = new System.Drawing.Point(686, 90);
            this.btnSavePath.Name = "btnSavePath";
            this.btnSavePath.Size = new System.Drawing.Size(26, 23);
            this.btnSavePath.TabIndex = 3;
            this.btnSavePath.Text = "...";
            this.btnSavePath.UseVisualStyleBackColor = true;
            this.btnSavePath.Click += new System.EventHandler(this.btnSavePath_Click);
            // 
            // txtSavePath
            // 
            this.txtSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSavePath.Location = new System.Drawing.Point(356, 92);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.ReadOnly = true;
            this.txtSavePath.Size = new System.Drawing.Size(324, 20);
            this.txtSavePath.TabIndex = 52;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(265, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Save Path";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnClose.Location = new System.Drawing.Point(549, 399);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 29);
            this.btnClose.TabIndex = 22;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStart.Location = new System.Drawing.Point(97, 399);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 29);
            this.btnStart.TabIndex = 21;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(15, 258);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(697, 26);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "Here, will show the status!";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLoadExcel
            // 
            this.btnLoadExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadExcel.Location = new System.Drawing.Point(686, 17);
            this.btnLoadExcel.Name = "btnLoadExcel";
            this.btnLoadExcel.Size = new System.Drawing.Size(26, 23);
            this.btnLoadExcel.TabIndex = 2;
            this.btnLoadExcel.Text = "...";
            this.btnLoadExcel.UseVisualStyleBackColor = true;
            this.btnLoadExcel.Click += new System.EventHandler(this.btnLoadExcel_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Location = new System.Drawing.Point(356, 19);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(324, 20);
            this.txtFileName.TabIndex = 51;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Filename to Load";
            // 
            // timerDaily
            // 
            this.timerDaily.Interval = 1000;
            this.timerDaily.Tick += new System.EventHandler(this.timerDelay_Tick);
            // 
            // btn_proxyusage
            // 
            this.btn_proxyusage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_proxyusage.Location = new System.Drawing.Point(686, 195);
            this.btn_proxyusage.Name = "btn_proxyusage";
            this.btn_proxyusage.Size = new System.Drawing.Size(26, 23);
            this.btn_proxyusage.TabIndex = 5;
            this.btn_proxyusage.Text = "...";
            this.btn_proxyusage.UseVisualStyleBackColor = true;
            this.btn_proxyusage.Click += new System.EventHandler(this.btn_proxyusage_Click);
            // 
            // txt_proxyusage
            // 
            this.txt_proxyusage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_proxyusage.Location = new System.Drawing.Point(356, 197);
            this.txt_proxyusage.Name = "txt_proxyusage";
            this.txt_proxyusage.ReadOnly = true;
            this.txt_proxyusage.Size = new System.Drawing.Size(324, 20);
            this.txt_proxyusage.TabIndex = 56;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(265, 202);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 55;
            this.label9.Text = "Proxy Usage";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 465);
            this.Controls.Add(this.groupSetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Amazon Scraper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupSetting.ResumeLayout(false);
            this.groupSetting.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupSetting;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnLoadExcel;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timerDaily;
        private System.Windows.Forms.Button btnSavePath;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar proBar;
        private System.Windows.Forms.Label lblpercent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbDelay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbThread;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnseller;
        private System.Windows.Forms.RadioButton rbtnname;
        private System.Windows.Forms.Button btn_proxy;
        private System.Windows.Forms.TextBox txt_proxypath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbtnboth;
        private System.Windows.Forms.RadioButton rbtnproxy;
        private System.Windows.Forms.RadioButton rbtnno;
        private System.Windows.Forms.Label lbltime;
        private System.Windows.Forms.Button btn_proxyusage;
        private System.Windows.Forms.TextBox txt_proxyusage;
        private System.Windows.Forms.Label label9;
    }
}

