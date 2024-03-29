﻿using Henke37.Valve.Source.Predownloader;
using Henke37.Valve.Source.ServerQuery;
using QueryTest.Properties;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryTest {
    public partial class PredownloadForm : Form {

        private ServerStatus st;

		private MapPreDownloader dn;
		private bool downloading;

		public PredownloadForm() {
            InitializeComponent();
            st = new ServerStatus();
            st.Address = addressBox.Text;
			st.Timeout = 3000;

			st.StateChanged += St_StateChanged;

			dn = new MapPreDownloader(st, downloadRoot_txt.Text);
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
				rules_btn.Enabled = false;
				updateDownloadButton();
				return;
			}

			if(st.State != ServerStatus.QueryState.AnswerReceived) return;

			updateDownloadButton();
			if(st.Info == null) return;
			mapTxt.Text = $"{st.Info.Map} {st.Info.PlayerCount}/{st.Info.MaxPlayerCount}";


			rules_btn.Enabled = true;
			nextMapTxt.Text = st.Rules.NextMap;
		}

		private async void updateBtn_ClickAsync(object sender, EventArgs e) {
            await st.Update();
        }

		private async void downloadButton_Click(object sender, EventArgs e) {
			await downloadMap();
		}

		private async Task downloadMap() {
			downloading = true;
			downloadButton.Enabled = false;
			try {
				await dn.ReadyServerAsync();
				httpErrCode.Visible = false;
			} catch(HttpRequestException err) {
				httpErrCode.Visible = true;
			} finally {
				downloading = false;
				updateDownloadButton();
			}
		}

		private void updateDownloadButton() {
			if(downloadButton.InvokeRequired) {
				downloadButton.Invoke(new Action(updateDownloadButton));
				return;
			}
			downloadButton.Text = dn.MapsAreReady? "Downloaded." : "Download!";
			downloadButton.Enabled = CanStartDownload;
		}

		private void addressBox_TextChanged(object sender, EventArgs e) {
			st.Address = addressBox.Text;
		}

		private void TestForm_FormClosed(object sender, FormClosedEventArgs e) {
			Settings.Default.Save();
		}

		private async void UpdateTimer_Tick(object sender, EventArgs e) {
			await st.Update();
			if(CanStartDownload && AutoDownload_cb.Checked) {
				await downloadMap();
			}
		}

		private bool CanStartDownload {
			get {
				return !downloading && st.Info != null && st.Rules != null && !string.IsNullOrEmpty(downloadRoot_txt.Text) && !dn.MapsAreReady;
			}
		}

		private void AutoUpdate_Cb_CheckedChanged(object sender, EventArgs e) {
			UpdateTimer.Enabled = autoUpdate_Cb.Checked;
			AutoDownload_cb.Enabled = autoUpdate_Cb.Checked;
		}

		private void DownloadRoot_txt_TextChanged(object sender, EventArgs e) {
			dn.FastDLRoot = downloadRoot_txt.Text;
			updateDownloadButton();
		}

		private void rules_btn_Click(object sender, EventArgs e) {
			new RulesForm(st.Rules.Rules).Show();
		}
	}
}
