namespace QueryTest {
    partial class TestForm {
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
			this.updateBtn = new System.Windows.Forms.Button();
			this.mapTxt = new System.Windows.Forms.Label();
			this.downloadButton = new System.Windows.Forms.Button();
			this.addressBox = new System.Windows.Forms.TextBox();
			this.nextMapTxt = new System.Windows.Forms.Label();
			this.autoUpdate_Cb = new System.Windows.Forms.CheckBox();
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// updateBtn
			// 
			this.updateBtn.Location = new System.Drawing.Point(13, 49);
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
			this.mapTxt.Location = new System.Drawing.Point(12, 75);
			this.mapTxt.Name = "mapTxt";
			this.mapTxt.Size = new System.Drawing.Size(35, 13);
			this.mapTxt.TabIndex = 1;
			this.mapTxt.Text = "label1";
			// 
			// downloadButton
			// 
			this.downloadButton.Enabled = false;
			this.downloadButton.Location = new System.Drawing.Point(94, 49);
			this.downloadButton.Name = "downloadButton";
			this.downloadButton.Size = new System.Drawing.Size(75, 23);
			this.downloadButton.TabIndex = 2;
			this.downloadButton.Text = "Download!";
			this.downloadButton.UseVisualStyleBackColor = true;
			this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
			// 
			// addressBox
			// 
			this.addressBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::QueryTest.Properties.Settings.Default, "ServerAddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.addressBox.Location = new System.Drawing.Point(13, 13);
			this.addressBox.Name = "addressBox";
			this.addressBox.Size = new System.Drawing.Size(236, 20);
			this.addressBox.TabIndex = 3;
			this.addressBox.Text = global::QueryTest.Properties.Settings.Default.ServerAddress;
			this.addressBox.TextChanged += new System.EventHandler(this.addressBox_TextChanged);
			// 
			// nextMapTxt
			// 
			this.nextMapTxt.AutoSize = true;
			this.nextMapTxt.Location = new System.Drawing.Point(13, 103);
			this.nextMapTxt.Name = "nextMapTxt";
			this.nextMapTxt.Size = new System.Drawing.Size(35, 13);
			this.nextMapTxt.TabIndex = 4;
			this.nextMapTxt.Text = "label1";
			// 
			// autoUpdate_Cb
			// 
			this.autoUpdate_Cb.AutoSize = true;
			this.autoUpdate_Cb.Location = new System.Drawing.Point(176, 54);
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
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.autoUpdate_Cb);
			this.Controls.Add(this.nextMapTxt);
			this.Controls.Add(this.addressBox);
			this.Controls.Add(this.downloadButton);
			this.Controls.Add(this.mapTxt);
			this.Controls.Add(this.updateBtn);
			this.Name = "TestForm";
			this.Text = "TestForm";
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
	}
}