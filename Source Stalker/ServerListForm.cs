using Source_Stalker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Source_Stalker {
    public partial class ServerListForm : Form {

        private List<ServerStatus> servers;

        private System.Timers.Timer updateTimer;
        private int pendingUpdateCount;
        private int timedOutCount;

        public ServerListForm() {
            InitializeComponent();

            pendingUpdateCount = 0;
            timedOutCount = 0;

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
                SetServerListeners(server);
                var row = new DataGridViewRow();
                SetRowForServer(server, row);
                serverGrid.Rows.Add(row);
            }
        }

        private void Server_StateChanged(ServerStatus server) {
            CheckDNSSuccess(server);
            UpdateStatusBarWithServerStatus(server);
            SetRowForServer(server);
            ServerResourceGatherer.Gather(server);
        }

        private void UpdateStatusBarWithServerStatus(ServerStatus server) {
            if(server.State == ServerStatus.StateEnum.ANSWER_RECEIVED || server.State == ServerStatus.StateEnum.TIME_OUT) {
                pendingUpdateCount--;
            }
            if(server.State == ServerStatus.StateEnum.TIME_OUT) {
                timedOutCount++;
            }
            UpdateStatusBar();
        }

        private async void CheckDNSSuccess(ServerStatus server) {
            if(server.State == ServerStatus.StateEnum.HOSTNAME_RESOLVED) {
                await updateServer(server);
            }
        }

        private void UpdateStatusBar() {
            if(InvokeRequired) {
                Invoke((MethodInvoker)UpdateStatusBar);
                return;
            }
            if(timedOutCount == 1) {
                timedoutStatus.Text = $"1 server timed out";
            } else { 
                timedoutStatus.Text = $"{timedOutCount} servers timed out";
            }

            if(pendingUpdateCount==1) {
                pendingStatus.Text = "1 server pending";
            } else {
                pendingStatus.Text = $"{pendingUpdateCount} servers pending";
            }
        }

        private void ServerGrid_UserAddedRow(object sender, DataGridViewRowEventArgs e) {
            var server = new ServerStatus();
            SetServerListeners(server);
            servers.Insert(e.Row.Index - 1, server);
        }

        private void SetServerListeners(ServerStatus server) {
            server.StateChanged += Server_StateChanged;
        }

        private void ServerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            var row = serverGrid.Rows.SharedRow(e.RowIndex);
            var server = servers[e.RowIndex];
            try {
                server.Address = (string)row.Cells[0].Value;
                SetRowForServer(server, row);
            } catch(ServerStatus.BadAddressException) { }
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
            List<Task> tasks = new List<Task>();
            foreach(var server in servers) {
                if(!server.IsReadyForUpdate) continue;
                tasks.Add(updateServer(server));
            }
            await Task.WhenAll(tasks);
        }

        private Task updateServer(ServerStatus server) {
            if(server.State==ServerStatus.StateEnum.TIME_OUT) {
                timedOutCount--;
            }

            pendingUpdateCount++;
            UpdateStatusBar();
            return server.Update();
        }

        private void updateNowBtn_Click(object sender, EventArgs e) {
            UpdateServerStatuses();
        }

        private void settingsBtn_Click(object sender, EventArgs e) {
            var sf = new SettingsForm();
            sf.Show(this);
        }

        private void serverGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if(serverGrid.NewRowIndex == e.RowIndex) return;
            var server = servers[e.RowIndex];
            string url = $"steam://connect/{server.Address}";
            Process.Start(url);
        }
    }
}
