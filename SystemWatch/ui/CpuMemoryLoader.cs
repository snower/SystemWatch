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
    public partial class CpuMemoryLoader : Window
    {
        public CpuMemoryLoader()
        {
            InitializeComponent();
            Program.GetInformation().SetDataToView(Information.DataType.ProcessorLoadPercent, this.drawImage, "_Total", new object[] { 1 });
            Program.GetInformation().SetDataToView(Information.DataType.MemoryLoadPercent, this.drawImage, "", new object[] { 2 });
            Program.GetInformation().SetDataToView(Information.DataType.ProcessorLoadPercent, this.drawImage, "0", new object[] { 3 });
            Program.GetInformation().SetDataToView(Information.DataType.ProcessorLoadPercent, this.drawImage, "1", new object[] { 4 });

            this.cpuLoadLable.ForeColor = this.NormalColor[0];
            this.drawImage.colorChannel[0] = this.NormalColor[0];
            this.memLoadLable.ForeColor = this.NormalColor[1];
            this.drawImage.colorChannel[1] = this.NormalColor[1];
            this.cpuCoreOneLoadLable.ForeColor = this.NormalColor[2];
            this.drawImage.colorChannel[2] = this.NormalColor[2];
            this.cpuCoreTwoLoadLable.ForeColor = this.NormalColor[3];
            this.drawImage.colorChannel[3] = this.NormalColor[3];
        }

        private void Window_RefrshLabel(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            if (data[0] != null)this.cpuLoadLable.Text = "C:" + String.Format("{0:0.0}", data[0].percent) + "%";
            if (data[1] != null) this.memLoadLable.Text = "M:" + String.Format("{0:0}", data[1].current / 1024) + "M/" + String.Format("{0:0}", data[1].total / 1024) + "M";
            if (data[2] != null) this.cpuCoreOneLoadLable.Text = "Core1:" + String.Format("{0:0.0}", data[2].percent) + "%";
            if (data[3] != null) this.cpuCoreTwoLoadLable.Text = "Core2:" + String.Format("{0:0.0}", data[3].percent) + "%";
        }

        public override void Close()
        {
            this.drawImage.Close();
            base.Close();
        }
    }
}
