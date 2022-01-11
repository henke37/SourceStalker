namespace QueryTest {
    partial class PredownloadForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this.updateBtn = new System.Windows.Forms.Button();
			this.mapTxt = new System.Windows.Forms.Label();
			this.downloadButton = new System.Windows.Forms.Button();
			this.nextMapTxt = new System.Windows.Forms.Label();
			this.autoUpdate_Cb = new System.Windows.Forms.CheckBox();
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.AutoDownload_cb = new System.Windows.Forms.CheckBox();
			this.downloadRoot_txt = new System.Windows.Forms.TextBox();
			this.addressBox = new System.Windows.Forms.TextBox();
			this.httpErrCode = new System.Windows.Forms.Label();
			this.rules_btn = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 9);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(78, 13);
			label1.TabIndex = 8;
			label1.Text = "Server address";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 48);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(113, 13);
			label2.TabIndex = 9;
			label2.Text = "Fastdownload address";
			// 
			// updateBtn
			// 
			this.updateBtn.Location = new System.Drawing.Point(12, 90);
			this.updateBtn.Name = "updateBtn";
			this.updateBtn.Size = new System.Drawing.Size(75, 23);
			this.updateBtn.TabIndex = 0;
			this.updateBtn.Text = "Update!";
			this.updateBtn.UseVisualStyleBackColor = true;
			this.updateBtn.Click += new System.EventHandler(this.updateBtn_ClickAsync);
			// 
			// mapTxt
			// 
			this.mapTxt.AutoSize = true;
			this.mapTxt.Location = new System.Drawing.Point(9, 121);
			this.mapTxt.Name = "mapTxt";
			this.mapTxt.Size = new System.Drawing.Size(77, 13);
			this.mapTxt.TabIndex = 1;
			this.mapTxt.Text = "%currectMap%";
			// 
			// downloadButton
			// 
			this.downloadButton.Enabled = false;
			this.downloadButton.Location = new System.Drawing.Point(91, 90);
			this.downloadButton.Name = "downloadButton";
			this.downloadButton.Size = new System.Drawing.Size(75, 23);
			this.downloadButton.TabIndex = 2;
			this.downloadButton.Text = "Download!";
			this.downloadButton.UseVisualStyleBackColor = true;
			this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
			// 
			// nextMapTxt
			// 
			this.nextMapTxt.AutoSize = true;
			this.nextMapTxt.Location = new System.Drawing.Point(9, 141);
			this.nextMapTxt.Name = "nextMapTxt";
			this.nextMapTxt.Size = new System.Drawing.Size(64, 13);
			this.nextMapTxt.TabIndex = 4;
			this.nextMapTxt.Text = "%nextMap%";
			// 
			// autoUpdate_Cb
			// 
			this.autoUpdate_Cb.AutoSize = true;
			this.autoUpdate_Cb.Location = new System.Drawing.Point(172, 119);
			this.autoUpdate_Cb.Name = "autoUpdate_Cb";
			this.autoUpdate_Cb.Size = new System.Drawing.Size(84, 17);
			this.autoUpdate_Cb.TabIndex = 5;
			this.autoUpdate_Cb.Text = "Auto update";
			this.autoUpdate_Cb.UseVisualStyleBackColor = true;
			this.autoUpdate_Cb.CheckedChanged += new System.EventHandler(this.AutoUpdate_Cb_CheckedChanged);
			// 
			// UpdateTimer
			// 
			this.UpdateTimer.Interval = 30000;
			this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
			// 
			// AutoDownload_cb
			// 
			this.AutoDownload_cb.AutoSize = true;
			this.AutoDownload_cb.Location = new System.Drawing.Point(172, 143);
			this.AutoDownload_cb.Name = "AutoDownload_cb";
			this.AutoDownload_cb.Size = new System.Drawing.Size(97, 17);
			this.AutoDownload_cb.TabIndex = 6;
			this.AutoDownload_cb.Text = "Auto download";
			this.AutoDownload_cb.UseVisualStyleBackColor = true;
			// 
			// downloadRoot_txt
			// 
			this.downloadRoot_txt.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::QueryTest.Properties.Settings.Default, "DownloadUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.downloadRoot_txt.Location = new System.Drawing.Point(12, 64);
			this.downloadRoot_txt.Name = "downloadRoot_txt";
			this.downloadRoot_txt.Size = new System.Drawing.Size(257, 20);
			this.downloadRoot_txt.TabIndex = 7;
			this.downloadRoot_txt.Text = global::QueryTest.Properties.Settings.Default.DownloadUrl;
			this.downloadRoot_txt.TextChanged += new System.EventHandler(this.DownloadRoot_txt_TextChanged);
			// 
			// addressBox
			// 
			this.addressBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::QueryTest.Properties.Settings.Default, "ServerAddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.addressBox.Location = new System.Drawing.Point(12, 25);
			this.addressBox.Name = "addressBox";
			this.addressBox.Size = new System.Drawing.Size(260, 20);
			this.addressBox.TabIndex = 3;
			this.addressBox.Text = global::QueryTest.Properties.Settings.Default.ServerAddress;
			this.addressBox.TextChanged += new System.EventHandler(this.addressBox_TextChanged);
			// 
			// httpErrCode
			// 
			this.httpErrCode.AutoSize = true;
			this.httpErrCode.ForeColor = System.Drawing.Color.Red;
			this.httpErrCode.Location = new System.Drawing.Point(172, 167);
			this.httpErrCode.Name = "httpErrCode";
			this.httpErrCode.Size = new System.Drawing.Size(30, 13);
			this.httpErrCode.TabIndex = 10;
			this.httpErrCode.Text = "ERR";
			this.httpErrCode.Visible = false;
			// 
			// rules_btn
			// 
			this.rules_btn.Enabled = false;
			this.rules_btn.Location = new System.Drawing.Point(172, 90);
			this.rules_btn.Name = "rules_btn";
			this.rules_btn.Size = new System.Drawing.Size(75, 23);
			this.rules_btn.TabIndex = 11;
			this.rules_btn.Text = "Rules";
			this.rules_btn.UseVisualStyleBackColor = true;
			this.rules_btn.Click += new System.EventHandler(this.rules_btn_Click);
			// 
			// PredownloadForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 191);
			this.Controls.Add(this.rules_btn);
			this.Controls.Add(this.httpErrCode);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Controls.Add(this.downloadRoot_txt);
			this.Controls.Add(this.AutoDownload_cb);
			this.Controls.Add(this.autoUpdate_Cb);
			this.Controls.Add(this.nextMapTxt);
			this.Controls.Add(this.addressBox);
			this.Controls.Add(this.downloadButton);
			this.Controls.Add(this.mapTxt);
			this.Controls.Add(this.updateBtn);
			this.Name = "PredownloadForm";
			this.Text = "Predownloader";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TestForm_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.Label mapTxt;
		private System.Windows.Forms.Button downloadButton;
		private System.Windows.Forms.TextBox addressBox;
		private System.Windows.Forms.Label nextMapTxt;
		private System.Windows.Forms.CheckBox autoUpdate_Cb;
		private System.Windows.Forms.Timer UpdateTimer;
		private System.Windows.Forms.CheckBox AutoDownload_cb;
		private System.Windows.Forms.TextBox downloadRoot_txt;
		private System.Windows.Forms.Label httpErrCode;
		private System.Windows.Forms.Button rules_btn;
	}
}