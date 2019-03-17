using Source_Stalker.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Source_Stalker {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext());
        }

        private class AppContext : ApplicationContext {
            ServerListForm serverForm;

            NotifyIcon notifyIcon;

            ServerManager manager;

            public AppContext() {
                manager = new ServerManager();

                notifyIcon = new NotifyIcon() {
                    ContextMenuStrip = makeContextMenu(),
                    Text = "Source Server Stalker",
                    Icon = Resources.NotificationIcon,
                    Visible = true
                };
                notifyIcon.MouseClick += NotifyIcon_MouseClick;
            }

            private ContextMenuStrip makeContextMenu() {
                ContextMenuStrip menu = new ContextMenuStrip();

                ToolStripItem exitItem=menu.Items.Add("Exit");
                exitItem.Click += ExitItem_Click;

				ToolStripItem updateItem = menu.Items.Add("Update");
				updateItem.Click += UpdateItem_Click;

				return menu;
            }

			private void UpdateItem_Click(object sender, EventArgs e) {
				manager.UpdateServerStatuses();
			}

			private void ExitItem_Click(object sender, EventArgs e) {
                ExitThread();
            }

            private void NotifyIcon_MouseClick(object sender, MouseEventArgs e) {
                if(e.Button != MouseButtons.Left) return;
                ShowServerForm();
            }

            private void ShowServerForm() {
                if(serverForm == null) {
                    serverForm = new ServerListForm(manager);
                    serverForm.FormClosed += ServerForm_FormClosed;
                    serverForm.Show();
                } else {
                    serverForm.Activate();
                }
            }

            private void ServerForm_FormClosed(object sender, FormClosedEventArgs e) {
                serverForm = null;
            }

            protected override void ExitThreadCore() {
                if(serverForm != null) { serverForm.Close(); }
                notifyIcon.Visible = false; // should remove lingering tray icon!
                base.ExitThreadCore();
            }
        }
    }
}
