﻿namespace Source_Stalker {
    partial class ServerListForm {
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.updateNowBtn = new System.Windows.Forms.ToolStripButton();
			this.settingsBtn = new System.Windows.Forms.ToolStripButton();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.pendingStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.timedoutStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.serverGrid = new System.Windows.Forms.DataGridView();
			this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Map = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PlayerCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Ping = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toolStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.serverGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateNowBtn,
            this.settingsBtn});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(925, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// updateNowBtn
			// 
			this.updateNowBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.updateNowBtn.Image = global::Source_Stalker.Properties.Resources.updateArrow;
			this.updateNowBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.updateNowBtn.Name = "updateNowBtn";
			this.updateNowBtn.Size = new System.Drawing.Size(23, 22);
			this.updateNowBtn.Text = "Update";
			this.updateNowBtn.ToolTipText = "Update all servers now!";
			this.updateNowBtn.Click += new System.EventHandler(this.updateNowBtn_Click);
			// 
			// settingsBtn
			// 
			this.settingsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.settingsBtn.Image = global::Source_Stalker.Properties.Resources.settings_cog;
			this.settingsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.settingsBtn.Name = "settingsBtn";
			this.settingsBtn.Size = new System.Drawing.Size(23, 22);
			this.settingsBtn.Text = "Settings";
			this.settingsBtn.ToolTipText = "Open Settings";
			this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pendingStatus,
            this.timedoutStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 529);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(925, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// pendingStatus
			// 
			this.pendingStatus.Name = "pendingStatus";
			this.pendingStatus.Size = new System.Drawing.Size(99, 17);
			this.pendingStatus.Text = "0 servers pending";
			// 
			// timedoutStatus
			// 
			this.timedoutStatus.Name = "timedoutStatus";
			this.timedoutStatus.Size = new System.Drawing.Size(107, 17);
			this.timedoutStatus.Text = "0 servers timed out";
			// 
			// serverGrid
			// 
			this.serverGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.serverGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address,
            this.Map,
            this.PlayerCount,
            this.Ping});
			this.serverGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.serverGrid.Location = new System.Drawing.Point(0, 25);
			this.serverGrid.Name = "serverGrid";
			this.serverGrid.Size = new System.Drawing.Size(925, 504);
			this.serverGrid.TabIndex = 3;
			this.serverGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.serverGrid_CellDoubleClick);
			this.serverGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.ServerGrid_CellEndEdit);
			this.serverGrid.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.ServerGrid_UserAddedRow);
			this.serverGrid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.serverGrid_UserDeletedRow);
			// 
			// Address
			// 
			this.Address.HeaderText = "Address";
			this.Address.MinimumWidth = 50;
			this.Address.Name = "Address";
			this.Address.Width = 150;
			// 
			// Map
			// 
			this.Map.HeaderText = "Current Map";
			this.Map.MinimumWidth = 100;
			this.Map.Name = "Map";
			this.Map.ReadOnly = true;
			this.Map.ToolTipText = "The map currently being played";
			this.Map.Width = 200;
			// 
			// PlayerCount
			// 
			this.PlayerCount.HeaderText = "Player Count";
			this.PlayerCount.Name = "PlayerCount";
			this.PlayerCount.ReadOnly = true;
			// 
			// Ping
			// 
			this.Ping.HeaderText = "Ping";
			this.Ping.Name = "Ping";
			this.Ping.ReadOnly = true;
			// 
			// ServerListForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(925, 551);
			this.Controls.Add(this.serverGrid);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "ServerListForm";
			this.Text = "Server List";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ServerListForm_FormClosed);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.serverGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel pendingStatus;
        private System.Windows.Forms.DataGridView serverGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn Map;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ping;
        private System.Windows.Forms.ToolStripButton updateNowBtn;
        private System.Windows.Forms.ToolStripButton settingsBtn;
        private System.Windows.Forms.ToolStripStatusLabel timedoutStatus;
    }
}

