using Source_Stalker.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            loadStoredServers();

            updateTimer = new System.Timers.Timer(Settings.Default.UpdatePeriod);
            updateTimer.Elapsed += UpdateTimer_Elapsed;

            updateTimer.Start();
            UpdateServerStatuses();
        }

        private void loadStoredServers() {
            servers = new List<ServerStatus>();

            foreach(var servAddr in Settings.Default.Servers) {
                var server = new ServerStatus();
                server.Timeout = Settings.Default.Timeout;
                servers.Add(server);

                var row = new DataGridViewRow();
                serverGrid.Rows.Add(row);

                SetServerListeners(server);

                server.Address = servAddr;
            }
        }

        private void Server_StateChanged(ServerStatus server) {
            CheckDNSSuccess(server);
            UpdateStatusBarWithServerStatus(server);
            SetRowForServer(server);
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

            if(pendingUpdateCount == 1) {
                pendingStatus.Text = "1 server pending";
            } else {
                pendingStatus.Text = $"{pendingUpdateCount} servers pending";
            }
        }

        private void ServerGrid_UserAddedRow(object sender, DataGridViewRowEventArgs e) {
            var server = new ServerStatus();
            server.Timeout = Settings.Default.Timeout;
            SetServerListeners(server);
            servers.Insert(e.Row.Index - 1, server);
        }

        private void serverGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e) {
            servers.RemoveAt(e.Row.Index);
            saveServerList();
        }

        private void saveServerList() {
            var list = new StringCollection();
            foreach(var server in servers) {
                if(server.Address == null) continue;
                list.Add(server.Address);
            }
            Settings.Default.Servers = list;
            Settings.Default.Save();
        }

        private void SetServerListeners(ServerStatus server) {
            server.StateChanged += Server_StateChanged;
        }

        private void ServerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            var row = serverGrid.Rows[e.RowIndex];
            var server = servers[e.RowIndex];
            try {
                server.Address = (string)row.Cells[0].Value;
                SetRowForServer(server, row);
            } catch(ServerStatus.BadAddressException) { }

            saveServerList();
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
            if(server.State == ServerStatus.StateEnum.HOSTNAME_INVALID) {
                row.SetValues(server.Address, "", "0 (0)/0", "Invalid Host");
            } else if(server.State == ServerStatus.StateEnum.TIME_OUT) {
                row.SetValues(server.Address, "", "0 (0)/0", $"-{Settings.Default.Timeout}");
            } else if(server.State == ServerStatus.StateEnum.HOSTNAME_UNRESOLVED) {
                row.SetValues(server.Address, "", "0 (0)/0", "Pending DNS");
            } else if(server.State == ServerStatus.StateEnum.QUERY_SENT) {
                row.SetValues(server.Address, "", "0 (0)/0", "Pending");
            } else if(server.info != null) {
                string playerCountString = $"{server.info.PlayerCount} ({server.info.BotCount})/{server.info.MaxPlayerCount}";
                row.SetValues(server.Address, server.info.Map, playerCountString, server.PingTime.TotalMilliseconds);
            } else {
                row.SetValues(server.Address, "", "0 (0)/0", "N/a");
            }
        }

        private void SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e) {
            updateTimer.Interval = Settings.Default.UpdatePeriod;

            foreach(var server in servers) {
                server.Timeout = Settings.Default.Timeout;
            }
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
            if(server.State == ServerStatus.StateEnum.TIME_OUT) {
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
