using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;

namespace SystemWatch.ui
{
    public class WidgetWindow : Window
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        
        private readonly List<Widget> _widgets = new List<Widget>();
        
        private readonly DispatcherTimer _timer;
        
        private IntPtr _descktopPtr;
        private IntPtr _shellViewPtr;
        
        public int WidgetWidth { get; }
        public int WidgetHeight { get; }
        
        public WidgetWindow()
        {
            this.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            this.ExtendClientAreaToDecorationsHint = false;
            this.SystemDecorations = SystemDecorations.None;
            this.ExtendClientAreaTitleBarHeightHint = -1;
            this.TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
            this.ShowInTaskbar = false;
            this.Background = Brushes.Transparent;

            this.WidgetWidth = 146;
            this.WidgetHeight = 120;
            this.Position = new PixelPoint(Screens.Primary.WorkingArea.Width - 250, 120);
            this.ClientSize = new Size(this.WidgetWidth + 10, this.WidgetHeight * 3 + 110);
            

            CpuMemoryWidget cpuMemoryLoader= new CpuMemoryWidget(new Point(5, 5), new Size(this.WidgetWidth, this.WidgetHeight));
            this._widgets.Add(cpuMemoryLoader);

            LogicalDiakWidget logicalDiakLoader = new LogicalDiakWidget(new Point(5, this.WidgetHeight + 55), new Size(this.WidgetWidth, this.WidgetHeight));
            this._widgets.Add(logicalDiakLoader);

            NetworkInterfaceWidget networkInterfaceLoader = new NetworkInterfaceWidget(new Point(6, this.WidgetHeight * 2 + 105), new Size(this.WidgetWidth, this.WidgetHeight));
            this._widgets.Add(networkInterfaceLoader);

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromSeconds(1);
            this._timer.IsEnabled = false;
            this._timer.Tick += TimerEvent;
        }

        public override void Show()
        {
            base.Show();
            if (this._descktopPtr == IntPtr.Zero)
            {
                this.FindDescktopWindow();
                if (this._shellViewPtr != IntPtr.Zero)
                {
                    SetParent(TryGetPlatformHandle().Handle, this._shellViewPtr);
                }
            }
            this._timer.Start();
        }

        public override void Hide()
        {
            base.Hide();
            this._timer.Stop();
        }

        public override void Render(DrawingContext context)
        {
            foreach (var widget in _widgets)
            {
                widget.Render(context);
            }
        }

        public void CloseWidgets()
        {
            foreach (var widget in _widgets)
            {
                widget.Close();
            }
        }
        
        private void TimerEvent(object? o, EventArgs e)
        {
            InvalidateVisual();
        }
        
        public void Resume()
        {
            this._timer.Start();
        }

        public void Suspend()
        {
            this._timer.Stop();
        }
        
        public string GetShortNotice()
        {
            string notice = "";
            foreach(Widget widget in this._widgets)
            {
                notice += widget.GetShortNoticce() + " ";
            }
            return notice.Substring(0, notice.Length - 1);
        }
        
        private void FindDescktopWindow()
        {
            this._descktopPtr = GetDesktopWindow();
            this._shellViewPtr = IntPtr.Zero;

            IntPtr progmanPtr = FindWindowEx(this._descktopPtr, new IntPtr(0), "Progman", null);
            do
            {
                this._shellViewPtr = FindWindowEx(progmanPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if (this._shellViewPtr.ToInt64() != 0)
                {
                    return;
                }

                progmanPtr = FindWindowEx(this._descktopPtr, progmanPtr, "WorkerW", null);
                if (progmanPtr.ToInt64() == 0)
                {
                    break;
                }

            } while (true);

            IntPtr workerWPtr = FindWindowEx(this._descktopPtr, new IntPtr(0), "WorkerW", null);
            do
            {
                this._shellViewPtr = FindWindowEx(workerWPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if (this._shellViewPtr.ToInt64() != 0)
                {
                    return;
                }

                workerWPtr = FindWindowEx(this._descktopPtr, workerWPtr, "WorkerW", null);
                if (workerWPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);
        }
    }
}
