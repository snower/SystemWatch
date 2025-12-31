using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SystemWatch.Datas;
using SystemWatch.ViewModels;

namespace SystemWatch.Repositorys
{

    public class StatisticsRepository
    {
        public void LoadData(StatisticsViewModel viewModel)
        {
            switch (viewModel.TimePeriodType)
            {
                case TimePeriodType.Minutes:
                    LoadMinutesDatas(viewModel);
                    break;
                case TimePeriodType.Hours:
                    LoadHourDatas(viewModel);
                    break;
                default:
                    LoadDayDatas(viewModel);
                    break;
            }
        }
        
        public void LoadMinutesDatas(StatisticsViewModel viewModel)
        {
            App? app = Application.Current as App;
            Statistics? statistics = app?.Statistics;
            if (statistics == null) return;
            DateTime now = DateTime.Now;
            double physicalMemorySize = app?.Performance?.PhysicalMemorySize ?? 0;

            Dictionary<string, Statistics.Data> cpuDatas = ConvertDatasToDictionary(statistics.CpuDataGroup.Channels[0].MinuteDatas, "yyyy/MM/dd HH:mm");
            Dictionary<string, Statistics.Data> memDatas = ConvertDatasToDictionary(statistics.MemoryDataGroup.Channels[0].MinuteDatas, "yyyy/MM/dd HH:mm");
            Dictionary<string, Statistics.Data> diskWriteDatas = ConvertDatasToDictionary(statistics.DiskDataGroup.Channels[0].MinuteDatas, "yyyy/MM/dd HH:mm");
            Dictionary<string, Statistics.Data> diskReadDatas = ConvertDatasToDictionary(statistics.DiskDataGroup.Channels[1].MinuteDatas, "yyyy/MM/dd HH:mm");
            Dictionary<string, Statistics.Data> netSentDatas = ConvertDatasToDictionary(statistics.NetworkDataGroup.Channels[0].MinuteDatas, "yyyy/MM/dd HH:mm");
            Dictionary<string, Statistics.Data> netRecvDatas = ConvertDatasToDictionary(statistics.NetworkDataGroup.Channels[1].MinuteDatas, "yyyy/MM/dd HH:mm");
        
            List<string> xData = new List<string>();
            List<double> ycpuData = new List<double>(), ymemData = new List<double>(), ydiskWriteData = new List<double>(),
                ydiskReadData = new List<double>(), ynetSentData = new List<double>(), ynetRecvData = new List<double>();
            double totalDiskWriteData = 0, totalDiskReadData = 0, totalNetSentData = 0,totalNetRecvData = 0;
            double maxMemValue = 0, maxDiskValue = 0, maxNeValue = 0;
            double total = 0;
        
            for (int i = 0; i <= viewModel.TimePeriod; i++)
            {

                DateTime dataTime = now.AddMinutes(-(viewModel.TimePeriod - i));
                xData.Add(dataTime.ToString("HH:mm"));
                
                string dateKey = dataTime.ToString("yyyy/MM/dd HH:mm");
                TryAddValue(cpuDatas, dateKey, ycpuData, 0, ref total);
                maxMemValue = TryAddValue(memDatas, dateKey, ymemData, maxMemValue, ref total);
                maxDiskValue = TryAddValue(diskWriteDatas, dateKey, ydiskWriteData, maxDiskValue, ref totalDiskWriteData);
                maxDiskValue = TryAddValue(diskReadDatas, dateKey, ydiskReadData, maxDiskValue, ref totalDiskReadData);
                maxNeValue = TryAddValue(netSentDatas, dateKey, ynetSentData, maxNeValue, ref totalNetSentData);
                maxNeValue = TryAddValue(netRecvDatas, dateKey, ynetRecvData, maxNeValue, ref totalNetRecvData);
            }
        
            viewModel.UpdateData(xData, ycpuData, ymemData, ydiskWriteData, ydiskReadData, ynetSentData, ynetRecvData,
                totalDiskWriteData, totalDiskReadData, totalNetSentData, totalNetRecvData, 
                Math.Max(physicalMemorySize, maxMemValue), maxDiskValue, maxNeValue);
        }
        
