namespace Source_Stalker {
    partial class SettingsForm {
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            this.timeoutUD = new System.Windows.Forms.NumericUpDown();
            this.updatePeriodUD = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updatePeriodUD)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 6);
            label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(74, 13);
            label1.TabIndex = 2;
            label1.Text = "Update period";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 133);
            label2.Margin = new System.Windows.Forms.Padding(3);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 13);
            label2.TabIndex = 3;
            label2.Text = "Timeout";
            // 
            // timeoutUD
            // 
            this.timeoutUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.timeoutUD.Location = new System.Drawing.Point(145, 130);
            this.timeoutUD.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.timeoutUD.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.timeoutUD.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.timeoutUD.Name = "timeoutUD";
            this.timeoutUD.Size = new System.Drawing.Size(120, 20);
            this.timeoutUD.TabIndex = 0;
            this.timeoutUD.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.timeoutUD.ValueChanged += new System.EventHandler(this.timeoutUD_ValueChanged);
            // 
            // updatePeriodUD
            // 
            this.updatePeriodUD.Increment = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.updatePeriodUD.Location = new System.Drawing.Point(145, 3);
            this.updatePeriodUD.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.updatePeriodUD.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.updatePeriodUD.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.updatePeriodUD.Name = "updatePeriodUD";
            this.updatePeriodUD.Size = new System.Drawing.Size(120, 20);
            this.updatePeriodUD.TabIndex = 1;
            this.updatePeriodUD.ThousandsSeparator = true;
            this.updatePeriodUD.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.updatePeriodUD.ValueChanged += new System.EventHandler(this.updatePeriodUD_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.timeoutUD, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.updatePeriodUD, 1, 0);
            this.tableLayoutPanel1.Controls.Add(label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(label2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 261);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updatePeriodUD)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown timeoutUD;
        private System.Windows.Forms.NumericUpDown updatePeriodUD;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}