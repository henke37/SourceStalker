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
            serverGrid.CellEndEdit += ServerGrid_CellEndEdit;
            foreach(var server in servers) {
                var row = new DataGridViewRow();
                SetRowForServer(server, row);
                serverGrid.Rows.Add(row);
            }
        }

        private void ServerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            var row = serverGrid.Rows.SharedRow(e.RowIndex);
            SetRowForServer(row);
        }

        private void SetRowForServer(DataGridViewRow row) {
            ServerStatus server = serverForRow(row);
            SetRowForServer(server, row);
        }

        private ServerStatus serverForRow(DataGridViewRow row) {
            throw new NotImplementedException();
        }

        private static void SetRowForServer(ServerStatus server, DataGridViewRow row) {
            if(server.state == ServerStatus.State.TIME_OUT) {
                row.SetValues(server.Address, "", "0 (0)/0", $"-{Settings.Default.Timeout}");
            } else if(server.info !=null) {
                string playerCountString = $"{server.info.PlayerCount} ({server.info.BotCount})/{server.info.MaxPlayerCount}";
                row.SetValues(server.Address, server.info.Map, playerCountString, server.PingTime);
            } else {
                row.SetValues(server.Address, "", "0 (0)/0", "N/a");
            }
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
