using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemWatch.ui
{
    public partial class StatisticsWindow : Form
    {
        protected string[] ByteUnits = new String[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        protected int TimePeriod = 7;
        public StatisticsWindow()
        {
            InitializeComponent();
            this.InitTodayStatistics();
            this.ShowMinuteChart();
        }

        private void InitTodayStatistics()
        {
            Statistics statistics = Program.GetStatistics();
            DateTime now = DateTime.Now;

            double maxMemBytes = Program.GetPerformance().PhysicalMemorySize;
            int unitIndex = 0;
            double memUintBytes = 1;
            while(maxMemBytes > 1024)
            {
                unitIndex++;
                memUintBytes *= 1024;
                maxMemBytes /= 1024;
            }

            List<string> xcpuData = new List<string>();
            List<double> ycpuData = new List<double>();
            List<string> xmemData = new List<string>();
            List<double> ymemData = new List<double>();

            for(int i = 0; i < 60; i++)
            {
                Statistics.DataChannel channel = statistics.CpuDataGroup.Channels[0];
                Statistics.Data data = channel.MinuteDatas[(channel.MinuteDataIndex + i) % 60];
                xcpuData.Add(data.Time.ToString("HH:mm"));
                ycpuData.Add(Math.Round(data.Value, 2));

                channel = statistics.MemoryDataGroup.Channels[0];
                data = channel.MinuteDatas[(channel.MinuteDataIndex + i) % 60];
                xmemData.Add(data.Time.ToString("HH:mm"));
                ymemData.Add(Math.Round(data.Value / memUintBytes, 2));
            }

            this.todayCpuMemChart.Series[1].Points.DataBindXY(xcpuData, ycpuData);
            this.todayCpuMemChart.Series[1].ToolTip = "#VALX: 使用CPU#VAL%";
            this.todayCpuMemChart.Series[0].Points.DataBindXY(xmemData, ymemData);
            this.todayCpuMemChart.Series[0].ToolTip = "#VALX: 使用内存#VAL" + ByteUnits[unitIndex];

            List<string> xDiskData = new List<string>() { "写", "读" };
            unitIndex = 0;
            double diskWrite = Math.Max(statistics.DiskDataGroup.Channels[0].TodayTotal, 1);
            double diskRead = Math.Max(statistics.DiskDataGroup.Channels[1].TodayTotal, 1);
            double rwMax = diskRead > diskWrite ? diskRead : diskWrite;
            while (rwMax >= 1024)
            {
                unitIndex++;
                rwMax /= 1024;
                diskRead /= 1024;
                diskWrite /= 1024;
            }
            List<double> yDiskData = new List<double>() { Math.Round(diskWrite, 2), Math.Round(diskRead, 2) };
            this.todayDiskChart.Series[0].Points.DataBindXY(xDiskData, yDiskData);
            this.todayDiskChart.Series[0].Label = "#VALX: #VAL" + ByteUnits[unitIndex];

            List<string> xNetData = new List<string>() { "上传", "下载" };
            unitIndex = 0;
            double netSent = Math.Max(statistics.NetworkDataGroup.Channels[0].TodayTotal, 1);
            double netRecv = Math.Max(statistics.NetworkDataGroup.Channels[1].TodayTotal, 1);
            double rsMax = netSent > netRecv ? netSent : netRecv;
            while (rsMax >= 1024)
            {
                unitIndex++;
                rsMax /= 1024;
                netRecv /= 1024;
                netSent /= 1024;
            }
            List<double> yNetData = new List<double>() { Math.Round(netSent, 2), Math.Round(netRecv, 2) };
            this.todayNetworkChart.Series[0].Points.DataBindXY(xNetData, yNetData);
            this.todayNetworkChart.Series[0].Label = "#VALX: #VAL" + ByteUnits[unitIndex];
        }

        private void ShowMinuteChart()
        {
            Statistics statistics = Program.GetStatistics();
            DateTime now = DateTime.Now;

            Dictionary<string, Statistics.Data> cpuDictDatas = this.ArrayToDict(statistics.CpuDataGroup.Channels[0].MinuteDatas, "HH:mm");
            Dictionary<string, Statistics.Data> memDictDatas = this.ArrayToDict(statistics.MemoryDataGroup.Channels[0].MinuteDatas, "HH:mm");
            Dictionary<string, Statistics.Data> diskWriteDictDatas = this.ArrayToDict(statistics.DiskDataGroup.Channels[0].MinuteDatas, "HH:mm");
            Dictionary<string, Statistics.Data> diskReadDictDatas = this.ArrayToDict(statistics.DiskDataGroup.Channels[1].MinuteDatas, "HH:mm");
            Dictionary<string, Statistics.Data> netSentDictDatas = this.ArrayToDict(statistics.NetworkDataGroup.Channels[0].MinuteDatas, "HH:mm");
            Dictionary<string, Statistics.Data> netRecvDictDatas = this.ArrayToDict(statistics.NetworkDataGroup.Channels[1].MinuteDatas, "HH:mm");

            List<string> xcpuData = new List<string>();
            List<double> ycpuData = new List<double>();

            List<string> xmemData = new List<string>();
            List<double> ymemData = new List<double>();

            double totalDiskWriteData = 0;
            double totalDiskReadData = 0;
            List<string> xdiskWriteData = new List<string>();
            List<double> ydiskWriteData = new List<double>();
            List<string> xdiskReadData = new List<string>();
            List<double> ydiskReadData = new List<double>();

            double totalNetSentData = 0;
            double totalNetRecvData = 0;
            List<string> xnetSentData = new List<string>();
            List<double> ynetSentData = new List<double>();
            List<string> xnetRecvData = new List<string>();
            List<double> ynetRecvData = new List<double>();

            for (int i = 0; i < 60; i++)
            {
                string dateKey = now.AddMinutes(-(56 - i)).ToString("HH:mm");

                xcpuData.Add(dateKey);
                ycpuData.Add(cpuDictDatas.ContainsKey(dateKey) ? Math.Round(cpuDictDatas[dateKey].Value, 2) : 0);

                xmemData.Add(dateKey);
                ymemData.Add(memDictDatas.ContainsKey(dateKey) ? Math.Round(memDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalDiskWriteData += diskWriteDictDatas.ContainsKey(dateKey) ? diskWriteDictDatas[dateKey].Value : 0;
                xdiskWriteData.Add(dateKey);
                ydiskWriteData.Add(diskWriteDictDatas.ContainsKey(dateKey) ? Math.Round(diskWriteDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalDiskReadData += diskReadDictDatas.ContainsKey(dateKey) ? diskReadDictDatas[dateKey].Value : 0;
                xdiskReadData.Add(dateKey);
                ydiskReadData.Add(diskReadDictDatas.ContainsKey(dateKey) ? Math.Round(diskReadDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalNetSentData += netSentDictDatas.ContainsKey(dateKey) ? netSentDictDatas[dateKey].Value : 0;
                xnetSentData.Add(dateKey);
                ynetSentData.Add(netSentDictDatas.ContainsKey(dateKey) ? Math.Round(netSentDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalNetRecvData += netRecvDictDatas.ContainsKey(dateKey) ? netRecvDictDatas[dateKey].Value : 0;
                xnetRecvData.Add(dateKey);
                ynetRecvData.Add(netRecvDictDatas.ContainsKey(dateKey) ? Math.Round(netRecvDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
            }

            this.cpuChart.Series[0].Points.DataBindXY(xcpuData, ycpuData);
            this.cpuChart.Series[0].ToolTip = "#VALX: 平均使用#VAL%";
            this.memChart.Series[0].Points.DataBindXY(xmemData, ymemData);
            this.memChart.Series[0].ToolTip = "#VALX: 平均使用#VALMB";
            this.diskChart.Series[0].Points.DataBindXY(xdiskWriteData, ydiskWriteData);
            this.diskChart.Series[0].ToolTip = "#VALX: 写#VALMB";
            this.diskChart.Series[1].Points.DataBindXY(xdiskReadData, ydiskReadData);
            this.diskChart.Series[1].ToolTip = " #VALX: 读#VALMB";
            this.networkChart.Series[0].Points.DataBindXY(xnetSentData, ynetSentData);
            this.networkChart.Series[0].ToolTip = "#VALX: 上传#VALMB";
            this.networkChart.Series[1].Points.DataBindXY(xnetRecvData, ynetRecvData);
            this.networkChart.Series[1].ToolTip = "#VALX: 下载#VALMB";
            this.label6.Text = "磁盘写(" + this.FormatByteSize(5, totalDiskWriteData) + ")读(" + this.FormatByteSize(2, totalDiskReadData) + ")";
            this.label7.Text = "网络上传(" + this.FormatByteSize(5, totalNetSentData) + ")下载(" + this.FormatByteSize(2, totalNetRecvData) + ")";
        }

        private void ShowHourChart()
        {
            Statistics statistics = Program.GetStatistics();
            DateTime now = DateTime.Now;

            Dictionary<string, Statistics.Data> cpuDictDatas = this.ArrayToDict(statistics.CpuDataGroup.Channels[0].HourDatas, "HH:00");
            Dictionary<string, Statistics.Data> memDictDatas = this.ArrayToDict(statistics.MemoryDataGroup.Channels[0].HourDatas, "HH:00");
            Dictionary<string, Statistics.Data> diskWriteDictDatas = this.ArrayToDict(statistics.DiskDataGroup.Channels[0].HourDatas, "HH:00");
            Dictionary<string, Statistics.Data> diskReadDictDatas = this.ArrayToDict(statistics.DiskDataGroup.Channels[1].HourDatas, "HH:00");
            Dictionary<string, Statistics.Data> netSentDictDatas = this.ArrayToDict(statistics.NetworkDataGroup.Channels[0].HourDatas, "HH:00");
            Dictionary<string, Statistics.Data> netRecvDictDatas = this.ArrayToDict(statistics.NetworkDataGroup.Channels[1].HourDatas, "HH:00");

            List<string> xcpuData = new List<string>();
            List<double> ycpuData = new List<double>();

            List<string> xmemData = new List<string>();
            List<double> ymemData = new List<double>();

            double totalDiskWriteData = 0;
            double totalDiskReadData = 0;
            List<string> xdiskWriteData = new List<string>();
            List<double> ydiskWriteData = new List<double>();
            List<string> xdiskReadData = new List<string>();
            List<double> ydiskReadData = new List<double>();

            double totalNetSentData = 0;
            double totalNetRecvData = 0;
            List<string> xnetSentData = new List<string>();
            List<double> ynetSentData = new List<double>();
            List<string> xnetRecvData = new List<string>();
            List<double> ynetRecvData = new List<double>();

            for (int i = 0; i < 24; i++)
            {
                string dateKey = now.AddHours(-(23 - i)).ToString("HH:00");

                xcpuData.Add(dateKey);
                ycpuData.Add(cpuDictDatas.ContainsKey(dateKey) ? Math.Round(cpuDictDatas[dateKey].Value, 2) : 0);

                xmemData.Add(dateKey);
                ymemData.Add(memDictDatas.ContainsKey(dateKey) ? Math.Round(memDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalDiskWriteData += diskWriteDictDatas.ContainsKey(dateKey) ? diskWriteDictDatas[dateKey].Value : 0;
                xdiskWriteData.Add(dateKey);
                ydiskWriteData.Add(diskWriteDictDatas.ContainsKey(dateKey) ? Math.Round(diskWriteDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalDiskReadData += diskReadDictDatas.ContainsKey(dateKey) ? diskReadDictDatas[dateKey].Value : 0;
                xdiskReadData.Add(dateKey);
                ydiskReadData.Add(diskReadDictDatas.ContainsKey(dateKey) ? Math.Round(diskReadDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalNetSentData += netSentDictDatas.ContainsKey(dateKey) ? netSentDictDatas[dateKey].Value : 0;
                xnetSentData.Add(dateKey);
                ynetSentData.Add(netSentDictDatas.ContainsKey(dateKey) ? Math.Round(netSentDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);

                totalNetRecvData += netRecvDictDatas.ContainsKey(dateKey) ? netRecvDictDatas[dateKey].Value : 0;
                xnetRecvData.Add(dateKey);
                ynetRecvData.Add(netRecvDictDatas.ContainsKey(dateKey) ? Math.Round(netRecvDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
            }

            this.cpuChart.Series[0].Points.DataBindXY(xcpuData, ycpuData);
            this.cpuChart.Series[0].ToolTip = "#VALX: 平均使用#VAL%";
            this.memChart.Series[0].Points.DataBindXY(xmemData, ymemData);
            this.memChart.Series[0].ToolTip = "#VALX: 平均使用#VALMB";
            this.diskChart.Series[0].Points.DataBindXY(xdiskWriteData, ydiskWriteData);
            this.diskChart.Series[0].ToolTip = "#VALX: 写#VALMB";
            this.diskChart.Series[1].Points.DataBindXY(xdiskReadData, ydiskReadData);
            this.diskChart.Series[1].ToolTip = " #VALX: 读#VALMB";
            this.networkChart.Series[0].Points.DataBindXY(xnetSentData, ynetSentData);
            this.networkChart.Series[0].ToolTip = "#VALX: 上传#VALMB";
            this.networkChart.Series[1].Points.DataBindXY(xnetRecvData, ynetRecvData);
            this.networkChart.Series[1].ToolTip = "#VALX: 下载#VALMB";
            this.label6.Text = "磁盘写(" + this.FormatByteSize(5, totalDiskWriteData) + ")读(" + this.FormatByteSize(2, totalDiskReadData) + ")";
            this.label7.Text = "网络上传(" + this.FormatByteSize(5, totalNetSentData) + ")下载(" + this.FormatByteSize(2, totalNetRecvData) + ")";
        }

        private void ShowDayChart()
        {
            Statistics statistics = Program.GetStatistics();
            DateTime now = DateTime.Now;

            Dictionary<string, Statistics.Data> cpuDictDatas = this.LinkListToDict(statistics.CpuDataGroup.Channels[0].DayDatas, "MM/dd");
            Dictionary<string, Statistics.Data> memDictDatas = this.LinkListToDict(statistics.MemoryDataGroup.Channels[0].DayDatas, "MM/dd");
            Dictionary<string, Statistics.Data> diskWriteDictDatas = this.LinkListToDict(statistics.DiskDataGroup.Channels[0].DayDatas, "MM/dd");
            Dictionary<string, Statistics.Data> diskReadDictDatas = this.LinkListToDict(statistics.DiskDataGroup.Channels[1].DayDatas, "MM/dd");
            Dictionary<string, Statistics.Data> netSentDictDatas = this.LinkListToDict(statistics.NetworkDataGroup.Channels[0].DayDatas, "MM/dd");
            Dictionary<string, Statistics.Data> netRecvDictDatas = this.LinkListToDict(statistics.NetworkDataGroup.Channels[1].DayDatas, "MM/dd");

            List<string> xcpuData = new List<string>();
            List<double> ycpuData = new List<double>();

            List<string> xmemData = new List<string>();
            List<double> ymemData = new List<double>();

            double totalDiskWriteData = 0;
            double totalDiskReadData = 0;
            List<string> xdiskWriteData = new List<string>();
            List<double> ydiskWriteData = new List<double>();
            List<string> xdiskReadData = new List<string>();
            List<double> ydiskReadData = new List<double>();

            double totalNetSentData = 0;
            double totalNetRecvData = 0;
            List<string> xnetSentData = new List<string>();
            List<double> ynetSentData = new List<double>();
            List<string> xnetRecvData = new List<string>();
            List<double> ynetRecvData = new List<double>();

            for (int i = 1; i <= this.TimePeriod; i++)
            {
                string dateKey = now.AddDays(-(this.TimePeriod - i)).ToString("MM/dd");

                if(ycpuData.Count > 0 || cpuDictDatas.ContainsKey(dateKey))
                {
                    xcpuData.Add(dateKey);
                    ycpuData.Add(cpuDictDatas.ContainsKey(dateKey) ? Math.Round(cpuDictDatas[dateKey].Value, 2) : 0);
                }

                if (ycpuData.Count > 0 || memDictDatas.ContainsKey(dateKey))
                {
                    xmemData.Add(dateKey);
                    ymemData.Add(memDictDatas.ContainsKey(dateKey) ? Math.Round(memDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
                }

                if (ycpuData.Count > 0 || diskWriteDictDatas.ContainsKey(dateKey))
                {
                    totalDiskWriteData += diskWriteDictDatas.ContainsKey(dateKey) ? diskWriteDictDatas[dateKey].Value : 0;
                    xdiskWriteData.Add(dateKey);
                    ydiskWriteData.Add(diskWriteDictDatas.ContainsKey(dateKey) ? Math.Round(diskWriteDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
                }

                if (ycpuData.Count > 0 || diskReadDictDatas.ContainsKey(dateKey))
                {
                    totalDiskReadData += diskReadDictDatas.ContainsKey(dateKey) ? diskReadDictDatas[dateKey].Value : 0;
                    xdiskReadData.Add(dateKey);
                    ydiskReadData.Add(diskReadDictDatas.ContainsKey(dateKey) ? Math.Round(diskReadDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
                }

                if (ycpuData.Count > 0 || netSentDictDatas.ContainsKey(dateKey))
                {
                    totalNetSentData += netSentDictDatas.ContainsKey(dateKey) ? netSentDictDatas[dateKey].Value : 0;
                    xnetSentData.Add(dateKey);
                    ynetSentData.Add(netSentDictDatas.ContainsKey(dateKey) ? Math.Round(netSentDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
                }

                if (ycpuData.Count > 0 || netRecvDictDatas.ContainsKey(dateKey))
                {
                    totalNetRecvData += netRecvDictDatas.ContainsKey(dateKey) ? netRecvDictDatas[dateKey].Value : 0;
                    xnetRecvData.Add(dateKey);
                    ynetRecvData.Add(netRecvDictDatas.ContainsKey(dateKey) ? Math.Round(netRecvDictDatas[dateKey].Value / 1024 / 1024, 2) : 0);
                }
            }

            this.cpuChart.Series[0].Points.DataBindXY(xcpuData, ycpuData);
            this.cpuChart.Series[0].ToolTip = "#VALX: 平均使用#VAL%";
            this.memChart.Series[0].Points.DataBindXY(xmemData, ymemData);
            this.memChart.Series[0].ToolTip = "#VALX: 平均使用#VALMB";
            this.diskChart.Series[0].Points.DataBindXY(xdiskWriteData, ydiskWriteData);
            this.diskChart.Series[0].ToolTip = "#VALX: 写#VALMB";
            this.diskChart.Series[1].Points.DataBindXY(xdiskReadData, ydiskReadData);
            this.diskChart.Series[1].ToolTip = " #VALX: 读#VALMB";
            this.networkChart.Series[0].Points.DataBindXY(xnetSentData, ynetSentData);
            this.networkChart.Series[0].ToolTip = "#VALX: 上传#VALMB";
            this.networkChart.Series[1].Points.DataBindXY(xnetRecvData, ynetRecvData);
            this.networkChart.Series[1].ToolTip = "#VALX: 下载#VALMB";
            this.label6.Text = "磁盘写(" + this.FormatByteSize(5, totalDiskWriteData) + ")读(" + this.FormatByteSize(2, totalDiskReadData) + ")";
            this.label7.Text = "网络上传(" + this.FormatByteSize(5, totalNetSentData) + ")下载(" + this.FormatByteSize(2, totalNetRecvData) + ")";
        }

        private Dictionary<string, Statistics.Data> ArrayToDict(Statistics.Data[] datas, string format)
        {
            Dictionary<string, Statistics.Data> dictDatas = new Dictionary<string, Statistics.Data>();
            foreach(Statistics.Data data in datas)
            {
                dictDatas[data.Time.ToString(format)] = data;
            }
            return dictDatas;
        }

        private Dictionary<string, Statistics.Data> LinkListToDict(LinkedList<Statistics.Data> datas, string format)
        {
            Dictionary<string, Statistics.Data> dictDatas = new Dictionary<string, Statistics.Data>();
            LinkedListNode<Statistics.Data> node = datas.First;
            while(node != null)
            {
                dictDatas[node.Value.Time.ToString(format)] = node.Value;
                node = node.Next;
            }
            return dictDatas;
        }

        private void timeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.timeComboBox.SelectedIndex)
            {
                case 0:
                    this.timePeriodComboBox.Visible = false;
                    this.ShowMinuteChart();
                    break;
                case 1:
                    this.timePeriodComboBox.Visible = false;
                    this.ShowHourChart();
                    break;
                case 2:
                    this.timePeriodComboBox.Visible = true;
                    this.ShowDayChart();
                    break;
            }
        }

        private void timePeriodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.timePeriodComboBox.SelectedIndex)
            {
                case 0:
                    this.TimePeriod = 7;
                    this.ShowDayChart();
                    break;
                case 1:
                    this.TimePeriod = 30;
                    this.ShowDayChart();
                    break;
                case 2:
                    this.TimePeriod = 365;
                    this.ShowDayChart();
                    break;
                case 3:
                    this.TimePeriod = 1;
                    this.ShowDayChart();
                    break;
                case 4:
                    this.TimePeriod = 2;
                    this.ShowDayChart();
                    break;
                case 5:
                    this.TimePeriod = 3;
                    this.ShowDayChart();
                    break;
                case 6:
                    this.TimePeriod = 5;
                    this.ShowDayChart();
                    break;
                case 7:
                    this.TimePeriod = 14;
                    this.ShowDayChart();
                    break;
                case 8:
                    this.TimePeriod = 90;
                    this.ShowDayChart();
                    break;
                case 9:
                    this.TimePeriod = 180;
                    this.ShowDayChart();
                    break;
                case 10:
                    this.TimePeriod = 270;
                    this.ShowDayChart();
                    break;
            }
        }

        protected String FormatByteSize(int len, double value, int type = 0)
        {
            for (int i = type, count = this.ByteUnits.Length; i < count; i++)
            {
                if (value < 1024)
                {
                    string result = Convert.ToString(value);
                    len = len > result.Length ? result.Length : len;
                    return result.Substring(0, len) + this.ByteUnits[i];
                }
                value /= 1024;
            }
            return "0B";
        }
    }
}
