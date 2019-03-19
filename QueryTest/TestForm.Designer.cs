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
			this.updateBtn = new System.Windows.Forms.Button();
			this.mapTxt = new System.Windows.Forms.Label();
			this.downloadButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// updateBtn
			// 
			this.updateBtn.Location = new System.Drawing.Point(40, 49);
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
			this.mapTxt.Location = new System.Drawing.Point(40, 132);
			this.mapTxt.Name = "mapTxt";
			this.mapTxt.Size = new System.Drawing.Size(35, 13);
			this.mapTxt.TabIndex = 1;
			this.mapTxt.Text = "label1";
			// 
			// downloadButton
			// 
			this.downloadButton.Location = new System.Drawing.Point(148, 49);
			this.downloadButton.Name = "downloadButton";
			this.downloadButton.Size = new System.Drawing.Size(75, 23);
			this.downloadButton.TabIndex = 2;
			this.downloadButton.Text = "Download!";
			this.downloadButton.UseVisualStyleBackColor = true;
			this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.downloadButton);
			this.Controls.Add(this.mapTxt);
			this.Controls.Add(this.updateBtn);
			this.Name = "TestForm";
			this.Text = "TestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.Label mapTxt;
		private System.Windows.Forms.Button downloadButton;
	}
}