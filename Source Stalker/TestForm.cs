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
    public partial class TestForm : Form {

        private ServerStatus st;

		private MapPreDownloader dn;

        public TestForm() {
            InitializeComponent();
            st = new ServerStatus();
            st.Address = "31.186.251.51:27015";
        }

        private async void updateBtn_ClickAsync(object sender, EventArgs e) {
            await st.Update();
            if(st.State==ServerStatus.StateEnum.TIME_OUT) {
                mapTxt.Text = "TIME_OUT";
                return;
            }
            mapTxt.Text = st.info.Map;
        }

		private void downloadButton_Click(object sender, EventArgs e) {
			dn = new MapPreDownloader(st,"");
			dn.ReadyServerAsync();
		}
	}
}
