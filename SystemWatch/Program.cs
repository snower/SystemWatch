using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SystemWatch
{
    static class Program
    {
        private static Information information;
        private static WindowManager windowManager;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            information = new Information();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow();
            windowManager = new WindowManager(mainWindow);

            windowManager.Init();
            information.Start();
            windowManager.ShowAllWindow();
            Application.Run(mainWindow);
        }

        public static Information GetInformation()
        {
            return information;        
        }

        public static WindowManager GetWindowManager()
        {
            return windowManager;
        }
    }
}