        public void LoadHourDatas(StatisticsViewModel viewModel)
        {
            App? app = Application.Current as App;
            Statistics? statistics = app?.Statistics;
            if (statistics == null) return;
            DateTime now = DateTime.Now;
            double physicalMemorySize = app?.Performance?.PhysicalMemorySize ?? 0;

            Dictionary<string, Statistics.Data> cpuDatas = ConvertDatasToDictionary(statistics.CpuDataGroup.Channels[0].HourDatas, "yyyy/MM/dd HH:00");
            Dictionary<string, Statistics.Data> memDatas = ConvertDatasToDictionary(statistics.MemoryDataGroup.Channels[0].HourDatas, "yyyy/MM/dd HH:00");
            Dictionary<string, Statistics.Data> diskWriteDatas = ConvertDatasToDictionary(statistics.DiskDataGroup.Channels[0].HourDatas, "yyyy/MM/dd HH:00");
            Dictionary<string, Statistics.Data> diskReadDatas = ConvertDatasToDictionary(statistics.DiskDataGroup.Channels[1].HourDatas, "yyyy/MM/dd HH:00");
            Dictionary<string, Statistics.Data> netSentDatas = ConvertDatasToDictionary(statistics.NetworkDataGroup.Channels[0].HourDatas, "yyyy/MM/dd HH:00");
            Dictionary<string, Statistics.Data> netRecvDatas = ConvertDatasToDictionary(statistics.NetworkDataGroup.Channels[1].HourDatas, "yyyy/MM/dd HH:00");
        
            List<string> xData = new List<string>();
            List<double> ycpuData = new List<double>(), ymemData = new List<double>(), ydiskWriteData = new List<double>(),
                ydiskReadData = new List<double>(), ynetSentData = new List<double>(), ynetRecvData = new List<double>();
            double totalDiskWriteData = 0, totalDiskReadData = 0, totalNetSentData = 0,totalNetRecvData = 0;
            double maxMemValue = 0, maxDiskValue = 0, maxNeValue = 0;
            double total = 0;
        
            for (int i = 0; i <= viewModel.TimePeriod; i++)
            {
                DateTime dataTime = now.AddHours(-(viewModel.TimePeriod - i));
                xData.Add(dataTime.ToString("HH:00"));
                
                string dateKey = dataTime.ToString("yyyy/MM/dd HH:00");
                TryAddValue(cpuDatas, dateKey, ycpuData, 0, ref total);
                maxMemValue = TryAddValue(memDatas, dateKey, ymemData, maxMemValue, ref total);
                maxDiskValue = TryAddValue(diskWriteDatas, dateKey, ydiskWriteData, maxDiskValue, ref totalDiskWriteData);
                maxDiskValue = TryAddValue(diskReadDatas, dateKey, ydiskReadData, maxDiskValue, ref totalDiskReadData);
                maxNeValue = TryAddValue(netSentDatas, dateKey, ynetSentData, maxNeValue, ref totalNetSentData);
                maxNeValue = TryAddValue(netRecvDatas, dateKey, ynetRecvData, maxNeValue, ref totalNetRecvData);
            }
        
            viewModel.UpdateData(xData, ycpuData, ymemData, ydiskWriteData, ydiskReadData, ynetSentData, ynetRecvData,
                totalDiskWriteData, totalDiskReadData, totalNetSentData, totalNetRecvData, 
                Math.Max(physicalMemorySize, maxMemValue), maxDiskValue, maxNeValue);
        }
        
