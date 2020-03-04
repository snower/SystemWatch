using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        private Timer timer;
        private WidgetWindow widgetWindow;
        private Graphics widgetGraphics;
        private List<Widget> widgets;
        private IntPtr descktopPtr;
        private IntPtr shellViewPtr;

        public int WidgetWidth { private set; get; }
        public int WidgetHeight { private set; get; }

        public WidgetManager()
        {
            this.widgets = new List<Widget>();
        }

        public void Init()
        {
            this.WidgetWidth = 146;
            this.WidgetHeight = 120;

            this.widgetWindow = new WidgetWindow();
            this.widgetWindow.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 200, 100);
            this.widgetWindow.ClientSize = new System.Drawing.Size(this.WidgetWidth + 10, this.WidgetHeight * 3 + 110);

            CpuMemoryWidget cpuMemoryLoader= new CpuMemoryWidget(new Point(5, 5), new Size(this.WidgetWidth, this.WidgetHeight));
            this.widgets.Add(cpuMemoryLoader);

            LogicalDiakWidget logicalDiakLoader = new LogicalDiakWidget(new Point(5, this.WidgetHeight + 55), new Size(this.WidgetWidth, this.WidgetHeight));
            this.widgets.Add(logicalDiakLoader);

            NetworkInterfaceWidget networkInterfaceLoader = new NetworkInterfaceWidget(new Point(6, this.WidgetHeight * 2 + 105), new Size(this.WidgetWidth, this.WidgetHeight));
            this.widgets.Add(networkInterfaceLoader);

            this.FindDescktopWindow();
            if(this.shellViewPtr.ToInt64() != 0)
            {
                SetWindowLong(this.widgetWindow.Handle, -8, (ulong)this.shellViewPtr.ToInt64());
            }

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Enabled = false;
            this.timer.Tick += new EventHandler(TimerEvent);
        }

        private void FindDescktopWindow()
        {
            this.descktopPtr = GetDesktopWindow();
            this.shellViewPtr = new IntPtr(0);

            IntPtr progmanPtr = FindWindowEx(this.descktopPtr, new IntPtr(0), "Progman", null);
            do
            {
                this.shellViewPtr = FindWindowEx(progmanPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if (this.shellViewPtr.ToInt64() != 0)
                {
                    return;
                }

                progmanPtr = FindWindowEx(this.descktopPtr, progmanPtr, "WorkerW", null);
                if (progmanPtr.ToInt64() == 0)
                {
                    break;
                }

            } while (true);

            IntPtr workerWPtr = FindWindowEx(this.descktopPtr, new IntPtr(0), "WorkerW", null);
            do
            {
                this.shellViewPtr = FindWindowEx(workerWPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if (this.shellViewPtr.ToInt64() != 0)
                {
                    return;
                }

                workerWPtr = FindWindowEx(this.descktopPtr, workerWPtr, "WorkerW", null);
                if (workerWPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);
        }

        public void Show()
        {
            for (int i = 0, count = this.widgets.Count; i < count; i++)
            {
                this.widgets[i].BackgroundUpdate();
            }
            this.widgetWindow.Show();
            this.widgetGraphics = this.widgetWindow.CreateGraphics();
            this.timer.Start();
        }

        private void TimerEvent(object o, EventArgs e)
        {
            for (int i = 0, count = this.widgets.Count; i < count; i++)
            {
                try
                {
                    this.widgets[i].Update();
                } catch(Exception execption)
                {
                    Console.WriteLine(execption.ToString());
                }
                
            }

            for (int i = 0, count = this.widgets.Count; i < count; i++)
            {
                this.widgets[i].Show(this.widgetGraphics);
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
            this.timer.Dispose();
            for (int i = 0, count = this.widgets.Count; i < count; i++)
            {
                this.widgets[i].Close();
            }
        }

        public string GetShortNotice()
        {
            string notice = "";
            foreach(Widget widget in this.widgets)
            {
                notice += widget.GetShortNoticce() + " ";
            }
            return notice.Substring(0, notice.Length - 1);
        }
    }
}
