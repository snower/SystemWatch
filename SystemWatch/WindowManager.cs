using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemWatch
{
    class WindowManager
    {
        private MainWindow mainWindow;
        private List<Window> windows;

        public WindowManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.windows = new List<Window>();
        }

        public void Init()
        {
            CpuMemoryLoader cpuMemoryLoader= new CpuMemoryLoader();
            this.windows.Add(cpuMemoryLoader);

            LogicalDiakLoader logicalDiakLoader = new LogicalDiakLoader();
            this.windows.Add(logicalDiakLoader);

            NetworkInterfaceLoader networkInterfaceLoader = new NetworkInterfaceLoader();
            this.windows.Add(networkInterfaceLoader);
        }

        public void ShowAllWindow()
        {
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].Show();
            }
        }

        public void HideAllWindow()
        {
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].Hide();
            }
        }

        public void CloseAllWindow()
        {
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].Close();
            }
        }
    }
}
