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
    internal partial class ServerListForm : Form {

        private ServerManager manager;

        public ServerListForm(ServerManager manager) {
            InitializeComponent();
            this.manager = manager;


            manager.ServerStateChanged += Manager_ServerStateChanged;

            buildGridRows();
        }

        private void Manager_ServerStateChanged(ServerStatus server) {
            UpdateStatusBar();
            SetRowForServer(server);
        }

        private void buildGridRows() {
            foreach(var server in manager) {
                serverGrid.Rows.Add();
                SetRowForServer(server);
            }
        }

        private void UpdateStatusBar() {
            if(InvokeRequired) {
                Invoke((MethodInvoker)UpdateStatusBar);
                return;
            }
            if(manager.TimedOutCount == 1) {
                timedoutStatus.Text = $"1 server timed out";
            } else {
                timedoutStatus.Text = $"{manager.TimedOutCount} servers timed out";
            }

            if(manager.PendingUpdateCount == 1) {
                pendingStatus.Text = "1 server pending";
            } else {
                pendingStatus.Text = $"{manager.PendingUpdateCount} servers pending";
            }
        }

        private void ServerGrid_UserAddedRow(object sender, DataGridViewRowEventArgs e) {
            manager.AddServer(e.Row.Index - 1);
        }

        private void serverGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e) {
            manager.RemoveServer(e.Row.Index);
        }

        private void ServerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            try {
                var row = serverGrid.Rows[e.RowIndex];
                manager.SetServerAddress(e.RowIndex, (string)row.Cells[0].Value);                
            } catch(ServerStatus.BadAddressException) { }
        }

        private void SetRowForServer(ServerStatus server) {
            var index = manager.IndexOf(server);
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
            if(server.State == ServerStatus.QueryState.HOSTNAME_INVALID) {
                row.SetValues(server.Address, "", Resources.ServerCountDummy, Resources.QueryState_InvalidHost);
            } else if(server.State == ServerStatus.QueryState.TIME_OUT) {
                row.SetValues(server.Address, "", Resources.ServerCountDummy, $"-{Settings.Default.Timeout}");
            } else if(server.State == ServerStatus.QueryState.HOSTNAME_UNRESOLVED) {
                row.SetValues(server.Address, "", Resources.ServerCountDummy, Resources.QueryState_PendingDNS);
            } else if(server.State == ServerStatus.QueryState.QUERY_SENT) {
                row.SetValues(server.Address, "", Resources.ServerCountDummy, Resources.QueryState_Pending);
            } else if(server.Info !=null) {
				string playerCountString = string.Format(Resources.ServerCountFormat, server.Info.PlayerCount, server.Info.BotCount, server.Info.MaxPlayerCount);
                row.SetValues(server.Address, server.Info.Map, playerCountString, server.PingTime.TotalMilliseconds);
            } else {
                row.SetValues(server.Address, "", Resources.ServerCountDummy, Resources.QueryState_BadState);
            }
        }

        private void updateNowBtn_Click(object sender, EventArgs e) {
            manager.UpdateServerStatuses();
        }

        private void settingsBtn_Click(object sender, EventArgs e) {
            var sf = new SettingsForm();
            sf.Show(this);
        }

        private void serverGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if(serverGrid.NewRowIndex == e.RowIndex) return;
            var server = manager[e.RowIndex];
            string url = $"steam://connect/{server.Address}";
            Process.Start(url);
        }

		private void ServerListForm_FormClosed(object sender, FormClosedEventArgs e) {
			manager.ServerStateChanged -= Manager_ServerStateChanged;
		}
	}
}
