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

            this.cpuLoadLable.ForeColor = this.NormalColor[0];
            this.drawImage.colorChannel[0] = this.NormalColor[0];
            this.memLoadLable.ForeColor = this.NormalColor[1];
            this.drawImage.colorChannel[1] = this.NormalColor[1];
        }

        private void Window_RefrshLabel(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            if (data[0] != null)this.cpuLoadLable.Text = "C:" + String.Format("{0:0.0}", data[0].percent) + "%";
            if (data[1] != null) this.memLoadLable.Text = "M:" + String.Format("{0:0}", data[1].current / 1024) + "M/" + String.Format("{0:0}", data[1].total / 1024) + "M";
        }

        public override void Close()
        {
            this.drawImage.Close();
            base.Close();
        }
    }
}
