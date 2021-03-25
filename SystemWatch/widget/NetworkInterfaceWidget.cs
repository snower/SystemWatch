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
    public class NetworkInterfaceWidget : Widget
    {
        private double receivedTotalBytes;
        private double sentTotalBytes;

        private Canvas canvasView;
        private string sentReceivedText;
        private string receivedText;
        private string sentText;
        private string totalReceivedText;
        private string totalSentText;

        private Font sentReceivedFont;
        private Font sentOrReceivedFont;
        private Font totalNetFont;

        private Brush sentReceivedBrush;
        private Brush sentBrush;
        private Brush receivedBrush;

        private Point sentReceivedLocation;
        private Point receivedLocation;
        private Point sentLoction;
        private Point totalReceivedLocation;
        private Point totalSentLocation;

        public string SentReceivedText
        {
            get
            {
                return this.sentReceivedText;
            }
        }

        public string ReceivedText
        {
            get
            {
                return this.receivedText;
            }
        }

        public string SentText
        {
            get
            {
                return this.sentText;
            }
        }

        public string TotalReceivedText
        {
            get
            {
                return this.totalReceivedText;
            }
        }

        public string TotalSentText
        {
            get
            {
                return this.totalSentText;
            }
        }

        public NetworkInterfaceWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this.sentReceivedText = "T: 0B/s";
            this.receivedText = "↓: 0B/s";
            this.sentText = "↑: 0B/s";
            this.totalReceivedText = "R: 0B";
            this.totalSentText = "S: 0B";

            this.sentReceivedFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.sentOrReceivedFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.totalNetFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            this.sentReceivedBrush = new SolidBrush(this.NormalColor[0]);
            this.receivedBrush = new SolidBrush(this.NormalColor[1]);
            this.sentBrush = new SolidBrush(this.NormalColor[2]);

            this.sentReceivedLocation = new Point(clientSize.Width / 2 - 12, 7);
            this.receivedLocation = new Point(17, 28); 
            this.sentLoction = new Point(clientSize.Width / 2 + 2, 28);
            this.totalReceivedLocation = new Point(17, 45);
            this.totalSentLocation = new Point(clientSize.Width / 2 + 2, 45); 

            this.canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 120, new Canvas.DataChannel[] {
                new Canvas.DataChannel(0, this.NormalColor[0]), new Canvas.DataChannel(1, this.NormalColor[2]), new Canvas.DataChannel(2, this.NormalColor[1]) },
                this.NormalColor[0]);
            this.canvasView.DataUpdateEvent += this.UpdateLatestDatas;
            this.canvasView.ResetDataUpdateEvent += Program.GetStatistics().NetworkWidgetDataUpdateEvent;

            Program.GetPerformance().SetDataToView(Performance.DataType.NetworkInterfaceLoadPercent, this.canvasView, "", new object[] { 0 });
            Program.GetPerformance().SetDataToView(Performance.DataType.NetworkInterfaceSentLoadPercent, this.canvasView, "", new object[] { 1 });
            Program.GetPerformance().SetDataToView(Performance.DataType.NetworkInterfaceReceivedLoadPercent, this.canvasView, "", new object[] { 2 });
        }

        protected override void BackgroundPaint(Graphics g)
        {
            base.BackgroundPaint(g);
            this.PaintTitle(g, "NET");
            this.canvasView.BackgroundPaint(g);
        }

        protected override void Paint(Graphics g)
        {
            base.Paint(g);

            g.DrawString(this.sentReceivedText, this.sentReceivedFont, this.sentReceivedBrush, this.sentReceivedLocation);
            g.DrawString(this.receivedText, this.sentOrReceivedFont, this.receivedBrush, this.receivedLocation);
            g.DrawString(this.sentText, this.sentOrReceivedFont, this.sentBrush, this.sentLoction);
            g.DrawString(this.totalReceivedText, this.totalNetFont, this.receivedBrush, this.totalReceivedLocation);
            g.DrawString(this.totalSentText, this.totalNetFont, this.sentBrush, this.totalSentLocation);
            this.canvasView.Paint(g);
        }

        public override void Close()
        {
            base.Close();
            this.canvasView.Close();
        }

        public override string GetShortNoticce()
        {
            return "Net:" + this.sentReceivedText.Substring(3);
        }

        private void UpdateLatestDatas(object sender, Canvas.DataUpdateEventArgs e)
        {
            Canvas.Data data = e.Channel.LatestDdata;
            switch (e.Channel.ChannelID)
            {
                case 0:
                    this.sentReceivedText = "N: " + this.FormatByteSize(5, data.Current) + "/s";
                    break;
                case 1:
                    this.sentText = "↑: " + this.FormatByteSize(5, data.Current) + "/s";
                    if(data.Current != 0)
                    {
                        this.sentTotalBytes += data.Current;
                        this.totalSentText = "S: " + this.FormatByteSize(5, this.sentTotalBytes);
                    }
                    break;
                case 2:
                    this.receivedText = "↓: " + this.FormatByteSize(5, data.Current) + "/s";
                    if(data.Current != 0)
                    {
                        this.receivedTotalBytes += data.Current;
                        this.totalReceivedText = "R: " + this.FormatByteSize(5, this.receivedTotalBytes);
                    }
                    break;
            }
        }
    }
}
