using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemWatch
{
    public partial class LogicalDiakLoader : Window
    {
        private double logicalDiskReadTotal;
        private double logicalDiskWriteTotal;

        public LogicalDiakLoader()
        {
            InitializeComponent();
            this.drawImage.HeightAuto = true;
            Program.GetInformation().SetDataToView(Information.DataType.LogicalDiskLoadPercent, this.drawImage, "_Total", new object[] { 1 });
            Program.GetInformation().SetDataToView(Information.DataType.LogicalDiskReadLoadPercent, this.drawImage, "_Total", new object[] { 2 });
            Program.GetInformation().SetDataToView(Information.DataType.LogicalDiskWriteLoadPercent, this.drawImage, "_Total", new object[] { 3 });

            this.diskLoadLable.ForeColor = this.NormalColor[0];
            this.drawImage.colorChannel[0] = this.NormalColor[0];
            this.diskReadLoadLable.ForeColor = this.NormalColor[1];
            this.diskReadTotalLable.ForeColor = this.NormalColor[1];
            this.drawImage.colorChannel[1] = this.NormalColor[1];
            this.diskWriteLoadLable.ForeColor = this.NormalColor[2];
            this.diskWriteTotalLable.ForeColor = this.NormalColor[2];
            this.drawImage.colorChannel[2] = this.NormalColor[2];
        }

        private void Window_RefrshLabel(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            if (data[0] != null) this.diskLoadLable.Text = "T:" + this.FormatByteSize(5, data[0].current) + "/s";
            if (data[1] != null)
            {
                this.diskReadLoadLable.Text = "R:" + this.FormatByteSize(5, data[1].current) + "/s";
                this.logicalDiskReadTotal += data[1].current;
                this.diskReadTotalLable.Text = "RT:" + this.FormatByteSize(5, this.logicalDiskReadTotal);
            }
            if (data[2] != null)
            {
                this.diskWriteLoadLable.Text = "W:" + this.FormatByteSize(5, data[2].current) + "/s";
                this.logicalDiskWriteTotal += data[2].current;
                this.diskWriteTotalLable.Text = "WT:" + this.FormatByteSize(5, this.logicalDiskWriteTotal);
            }
        }

        public override void Close()
        {
            this.drawImage.Close();
            base.Close();
        }
    }
}
