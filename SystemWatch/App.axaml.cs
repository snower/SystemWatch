using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _performance = new Performance();
            _statistics = new Statistics();
            _statistics.Init();
            
            desktop.MainWindow = new WidgetWindow();
            _notifyMenu = new NotifyMenu();
            
            _statistics.Start();
            _performance.Start();

            SystemEvents.PowerModeChanged += PowerModeChanged;
            desktop.Exit += ApplicationExitEvent;
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void ApplicationExitEvent(object? sender, EventArgs e)
    {
        _performance?.Close();
        _statistics?.Close();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            WidgetWindow? widgetWindow = (WidgetWindow)((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow!;
            widgetWindow?.CloseWidgets();
        }
    }
    
    private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            WidgetWindow? widgetWindow = (WidgetWindow)((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow!;
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