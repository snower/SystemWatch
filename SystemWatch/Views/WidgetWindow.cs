using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using SystemWatch.Widgets;

namespace SystemWatch.Views
{
    public partial class WidgetWindow : Window
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOSIZE = 0x0001;
        
        private readonly List<Widget> _widgets = new List<Widget>();
        
        private readonly DispatcherTimer _timer;
        private IntPtr _shellViewPtr = IntPtr.Zero;
        private DrawingVisual _drawingVisual;
        
        public int WidgetWidth { get; }
        public int WidgetHeight { get; }

        public TimeSpan RefreshInterval
        {
            get
            {
                return _timer.Interval;
            }
            set
            {
                _timer.Interval = value;
            }
        }

        public WidgetWindow()
        {
            InitializeComponent();
            
            this.WidgetWidth = 146;
            this.WidgetHeight = 120;
            
            UpdateWindowPosition();
            this.Width = this.WidgetWidth + 10;
            this.Height = this.WidgetHeight * 3 + 110;
            this.Left = SystemParameters.PrimaryScreenWidth - 250;
            this.Top = 120;
            

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

            this.Activated += OnActivated;
            
            InitializeDrawing();
            
            this.Loaded += OnLoaded;
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _drawingVisual;

        private void InitializeDrawing()
        {
            _drawingVisual = new DrawingVisual();
            AddVisualChild(_drawingVisual);
            AddLogicalChild(_drawingVisual);
        }

        private void DoRender()
        {
            using (var drawingContext = _drawingVisual.RenderOpen())
            {
                foreach (var widget in _widgets)
                {
                    widget.Render(drawingContext);
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            DoRender();
        }

        public new void Show()
        {
            base.Show();
            UpdateDesktopWindow();
            this._timer.Start();
        }

        public new void Hide()
        {
            base.Hide();
            UpdateDesktopWindow();
            this._timer.Stop();
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
            DoRender();
        }
        
        private async void OnScreenChanged(object? sender, EventArgs e)
        {
            await Task.Delay(2000);
            UpdateWindowPosition();
        }

        private void OnLoaded(object? sender, EventArgs e)
        {
            SystemEvents.DisplaySettingsChanged += OnScreenChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            SystemEvents.DisplaySettingsChanged -= OnScreenChanged;
            base.OnClosed(e);
        }
        
        private void OnActivated(object? sender, EventArgs e)
        {
            UpdateWindowPosition();
            UpdateDesktopWindow();
            this._timer.Start();
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

        private void UpdateWindowPosition()
        {
            if (this._shellViewPtr != IntPtr.Zero)
            {
                var windowInteropHelper = new WindowInteropHelper(this);
                IntPtr hWnd = windowInteropHelper.Handle;
                if (hWnd != IntPtr.Zero)
                {
                    // 获取物理屏幕尺寸
                    IntPtr monitor = MonitorFromWindow(this._shellViewPtr, MONITOR_DEFAULTTONEAREST);
                    if (monitor != IntPtr.Zero)
                    {
                        MONITORINFO monitorInfo = new MONITORINFO();
                        monitorInfo.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
                        if (GetMonitorInfo(monitor, ref monitorInfo))
                        {
                            int screenWidth = monitorInfo.rcMonitor.Right - monitorInfo.rcMonitor.Left;
                            double screenRatio = ((double)screenWidth) / SystemParameters.PrimaryScreenWidth;
                            // 计算相对父窗口的坐标
                            int targetX = (int) (screenWidth - 250 * screenRatio);
                            int targetY = (int) (120 * screenRatio);

                            SetWindowPos(hWnd, IntPtr.Zero, targetX, targetY, 0, 0,
                                SWP_NOZORDER | SWP_NOACTIVATE | SWP_NOSIZE);
                            return;
                        }
                    }
                }
            }
            this.Left = SystemParameters.PrimaryScreenWidth - 250;
            this.Top = 120;
        }

        private void UpdateDesktopWindow()
        {
            IntPtr shellViewPtr = this.FindDescktopWindow();
            if (shellViewPtr != IntPtr.Zero && this._shellViewPtr != shellViewPtr)
            {
               var windowInteropHelper = new WindowInteropHelper(this);
               SetParent(windowInteropHelper.Handle, shellViewPtr);
               this._shellViewPtr = shellViewPtr;
            }
        }
        
        private IntPtr FindDescktopWindow()
        {
            IntPtr descktopPtr = GetDesktopWindow();
            IntPtr progmanPtr = FindWindowEx(descktopPtr, IntPtr.Zero, "Progman", null);
            do
            {
                IntPtr shellViewPtr = FindWindowEx(progmanPtr, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellViewPtr.ToInt64() != 0)
                {
                    return shellViewPtr;
                }
                progmanPtr = FindWindowEx(descktopPtr, progmanPtr, "WorkerW", null);
                if (progmanPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);

            IntPtr workerWPtr = FindWindowEx(descktopPtr, IntPtr.Zero, "WorkerW", null);
            do
            {
                IntPtr shellViewPtr = FindWindowEx(workerWPtr, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellViewPtr.ToInt64() != 0)
                {
                    return shellViewPtr;
                }
                workerWPtr = FindWindowEx(descktopPtr, workerWPtr, "WorkerW", null);
                if (workerWPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);
            return IntPtr.Zero;
        }
    }
}