        public void LoadDayDatas(StatisticsViewModel viewModel)
        {
            App? app = Application.Current as App;
            Statistics? statistics = app?.Statistics;
            if (statistics == null) return;
            DateTime now = DateTime.Now;
            double physicalMemorySize = app?.Performance?.PhysicalMemorySize ?? 0;

            Dictionary<string, Statistics.Data> cpuDatas = ConvertDatasToDictionary(statistics.CpuDataGroup.Channels[0].DayDatas, "yyyy/MM/dd");
            Dictionary<string, Statistics.Data> memDatas = ConvertDatasToDictionary(statistics.MemoryDataGroup.Channels[0].DayDatas, "yyyy/MM/dd");
            Dictionary<string, Statistics.Data> diskWriteDatas = ConvertDatasToDictionary(statistics.DiskDataGroup.Channels[0].DayDatas, "yyyy/MM/dd");
            Dictionary<string, Statistics.Data> diskReadDatas = ConvertDatasToDictionary(statistics.DiskDataGroup.Channels[1].DayDatas, "yyyy/MM/dd");
            Dictionary<string, Statistics.Data> netSentDatas = ConvertDatasToDictionary(statistics.NetworkDataGroup.Channels[0].DayDatas, "yyyy/MM/dd");
            Dictionary<string, Statistics.Data> netRecvDatas = ConvertDatasToDictionary(statistics.NetworkDataGroup.Channels[1].DayDatas, "yyyy/MM/dd");
        
            List<string> xData = new List<string>();
            List<double> ycpuData = new List<double>(), ymemData = new List<double>(), ydiskWriteData = new List<double>(),
                ydiskReadData = new List<double>(), ynetSentData = new List<double>(), ynetRecvData = new List<double>();
            double totalDiskWriteData = 0, totalDiskReadData = 0, totalNetSentData = 0,totalNetRecvData = 0;
            double maxMemValue = 0, maxDiskValue = 0, maxNeValue = 0;
            double total = 0;
        
            for (int i = 0; i <= viewModel.TimePeriod; i++)
            {
                DateTime dataTime = now.AddDays(-(viewModel.TimePeriod - i));
                xData.Add(dataTime.ToString("MM/dd"));
                
                string dateKey = dataTime.ToString("yyyy/MM/dd");
                TryAddValue(cpuDatas, dateKey, ycpuData, 0, ref total);
                maxMemValue = TryAddValue(memDatas, dateKey, ymemData, maxMemValue, ref total);
                maxDiskValue = TryAddValue(diskWriteDatas, dateKey, ydiskWriteData, maxDiskValue, ref totalDiskWriteData);
                maxDiskValue = TryAddValue(diskReadDatas, dateKey, ydiskReadData, maxDiskValue, ref totalDiskReadData);
                maxNeValue = TryAddValue(netSentDatas, dateKey, ynetSentData, maxNeValue, ref totalNetSentData);
                maxNeValue = TryAddValue(netRecvDatas, dateKey, ynetRecvData, maxNeValue, ref totalNetRecvData);
            }
        
            viewModel.UpdateData(xData, ycpuData, ymemData, ydiskWriteData, ydiskReadData, ynetSentData, ynetRecvData,
                totalDiskWriteData, totalDiskReadData, totalNetSentData, totalNetRecvData, 
                Math.Max(physicalMemorySize, maxMemValue), maxDiskValue, maxNeValue);
        }

        private double TryAddValue(Dictionary<string, Statistics.Data> datas, string key, List<double> values, double maxValue, ref double totalValue)
        {
            if (datas.TryGetValue(key, out var data))
            {
                values.Add(data.Value);
                totalValue += data.Value;
                if (data.Value > maxValue)
                {
                    return data.Value;
                }
                return maxValue;
            }
            values.Add(0);
            return maxValue;
        }

        private Dictionary<string, Statistics.Data> ConvertDatasToDictionary(LinkedList<Statistics.Data> datas, string keyFormat)
        {
            Dictionary<string, Statistics.Data> result = new Dictionary<string, Statistics.Data>();
            foreach (Statistics.Data data in datas)
            {
                if (data.Time <= DateTimeOffset.MinValue) continue;
                result[data.Time.ToString(keyFormat)] = data;
            }
            return result;
        }
        
        private Dictionary<string, Statistics.Data> ConvertDatasToDictionary(Statistics.Data[] datas, string keyFormat)
        {
            Dictionary<string, Statistics.Data> result = new Dictionary<string, Statistics.Data>();
            foreach (Statistics.Data data in datas)
            {
                if (data.Time <= DateTimeOffset.MinValue) continue;
                result[data.Time.ToString(keyFormat)] = data;
            }
            return result;
        }
    }
}