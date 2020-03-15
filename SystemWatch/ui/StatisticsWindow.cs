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
        public StatisticsWindow()
        {
            InitializeComponent();

            Statistics statistics = Program.GetStatistics();

            List<string> xDiskData = new List<string>() { "写", "读" };
            int unitIndex = 0;
            double diskWrite = Math.Max(statistics.DiskDataGroup.Channels[0].TodayTotal, 1);
            double diskRead = Math.Max(statistics.DiskDataGroup.Channels[1].TodayTotal, 1);
            double rwMax = diskRead > diskWrite ? diskRead : diskWrite;
            while(rwMax >= 1024)
            {
                unitIndex++;
                rwMax /= 1024;
                diskRead /= 1024;
                diskWrite /= 1024;
            }
            List<double> yDiskData = new List<double>() {Math.Round(diskWrite, 2), Math.Round(diskRead, 2) };
            this.chart1.Series[0].Points.DataBindXY(xDiskData, yDiskData);
            this.chart1.Series[0].Label = "#VALX: #VAL" + ByteUnits[unitIndex];

            List<string> xNetData = new List<string>() { "上传", "下载"};
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
            this.chart2.Series[0].Points.DataBindXY(xNetData, yNetData);
            this.chart2.Series[0].Label = "#VALX: #VAL" + ByteUnits[unitIndex];
        }
    }
}
