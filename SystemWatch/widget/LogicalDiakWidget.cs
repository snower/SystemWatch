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
    public class LogicalDiakWidget : Widget
    {
        private double logicalDiskReadTotal;
        private double logicalDiskWriteTotal;

        private Canvas canvasView;
        private string writeReadText;
        private string readText;
        private string writeText;
        private string totalReadText;
        private string totalWriteText;

        private Font rwFont;
        private Font totalRwFont;

        private Brush writeReadBrush;
        private Brush writeBrush;
        private Brush readBrush;

        private Point writeReadLocation;
        private Point readLocation;
        private Point writeLoction;
        private Point totalReadLocation;
        private Point totalWriteLocation;

        public string WriteReadText
        {
            get
            {
                return this.writeReadText;
            }
        }

        public string ReadText
        {
            get
            {
                return this.readText;
            }
        }

        public string WriteText
        {
            get
            {
                return this.writeText;
            }
        }

        public string TotalReadText
        {
            get
            {
                return this.totalReadText;
            }
        }

        public string TotalWriteText
        {
            get
            {
                return this.totalWriteText;
            }
        }

        public LogicalDiakWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this.writeReadText = "T: 0B/s";
            this.writeText = "W: 0B/s";
            this.readText = "R: 0B/s";
            this.totalWriteText = "TW: 0B/s";
            this.totalReadText = "TR: 0B/s";

            this.rwFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.totalRwFont = new Font("微软雅黑", 6.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            this.writeReadBrush = new SolidBrush(this.NormalColor[0]);
            this.writeBrush = new SolidBrush(this.NormalColor[1]);
            this.readBrush = new SolidBrush(this.NormalColor[2]);

            this.writeReadLocation = new Point(80, 30);
            this.readLocation = new Point(75, 47);
            this.writeLoction = new Point(10, 47);
            this.totalReadLocation = new Point(75, 65);
            this.totalWriteLocation = new Point(10, 65);

            this.canvasView = new Canvas(new Point(10, 85), new Size(126, 40), 3, 123, new Color[] { this.NormalColor[0], this.NormalColor[1], this.NormalColor[1] });
            this.canvasView.RefreshLatestDataEvent += this.UpdateLatestDatas;

            Program.GetInformation().SetDataToView(Performance.DataType.LogicalDiskLoadPercent, this.canvasView, "_Total", new object[] { 1 });
            Program.GetInformation().SetDataToView(Performance.DataType.LogicalDiskReadLoadPercent, this.canvasView, "_Total", new object[] { 2 });
            Program.GetInformation().SetDataToView(Performance.DataType.LogicalDiskWriteLoadPercent, this.canvasView, "_Total", new object[] { 3 });
        }

        protected override void BackgroundPaint(Graphics g)
        {
            base.BackgroundPaint(g);
            this.PaintTitle(g, "Disk");
            this.canvasView.BackgroundPaint(g);
        }

        protected override void Paint(Graphics g)
        {
            base.Paint(g);

            g.DrawString(this.writeReadText, this.rwFont, this.writeReadBrush, this.writeReadLocation);
            g.DrawString(this.readText, this.rwFont, this.readBrush, this.readLocation);
            g.DrawString(this.writeText, this.rwFont, this.writeBrush, this.writeLoction);
            g.DrawString(this.totalReadText, this.totalRwFont, this.readBrush, this.totalReadLocation);
            g.DrawString(this.totalWriteText, this.totalRwFont, this.writeBrush, this.totalWriteLocation);
            this.canvasView.Paint(g);
        }

        public override void Close()
        {
            base.Close();
            this.canvasView.Close();
        }

        public override string GetShortNoticce()
        {
            return "Disk:" + this.writeReadText.Substring(3);
        }

        private void UpdateLatestDatas(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            switch (e.Channel)
            {
                case 1:
                    this.writeReadText = "T: " + this.FormatByteSize(5, data[0].current) + "/s";
                    break;
                case 2:
                    this.readText = "R: " + this.FormatByteSize(5, data[1].current) + "/s";
                    this.logicalDiskReadTotal += data[1].current;
                    this.totalReadText = "RT: " + this.FormatByteSize(5, this.logicalDiskReadTotal);
                    break;
                case 3:
                    this.writeText = "W: " + this.FormatByteSize(5, data[2].current) + "/s";
                    this.logicalDiskWriteTotal += data[2].current;
                    this.totalWriteText = "WT: " + this.FormatByteSize(5, this.logicalDiskWriteTotal);
                    break;
            }
        }
    }
}
