using Henke37.Valve.Source.ServerQuery;
using Source_Stalker.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Source_Stalker {
    class ServerManager {

        private List<ServerStatus> servers;

        private System.Timers.Timer updateTimer;
        private int pendingUpdateCount;
        private int timedOutCount;

        public int PendingUpdateCount { get => pendingUpdateCount; }
        public int TimedOutCount { get => timedOutCount; }

        public event Action<ServerStatus> ServerStateChanged;

        public ServerManager() {

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

                SetServerListeners(server);

                server.Address = servAddr;
            }
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


        private void SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e) {
            updateTimer.Interval = Settings.Default.UpdatePeriod;

            foreach(var server in servers) {
                server.Timeout = Settings.Default.Timeout;
            }
        }

        private void SetServerListeners(ServerStatus server) {
            server.StateChanged += Server_StateChanged;
        }

        internal void AddServer(int index) {
            var server = new ServerStatus();
            server.Timeout = Settings.Default.Timeout;
            SetServerListeners(server);
            servers.Insert(index, server);
        }

        internal void RemoveServer(int index) {
            servers.RemoveAt(index);
            saveServerList();
        }

        internal ServerStatus this[int index] {
            get => servers[index];
        }

        internal int IndexOf(ServerStatus server) {
            return servers.IndexOf(server);
        }

        internal void SetServerAddress(int index, string address) {
            var server = servers[index];
            server.Address = address;

            saveServerList();
        }

        public IEnumerator<ServerStatus> GetEnumerator() => servers.GetEnumerator();

        private void Server_StateChanged(ServerStatus server) {
            CheckDNSSuccess(server);
            UpdateStatusCounters(server);

            ServerStateChanged?.Invoke(server);
        }

        private void UpdateStatusCounters(ServerStatus server) {
            if(server.State == ServerStatus.QueryState.ANSWER_RECEIVED || server.State == ServerStatus.QueryState.TIME_OUT) {
                pendingUpdateCount--;
            }
            if(server.State == ServerStatus.QueryState.TIME_OUT) {
                timedOutCount++;
            }
        }

        private async void CheckDNSSuccess(ServerStatus server) {
            if(server.State == ServerStatus.QueryState.HOSTNAME_RESOLVED) {
                await updateServer(server);
            }
        }

        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            UpdateServerStatuses();
        }

        public async void UpdateServerStatuses() {
            List<Task> tasks = new List<Task>();
            foreach(var server in servers) {
                if(!server.IsReadyForUpdate) continue;
                tasks.Add(updateServer(server));
            }
            await Task.WhenAll(tasks);
        }

        private Task updateServer(ServerStatus server) {
            if(server.State == ServerStatus.QueryState.TIME_OUT) {
                timedOutCount--;
            }

            pendingUpdateCount++;
            return server.Update();
        }
    }
}
