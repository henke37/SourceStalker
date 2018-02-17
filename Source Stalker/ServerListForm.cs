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

            updateTimer.Start();
            UpdateServerStatuses();
        }

        private void buildGrid() {
            serverGrid.UserAddedRow += ServerGrid_UserAddedRow;
            serverGrid.CellEndEdit += ServerGrid_CellEndEdit;
            foreach(var server in servers) {
                server.StateChanged += Server_StateChanged;
                var row = new DataGridViewRow();
                SetRowForServer(server, row);
                serverGrid.Rows.Add(row);
            }
        }

        private async void Server_StateChanged(ServerStatus server) {
            if(server.State == ServerStatus.StateEnum.HOSTNAME_RESOLVED) {
                await server.Update();
            }
            SetRowForServer(server);
        }

        private void ServerGrid_UserAddedRow(object sender, DataGridViewRowEventArgs e) {
            var server = new ServerStatus();
            server.StateChanged += Server_StateChanged;
            servers.Insert(e.Row.Index-1, server);
        }

        private void ServerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            var row = serverGrid.Rows.SharedRow(e.RowIndex);
            var server = servers[e.RowIndex];
            server.Address = (string) row.Cells[0].Value;
            SetRowForServer(server, row);
        }

        private void SetRowForServer(ServerStatus server) {
            var index = servers.IndexOf(server);
            var row = serverGrid.Rows[index];
            SetRowForServer(server, row);
        }

        private void SetRowForServer(ServerStatus server, DataGridViewRow row) {
            if(serverGrid.InvokeRequired) {
                serverGrid.Invoke((MethodInvoker)delegate {
                    SetRowForServer(server, row);
                });
                return;
            }
            if(server.State == ServerStatus.StateEnum.TIME_OUT) {
                row.SetValues(server.Address, "", "0 (0)/0", $"-{Settings.Default.Timeout}");
            } else if(server.info !=null) {
                string playerCountString = $"{server.info.PlayerCount} ({server.info.BotCount})/{server.info.MaxPlayerCount}";
                row.SetValues(server.Address, server.info.Map, playerCountString, server.PingTime.TotalMilliseconds);
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

        private async void UpdateServerStatuses() {
            Task[] tasks = new Task[servers.Count];
            var i = 0;
            foreach(var server in servers) {
                if(!server.IsReadyForUpdate) continue;
                tasks[i++]=server.Update();
            }
            await Task.WhenAll(tasks);
        }

        private void updateNowBtn_Click(object sender, EventArgs e) {
            UpdateServerStatuses();
        }

        private void settingsBtn_Click(object sender, EventArgs e) {
            var sf = new SettingsForm();
            sf.Show(this);
        }
    }
}
