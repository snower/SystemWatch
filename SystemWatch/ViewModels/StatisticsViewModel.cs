using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace SystemWatch.ViewModels
{
    
    public enum TimePeriodType
    {
        Minutes,
        Hours,
        Days,
    }

    public class StatisticsViewModel : ReactiveObject
    {
        private TimePeriodType _timePeriodType = TimePeriodType.Days;
        private int _timePeriod = 30;
        private SolidColorPaint _textPaint = new SolidColorPaint()
        {
            FontFamily = "Microsoft YaHei",
            SKTypeface = SKFontManager.Default.MatchFamily("Microsoft YaHei"),
            Color = SKColors.Black,
        };

        private string _cpuTitle = "";
        private ObservableCollection<Axis> _cpuXAxes = [];
        private ObservableCollection<Axis> _cpuYAxes = [];
        private ObservableCollection<ISeries> _cpuSeries = [];
        private string _memTitle = "";
        private ObservableCollection<Axis> _memXAxes = [];
        private ObservableCollection<Axis> _memYAxes = [];
        private ObservableCollection<ISeries> _memSeries = [];
        private string _diskTitle = "";
        private ObservableCollection<Axis> _diskXAxes = [];
        private ObservableCollection<Axis> _diskYAxes = [];
        private ObservableCollection<ISeries> _diskSeries = [];
        private string _networkTitle = "";
        private ObservableCollection<Axis> _networkXAxes = [];
        private ObservableCollection<Axis> _networkYAxes = [];
        private ObservableCollection<ISeries> _networkSeries = [];
        
        public event PropertyChangedEventHandler PropertyChanged;

        public TimePeriodType TimePeriodType
        {
            get => _timePeriodType;
            set => this.RaiseAndSetIfChanged(ref _timePeriodType, value);
        }
        
        public int TimePeriod
        {
            get => _timePeriod;
            set => this.RaiseAndSetIfChanged(ref _timePeriod, value); 
        }
        
        public SolidColorPaint TextPaint
        {
            get => _textPaint;
            set => this.RaiseAndSetIfChanged(ref _textPaint, value);
        }

        public string CpuTitle
        {
            get => _cpuTitle;
            set => this.RaiseAndSetIfChanged(ref _cpuTitle, value);
        }
        
        public ObservableCollection<Axis> CpuXAxes
        {
            get => _cpuXAxes;
            set => this.RaiseAndSetIfChanged(ref _cpuXAxes, value);
        }
        
        public ObservableCollection<Axis> CpuYAxes
        {
            get => _cpuYAxes;
            set => this.RaiseAndSetIfChanged(ref _cpuYAxes, value);
        }
        
        public ObservableCollection<ISeries> CpuSeries
        {
            get => _cpuSeries;
            set => this.RaiseAndSetIfChanged(ref _cpuSeries, value);
        }
        
        public string MemTitle
        {
            get => _memTitle;
            set => this.RaiseAndSetIfChanged(ref _memTitle, value);
        }
        
        public ObservableCollection<Axis> MemXAxes
        {
            get => _memXAxes;
            set => this.RaiseAndSetIfChanged(ref _memXAxes, value);
        }
        
        public ObservableCollection<Axis> MemYAxes
        {
            get => _memYAxes;
            set => this.RaiseAndSetIfChanged(ref _memYAxes, value);
        }
        
        public ObservableCollection<ISeries> MemSeries
        {
            get => _memSeries;
            set => this.RaiseAndSetIfChanged(ref _memSeries, value);
        }
        
        public string DiskTitle
        {
            get => _diskTitle;
            set => this.RaiseAndSetIfChanged(ref _diskTitle, value);
        }
        
        public ObservableCollection<Axis> DiskXAxes
        {
            get => _diskXAxes;
            set => this.RaiseAndSetIfChanged(ref _diskXAxes, value);
        }
        
        public ObservableCollection<Axis> DiskYAxes
        {
            get => _diskYAxes;
            set => this.RaiseAndSetIfChanged(ref _diskYAxes, value);
        }
        
        public ObservableCollection<ISeries> DiskSeries
        {
            get => _diskSeries;
            set => this.RaiseAndSetIfChanged(ref _diskSeries, value);
        }
        
        public string NetworkTitle
        {
            get => _networkTitle;
            set => this.RaiseAndSetIfChanged(ref _networkTitle, value);
        }
        
        public ObservableCollection<Axis> NetworkXAxes
        {
            get => _networkXAxes;
            set => this.RaiseAndSetIfChanged(ref _networkXAxes, value);
        }
        
        public ObservableCollection<Axis> NetworkYAxes
        {
            get => _networkYAxes;
            set => this.RaiseAndSetIfChanged(ref _networkYAxes, value);
        }
        
        public ObservableCollection<ISeries> NetworkSeries
        {
            get => _networkSeries;
            set => this.RaiseAndSetIfChanged(ref _networkSeries, value);
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
        
        public void UpdateCpuData(string[] xcpuData, double[] ycpuData)
        {
            CpuTitle = "CPU使用率";
            CpuXAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    Labels = xcpuData,
                    LabelsRotation = 0,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                    SeparatorsAtCenter = false,
                    TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                    TicksAtCenter = true,
                    ForceStepToMin = false,
                    MinStep = 1
                }
            ];
            CpuYAxes =
            [
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    TextSize = 9,
                    ForceStepToMin = true,
                    MinStep = 20,
                    Labeler = value => Math.Round(value, 2) + "%",
                }
            ];
            CpuSeries =
            [
                new LineSeries<double>
                {
                    Name = "使用率",
                    Values = ycpuData,
                    Fill = null,
                    GeometrySize = 2,
                    YToolTipLabelFormatter = point => point.AsDataLabel + "%",
                }
            ];
        }

        public void UpdateMemData(string[] xmemData, double[] ymemData, double maxValue, string unitLabel)
        {
            MemTitle = "内存使用量";
            MemXAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    Labels = xmemData,
                    LabelsRotation = 0,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                    SeparatorsAtCenter = false,
                    TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                    TicksAtCenter = true,
                    ForceStepToMin = false,
                    MinStep = 1
                }
            ];
            MemYAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    MinLimit = 0,
                    MaxLimit = Math.Max(maxValue, 1),
                    ForceStepToMin = true,
                    MinStep = Math.Max(maxValue / 5, 0.2),
                    Labeler = value => Math.Round(value, 2) + unitLabel,
                }
            ];
            MemSeries =
            [
                new LineSeries<double>
                {
                    Name = "使用量",
                    Values = ymemData,
                    Fill = null,
                    GeometrySize = 2,
                    YToolTipLabelFormatter = point => point.AsDataLabel + unitLabel,
                }
            ];
        }

        public void UpdateDiskData(string[] xdiskData, double[] ydiskWriteData, double[] ydiskReadData,
            double totalDiskWriteData, double totalDiskReadData, double maxValue, string unitLabel)
        {
            DiskTitle = "磁盘读写(读：" + Utils.FormatByteSize(5, totalDiskWriteData) + ", 写：" +
                        Utils.FormatByteSize(5, totalDiskReadData) + ")";
            DiskXAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    Labels = xdiskData,
                    LabelsRotation = 0,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                    SeparatorsAtCenter = false,
                    TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                    TicksAtCenter = true,
                    ForceStepToMin = false,
                    MinStep = 1
                }
            ];
            DiskYAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    MinLimit = 0,
                    MaxLimit = Math.Max(maxValue, 1),
                    ForceStepToMin = true,
                    MinStep = Math.Max(maxValue / 5, 0.2),
                    Labeler = value => Math.Round(value, 2) + unitLabel,
                }
            ];
            DiskSeries =
            [
                new ColumnSeries<double>
                {
                    Name = "读",
                    Values = ydiskReadData,
                    YToolTipLabelFormatter = point => point.AsDataLabel + unitLabel,
                },
                new ColumnSeries<double>
                {
                    Name = "写",
                    Values = ydiskWriteData,
                    YToolTipLabelFormatter = point => point.AsDataLabel + unitLabel,
                }
            ];
        }

        public void UpdateNetworkData(string[] xnetData, double[] ynetSentData, double[] ynetRecvData,
            double totalNetSentData, double totalNetRecvData, double maxValue, string unitLabel)
        {
            NetworkTitle = "网络传输(上传：" + Utils.FormatByteSize(5, totalNetSentData) + ", 下载：" +
                           Utils.FormatByteSize(5, totalNetRecvData) + ")";
            NetworkXAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    Labels = xnetData,
                    LabelsRotation = 0,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                    SeparatorsAtCenter = false,
                    TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                    TicksAtCenter = true,
                    ForceStepToMin = false,
                    MinStep = 1
                }
            ];
            NetworkYAxes =
            [
                new Axis
                {
                    TextSize = 9,
                    MinLimit = 0,
                    MaxLimit = Math.Max(maxValue, 1),
                    ForceStepToMin = true,
                    MinStep = Math.Max(maxValue / 5, 0.2),
                    Labeler = value => Math.Round(value, 2) + unitLabel,
                }
            ];
            NetworkSeries =
            [
                new ColumnSeries<double>
                {
                    Name = "上传",
                    Values = ynetSentData,
                    YToolTipLabelFormatter = point => point.AsDataLabel + unitLabel,
                },
                new ColumnSeries<double>
                {
                    Name = "下载",
                    Values = ynetRecvData,
                    YToolTipLabelFormatter = point => point.AsDataLabel + unitLabel,
                }
            ];
        }

        public void UpdateData(List<string> xData, List<double> ycpuData, List<double> ymemData, 
            List<double> ydiskWriteData, List<double> ydiskReadData, List<double> ynetSentData, List<double> ynetRecvData,
            double totalDiskWriteData, double totalDiskReadData, double totalNetSentData, double totalNetRecvData,
            double maxMemValue, double maxDiskValue, double maxNetValue)
        {
            UpdateCpuData(xData.ToArray(), ycpuData.Select(val => Math.Round(val, 2)).ToArray());
            UpdateMemData(xData.ToArray(), Utils.FormatByteValues(ymemData.ToArray(), maxMemValue), 
                Math.Ceiling(maxMemValue / Utils.GetByteScale(maxMemValue)), Utils.GetByteUnitLabel(maxMemValue));
            UpdateDiskData(xData.ToArray(), Utils.FormatByteValues(ydiskWriteData.ToArray(), maxDiskValue), 
                Utils.FormatByteValues(ydiskReadData.ToArray(), maxDiskValue), totalDiskWriteData, totalDiskReadData, 
                Utils.CeilingByteValue(maxDiskValue / Utils.GetByteScale(maxDiskValue)), Utils.GetByteUnitLabel(maxDiskValue));
            UpdateNetworkData(xData.ToArray(), Utils.FormatByteValues(ynetSentData.ToArray(), maxNetValue), 
                Utils.FormatByteValues(ynetRecvData.ToArray(), maxNetValue), totalNetSentData, totalNetRecvData,
                Utils.CeilingByteValue(maxNetValue / Utils.GetByteScale(maxNetValue)), Utils.GetByteUnitLabel(maxNetValue));
        }
    }
}