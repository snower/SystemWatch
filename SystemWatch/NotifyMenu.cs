using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using SystemWatch.ui;

namespace SystemWatch
{
    class NotifyMenu
    {
        private readonly TrayIcon _icon;
        private NativeMenu _menus;
        private readonly Dictionary<string, Window> _windows;

        public NotifyMenu()
        {
            this._icon = new TrayIcon();
            this._icon.ToolTipText = "SystemWatch";
            this._icon.Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://SystemWatch/Assets/Ico.ico")));
            // this.icon. += new MouseEventHandler(this.IconMenuMouseMove);

            this.InitMenus();
            this._icon.Menu = this._menus;
            this._windows = new Dictionary<string, Window>();
        }

        public void Show()
        {
            this._icon.IsVisible = true;
        }

        public void Close()
        {
            foreach (Window window in this._windows.Values.ToArray<Window>())
            {
                window.Close();
            }
        }

        public void UpdateIconText(string text)
        {
            this._icon.ToolTipText = text;
        }

        private void InitMenus()
        {
            this._menus = new NativeMenu();

            NativeMenuItem statisticsMenu = new NativeMenuItem
            {
                Header = "统计"
            };
            statisticsMenu.Click += this.StatisticsMenuMenuClick;
            this._menus.Add(statisticsMenu);

            NativeMenuItem configMenu = new NativeMenuItem
            {
                Header = "设置"
            };
            configMenu.Click += this.ConfigMenuClick;
            this._menus.Add(configMenu);

            NativeMenuItem exitMenu = new NativeMenuItem
            {
                Header = "退出"
            };
            exitMenu.Click += this.ExitMenuClick;
            this._menus.Add(exitMenu);
        }

        private void ConfigMenuClick(object? sender, EventArgs e)
        {
            if(this._windows.ContainsKey("config"))
            {
                this._windows["config"].Show();
                return;
            }

            Window configWindow = new ConfigWindow();
            configWindow.Closed += this.ConfigWindowClosedEvent;
            this._windows["config"] = configWindow;
            configWindow.Show();
        }

        private void StatisticsMenuMenuClick(object? sender, EventArgs e)
        {
            if (this._windows.ContainsKey("statistics"))
            {
                this._windows["statistics"].Show();
                return;
            }

            Window statisticsWindow = new StatisticsWindow();
            statisticsWindow.Closed += this.StatisticsWindowClosedEvent;
            this._windows["statistics"] = statisticsWindow;
            statisticsWindow.Show();
        }

        private void ExitMenuClick(object? sender, EventArgs e)
        {
            WidgetWindow widgetWindow = (WidgetWindow) ((IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime).MainWindow;
            widgetWindow?.Close();
        }

        private void ConfigWindowClosedEvent(object? sender, EventArgs e)
        {
            if (this._windows.ContainsKey("config"))
            {
                this._windows.Remove("config");
            }
        }

        private void StatisticsWindowClosedEvent(object? sender, EventArgs e)
        {
            if (this._windows.ContainsKey("statistics"))
            {
                this._windows.Remove("statistics");
            }
        }
    }
}
