﻿using Henke37.Valve.Source.Predownloader;
using Henke37.Valve.Source.ServerQuery;
using QueryTest.Properties;
using System;
using System.Windows.Forms;

namespace QueryTest {
    public partial class TestForm : Form {

        private ServerStatus st;

		private MapPreDownloader dn;

        public TestForm() {
            InitializeComponent();
            st = new ServerStatus();
            st.Address = addressBox.Text;
			st.Timeout = 3000;

			st.StateChanged += St_StateChanged;

			dn = new MapPreDownloader(st, "http://redirect.tf2maps.net/");
		}

		private void St_StateChanged(ServerStatus obj) {
			if(mapTxt.InvokeRequired) {
				mapTxt.Invoke((MethodInvoker)delegate {
					St_StateChanged(obj);
				});
				return;
			}

			if(st.State == ServerStatus.QueryState.TimeOut) {
				mapTxt.Text = "TIME_OUT";
				return;
			}
			if(st.Info == null) return;
			mapTxt.Text = $"{st.Info.Map} {st.Info.PlayerCount}/{st.Info.MaxPlayerCount}";

			string nextMap = st.Rules["nextlevel"];
			if(st.Rules.TryGetCVar("sm_nextmap", out string smNextMap)) {
				nextMap = smNextMap;
			}
			nextMapTxt.Text = nextMap;
		}

		private async void updateBtn_ClickAsync(object sender, EventArgs e) {
            await st.Update();
           
        }

		private async void downloadButton_Click(object sender, EventArgs e) {
			downloadButton.Enabled = false;
			await dn.ReadyServerAsync();
			downloadButton.Enabled = true;
		}

		private void addressBox_TextChanged(object sender, EventArgs e) {
			st.Address = addressBox.Text;
		}

		private void TestForm_FormClosed(object sender, FormClosedEventArgs e) {
			Settings.Default.Save();
		}
	}
}
