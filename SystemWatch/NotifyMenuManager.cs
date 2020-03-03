using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SystemWatch.Properties;
using SystemWatch.ui;

namespace SystemWatch
{
    class NotifyMenuManager
    {
        private NotifyIcon icon;
        private ContextMenu menus;
        private Dictionary<string, Form> windows;

        public NotifyMenuManager()
        {
            this.icon = new NotifyIcon();
            this.icon.Text = "SystemWatch";
            this.icon.Icon = Resources.ico;
            this.icon.MouseMove += new MouseEventHandler(this.IconMenuMouseMove);

            this.InitMenus();
            this.icon.ContextMenu = this.menus;
            this.windows = new Dictionary<string, Form>();
        }

        public void Show()
        {
            this.icon.Visible = true;
        }

        public void Close()
        {
            foreach (KeyValuePair<string, Form> item in this.windows)
            {
                item.Value.Close();
            }
        }

        public void UpdateIconText(string text)
        {
            this.icon.Text = text;
        }

        private void InitMenus()
        {
            this.menus = new ContextMenu();

            MenuItem configMenu = new MenuItem();
            configMenu.Index = 0;
            configMenu.Text = "&Config";
            configMenu.Click += new System.EventHandler(this.ConfigMenuClick);

            MenuItem exitMenu = new MenuItem();
            exitMenu.Index = 1;
            exitMenu.Text = "&Exit";
            exitMenu.Click += new System.EventHandler(this.ExitMenuClick);

            this.menus.MenuItems.AddRange(new MenuItem[] {configMenu, exitMenu});
        }

        private void IconMenuMouseMove(object sender, MouseEventArgs e)
        {
            this.icon.Text = Program.GetWigetManager().GetShortNotice();
        }

        private void ConfigMenuClick(object sender, EventArgs e)
        {
            if(this.windows.ContainsKey("config"))
            {
                this.windows["config"].Show();
                return;
            }

            Form configWindow = new ConfigWindow();
            configWindow.FormClosed += new FormClosedEventHandler(this.ConfigWindowClosedEvent);
            this.windows["config"] = configWindow;
            configWindow.Show();
        }

        private void ExitMenuClick(object sender, EventArgs e)
        {
            this.Close();
            Program.GetInformation().Close();
            Program.GetWigetManager().Close();
            Application.Exit();
        }

        private void ConfigWindowClosedEvent(object sender, FormClosedEventArgs e)
        {
            if (this.windows.ContainsKey("config"))
            {
                this.windows.Remove("config");
            }
        }
    }
}
