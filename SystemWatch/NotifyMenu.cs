using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using SystemWatch.Views;

namespace SystemWatch
{
    class NotifyMenu
    {
        private readonly TaskbarIcon _icon;
        private ContextMenu _menu;
        private readonly ConcurrentDictionary<string, Window> _windows;

        public NotifyMenu()
        {
            this._icon = new TaskbarIcon();
            this._icon.ToolTipText = "SystemWatch";
            this._icon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "Assets\\Ico.ico");
            
            this._menu = new ContextMenu();

            this.InitMenus();
            this._icon.ContextMenu = this._menu;
            this._windows = new ConcurrentDictionary<string, Window>();
        }

        public void Show()
        {
            this._icon.Visibility = Visibility.Visible;
        }

        public void Close()
        {
            foreach (Window window in this._windows.Values.ToArray<Window>())
            {
                window.Close();
            }
            this._icon.Dispose();
        }

        public void UpdateIconText(string text)
        {
            this._icon.ToolTipText = text;
        }

        private void InitMenus()
        {
            MenuItem statisticsMenu = new MenuItem
            {
                Header = "统计"
            };
            statisticsMenu.Click += this.StatisticsMenuMenuClick;
            this._menu.Items.Add(statisticsMenu);

            MenuItem configMenu = new MenuItem
            {
                Header = "设置"
            };
            configMenu.Click += this.ConfigMenuClick;
            this._menu.Items.Add(configMenu);

            MenuItem exitMenu = new MenuItem
            {
                Header = "退出"
            };
            exitMenu.Click += this.ExitMenuClick;
            this._menu.Items.Add(exitMenu);
        }

        private void ConfigMenuClick(object sender, RoutedEventArgs e)
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

        private void StatisticsMenuMenuClick(object sender, RoutedEventArgs e)
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

        private void ExitMenuClick(object sender, RoutedEventArgs e)
        {
            WidgetWindow widgetWindow = Application.Current.MainWindow as WidgetWindow;
            widgetWindow?.Close();
            string[] windowNames = this._windows.Keys.ToArray();
            foreach (var windowName in windowNames)
            {
                if (this._windows.TryRemove(windowName, out var window))
                {
                    window?.Close();
                }
            }
            Application.Current.Shutdown();
        }

        private void ConfigWindowClosedEvent(object? sender, EventArgs e)
        {
            this._windows.TryRemove("config", out var _);
        }

        private void StatisticsWindowClosedEvent(object? sender, EventArgs e)
        {
            this._windows.TryRemove("statistics", out var _);
        }
    }
}
