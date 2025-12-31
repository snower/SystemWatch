using System;
using System.Windows;
using Microsoft.Win32;
using SystemWatch.Datas;
using SystemWatch.Views;

namespace SystemWatch;

public partial class App : Application
{
    private Performance? _performance = null;
    private Statistics? _statistics = null;
    private NotifyMenu _notifyMenu;
    
    public Performance?  Performance => _performance;
    public Statistics? Statistics => _statistics;
    
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        _performance = new Performance();
        _statistics = new Statistics();
        _statistics.Init();
        
        MainWindow = new WidgetWindow();
        _notifyMenu = new NotifyMenu();
        _notifyMenu.Show();
        
        _statistics.Start();
        _performance.Start();
        MainWindow.Show();

        SystemEvents.PowerModeChanged += PowerModeChanged;
    }
    
    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _performance?.Close();
        _statistics?.Close();
        
        if (MainWindow is WidgetWindow widgetWindow)
        {
            widgetWindow?.CloseWidgets();
        }
        _notifyMenu?.Close();
        _notifyMenu = null;
    }
    
    private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (MainWindow is WidgetWindow widgetWindow)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    _performance?.UpdateNetworkAvailability();
                    _performance?.Start();
                    _statistics?.Start();
                    widgetWindow?.Resume();
                    break;
                case PowerModes.Suspend:
                    widgetWindow?.Suspend();
                    _performance?.Stop();
                    _statistics?.Stop();
                    widgetWindow?.Suspend();
                    break;
            }
        }
    }
}