using Source_Stalker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Source_Stalker {
    public partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            timeoutUD.Value = Settings.Default.Timeout;
            updatePeriodUD.Value = Settings.Default.UpdatePeriod;
        }

        private void timeoutUD_ValueChanged(object sender, EventArgs e) {
            Settings.Default.Timeout = (int)timeoutUD.Value;
        }

        private void updatePeriodUD_ValueChanged(object sender, EventArgs e) {
            Settings.Default.UpdatePeriod = (int)updatePeriodUD.Value;
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e) {
            Settings.Default.Save();
        }
    }
}
