using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SystemWatch.Properties;

namespace SystemWatch
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            this.notifyIcon.Icon = Resources.ico;
            this.ShowInTaskbar = false;
            this.notifyIcon.Visible = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.ShowInTaskbar = true;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                e.Cancel = true;
            }
        }

        private void notifyMenuExit_Click(object sender, EventArgs e)
        {
            Program.GetInformation().Close();
            Program.GetWigetManager().Close();
            this.Close();
            Application.Exit();
        }

        public void AddWindowName(MenuItem[] item)
        {
            this.notifyMenuWindow.MenuItems.AddRange(item);
        }
    }
}
