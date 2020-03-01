using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using SystemWatch.ui;

namespace SystemWatch
{
    class WindowManager
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, ulong dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private MainWindow mainWindow;
        private LoadWindow loadWindow;
        private List<Window> windows;
        private System.Timers.Timer timer;
        private IntPtr descktopPtr;
        private IntPtr shellViewPtr;

        public WindowManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.windows = new List<Window>();
        }

        public void Init()
        {
            this.loadWindow = new LoadWindow();

            CpuMemoryLoader cpuMemoryLoader= new CpuMemoryLoader(new Point(6, 1), new Size(146, 135));
            this.windows.Add(cpuMemoryLoader);

            LogicalDiakLoader logicalDiakLoader = new LogicalDiakLoader(new Point(6, 146), new Size(146, 135));
            this.windows.Add(logicalDiakLoader);

            NetworkInterfaceLoader networkInterfaceLoader = new NetworkInterfaceLoader(new Point(6, 291), new Size(146, 135));
            this.windows.Add(networkInterfaceLoader);


            this.descktopPtr = GetDesktopWindow();
            this.shellViewPtr = new IntPtr(0);
            IntPtr workerWPtr = FindWindowEx(this.descktopPtr, new IntPtr(0), "WorkerW", null);
            do
            {
                this.shellViewPtr = FindWindowEx(workerWPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if(this.shellViewPtr.ToInt64() != 0)
                {
                    break;
                }

                workerWPtr = FindWindowEx(this.descktopPtr, workerWPtr, "WorkerW", null);
                if (workerWPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);
            SetWindowLong(this.loadWindow.Handle, -8, (ulong)this.shellViewPtr.ToInt64());
            //SetParent(this.loadWindow.Handle, this.shellViewPtr);


            this.timer = new System.Timers.Timer(1000);
            this.timer.Interval = 1000;
            this.timer.Enabled = false;
            this.timer.Elapsed += new ElapsedEventHandler(this.TimerEvent);
        }

        public void Show()
        {
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].BackgroundUpdate();
            }
            this.timer.Start();
            this.loadWindow.Show();
        }

        private void TimerEvent(object o, ElapsedEventArgs e)
        {
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                try
                {
                    this.windows[i].Update();
                } catch(Exception execption)
                {
                    Console.WriteLine(execption.ToString());
                }
                
            }

            using (Graphics g = this.loadWindow.CreateGraphics())
            {
                for (int i = 0, count = this.windows.Count; i < count; i++)
                {
                    this.windows[i].Show(g);
                }
            }
        }

        public void Close()
        {
            this.timer.Stop();
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].Close();
            }
        }
    }
}
