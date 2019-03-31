using Source_Stalker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			if(st.State == ServerStatus.QueryState.TIME_OUT) {
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
	}
}
