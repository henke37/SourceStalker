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
    public partial class ServerListForm : Form {

        private List<ServerStatus> servers;

        private System.Timers.Timer updateTimer;

        public ServerListForm() {
            InitializeComponent();

            Settings.Default.SettingChanging += SettingChanging;

            servers = new List<ServerStatus>();
            updateTimer = new System.Timers.Timer(Settings.Default.UpdatePeriod);
            updateTimer.Elapsed += UpdateTimer_Elapsed;

            buildGrid();
        }

        private void buildGrid() {
        }

        private void SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e) {
            updateTimer.Interval = Settings.Default.UpdatePeriod;
        }

        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            UpdateServerStatuses();
        }

        private void UpdateServerStatuses() {
            foreach(var server in servers) {
                if(server.state == ServerStatus.State.HOSTNAME_UNRESOLVED) continue;
                if(server.state == ServerStatus.State.INVALID) continue;
                if(server.state == ServerStatus.State.QUERY_SENT) continue;
                server.Update();
            }
        }
    }
}
