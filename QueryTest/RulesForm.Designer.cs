
namespace QueryTest {
	partial class RulesForm {
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
			this.propGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// propGrid
			// 
			this.propGrid.Location = new System.Drawing.Point(13, 13);
			this.propGrid.Name = "propGrid";
			this.propGrid.Size = new System.Drawing.Size(423, 425);
			this.propGrid.TabIndex = 0;
			// 
			// RulesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(448, 450);
			this.Controls.Add(this.propGrid);
			this.Name = "RulesForm";
			this.Text = "RulesForm";
			this.Load += new System.EventHandler(this.RulesForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propGrid;
	}
}