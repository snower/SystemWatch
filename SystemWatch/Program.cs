using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace SystemWatch
{
    static class Program
    {
        private static Performance information;
        private static WidgetManager widgetManager;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            information = new Performance();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow();
            widgetManager = new WidgetManager(mainWindow);

            widgetManager.Init();
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(PowerModeChanged);

            information.Start();
            widgetManager.Show();

            Application.Run(mainWindow);
        }

        public static Performance GetInformation()
        {
            return information;
        }

        public static WidgetManager GetWigetManager()
        {
            return widgetManager;
        }
        private static void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    information.Start();
                    widgetManager.Resume();
                    break;
                case PowerModes.Suspend:
                    information.Stop();
                    widgetManager.Suspend();
                    break;
            }
        }
    }
}
