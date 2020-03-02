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
    class WidgetManager
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, ulong dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private System.Timers.Timer timer;
        private MainWindow mainWindow;
        private WidgetWindow widgetWindow;
        private List<Widget> windows;
        private IntPtr descktopPtr;
        private IntPtr shellViewPtr;

        public WidgetManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.windows = new List<Widget>();
        }

        public void Init()
        {
            this.widgetWindow = new WidgetWindow();

            CpuMemoryWidget cpuMemoryLoader= new CpuMemoryWidget(new Point(6, 1), new Size(146, 135));
            this.windows.Add(cpuMemoryLoader);

            LogicalDiakWidget logicalDiakLoader = new LogicalDiakWidget(new Point(6, 146), new Size(146, 135));
            this.windows.Add(logicalDiakLoader);

            NetworkInterfaceWidget networkInterfaceLoader = new NetworkInterfaceWidget(new Point(6, 291), new Size(146, 135));
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
            SetWindowLong(this.widgetWindow.Handle, -8, (ulong)this.shellViewPtr.ToInt64());

            timer = new System.Timers.Timer(1000);
            timer.Enabled = false;
            timer.Elapsed += new ElapsedEventHandler(TimerEvent);
        }

        public void Show()
        {
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].BackgroundUpdate();
            }
            this.widgetWindow.Show();
            this.timer.Start();
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

            Graphics g = this.widgetWindow.CreateGraphics();
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].Show(g);
            }
        }

        public void Resume()
        {
            this.timer.Start();
        }

        public void Suspend()
        {
            this.timer.Stop();
        }

        public void Close()
        {
            this.timer.Stop();
            this.timer.Close();
            for (int i = 0, count = this.windows.Count; i < count; i++)
            {
                this.windows[i].Close();
            }
        }
    }
}
