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
    public partial class NetworkInterfaceLoader : Window
    {
        private double receivedTotalBytes;
        private double sentTotalBytes;

        public NetworkInterfaceLoader()
        {
            InitializeComponent();
            this.drawImage.HeightAuto = true;
            Program.GetInformation().SetDataToView(Information.DataType.NetworkInterfaceLoadPercent, this.drawImage, "Intel[R] WiFi 链接 5100 AGN", new object[] { 1 });
            Program.GetInformation().SetDataToView(Information.DataType.NetworkInterfaceReceivedLoadPercent, this.drawImage, "Intel[R] WiFi 链接 5100 AGN", new object[] { 2 });
            Program.GetInformation().SetDataToView(Information.DataType.NetworkInterfaceSentLoadPercent, this.drawImage, "Intel[R] WiFi 链接 5100 AGN", new object[] { 3 });

            this.networkInterfaceLoad.ForeColor = this.NormalColor[0];
            this.drawImage.colorChannel[0] = this.NormalColor[0];
            this.networkInterfaceReceivedLoad.ForeColor = this.NormalColor[1];
            this.networkInterfaceReceivedTotal.ForeColor = this.NormalColor[1];
            this.drawImage.colorChannel[1] = this.NormalColor[1];
            this.networkInterfaceSentLoad.ForeColor = this.NormalColor[2];
            this.networkInterfaceSentTotal.ForeColor = this.NormalColor[2];
            this.drawImage.colorChannel[2] = this.NormalColor[2];
        }

        private void Window_RefrshLabel(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            if (data[0] != null) this.networkInterfaceLoad.Text = "T:" + this.FormatByteSize(5, data[0].current)+"/s";
            if (data[1] != null)
            {
                this.networkInterfaceReceivedLoad.Text = "R:" + this.FormatByteSize(5, data[1].current) + "/s";
                this.receivedTotalBytes += data[1].current;
                this.networkInterfaceReceivedTotal.Text = "RT:" + this.FormatByteSize(5, this.receivedTotalBytes) + "B";
            }
            if (data[2] != null)
            {
                this.networkInterfaceSentLoad.Text = "S:" + this.FormatByteSize(5, data[2].current) + "/s";
                this.sentTotalBytes += data[2].current;
                this.networkInterfaceSentTotal.Text = "ST:" + this.FormatByteSize(5, this.sentTotalBytes) + "B";
            }
        }

        public override void Close()
        {
            this.drawImage.Close();
            base.Close();
        }
    }
}
