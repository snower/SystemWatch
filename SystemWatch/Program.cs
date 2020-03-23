using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace SystemWatch
{
    static class Program
    {
        private static Performance performance;
        private static Statistics statistics;
        private static WidgetManager widgetManager;
        private static NotifyMenuManager notifyMenuManager;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            performance = new Performance();
            statistics = new Statistics();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            widgetManager = new WidgetManager();
            notifyMenuManager = new NotifyMenuManager();

            statistics.Init();
            widgetManager.Init();
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(PowerModeChanged);

            statistics.Start();
            performance.Start();
            widgetManager.Show();
            notifyMenuManager.Show();

            Application.ApplicationExit += ApplicationExitEvent;
            Application.Run();
        }

        private static void ApplicationExitEvent(object sender, EventArgs e)
        {
            performance.Close();
            widgetManager.Close();
            statistics.Close();
        }

        public static Performance GetPerformance()
        {
            return performance;
        }

        public static WidgetManager GetWigetManager()
        {
            return widgetManager;
        }

        public static Statistics GetStatistics()
        {
            return statistics;
        }

        private static void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    performance.UpdateNetworkAvailability();
                    performance.Start();
                    statistics.Start();
                    widgetManager.Resume();
                    break;
                case PowerModes.Suspend:
                    widgetManager.Suspend();
                    performance.Stop();
                    statistics.Stop();
                    break;
            }
        }
    }
}
