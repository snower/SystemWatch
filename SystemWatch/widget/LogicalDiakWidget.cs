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

        private Font writeReadFont;
        private Font writeOrReadFont;
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
            this.writeReadText = "D: 0B/s";
            this.readText = "↓: 0B/s";
            this.writeText = "↑: 0B/s";
            this.totalReadText = "R: 0B";
            this.totalWriteText = "W: 0B";

            this.writeReadFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.writeOrReadFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.totalRwFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            this.writeReadBrush = new SolidBrush(this.NormalColor[0]);
            this.readBrush = new SolidBrush(this.NormalColor[1]);
            this.writeBrush = new SolidBrush(this.NormalColor[2]);

            this.writeReadLocation = new Point(clientSize.Width / 2 - 12, 7);
            this.readLocation = new Point(17, 28);
            this.writeLoction = new Point(clientSize.Width / 2 + 2, 28); 
            this.totalReadLocation = new Point(17, 45);
            this.totalWriteLocation = new Point(clientSize.Width / 2 + 2, 45); 

            this.canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 120, new Canvas.DataChannel[] {
                new Canvas.DataChannel(0, this.NormalColor[0]), new Canvas.DataChannel(1, this.NormalColor[2]), new Canvas.DataChannel(2, this.NormalColor[1]) },
                this.NormalColor[0]);
            this.canvasView.DataUpdateEvent += this.UpdateLatestDatas;
            this.canvasView.ResetDataUpdateEvent += Program.GetStatistics().DiskWidgetDataUpdateEvent;

            Program.GetPerformance().SetDataToView(Performance.DataType.LogicalDiskLoadPercent, this.canvasView, "_Total", new object[] { 0 });
            Program.GetPerformance().SetDataToView(Performance.DataType.LogicalDiskWriteLoadPercent, this.canvasView, "_Total", new object[] { 1 });
            Program.GetPerformance().SetDataToView(Performance.DataType.LogicalDiskReadLoadPercent, this.canvasView, "_Total", new object[] { 2 });
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

            g.DrawString(this.writeReadText, this.writeReadFont, this.writeReadBrush, this.writeReadLocation);
            g.DrawString(this.readText, this.writeOrReadFont, this.readBrush, this.readLocation);
            g.DrawString(this.writeText, this.writeOrReadFont, this.writeBrush, this.writeLoction);
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

        private void UpdateLatestDatas(object sender, Canvas.DataUpdateEventArgs e)
        {
            Canvas.Data data = e.Channel.LatestDdata;
            switch (e.Channel.ChannelID)
            {
                case 0:
                    this.writeReadText = "D: " + this.FormatByteSize(5, data.Current) + "/s";
                    break;
                case 1:
                    this.writeText = "↑: " + this.FormatByteSize(5, data.Current) + "/s";
                    if (data.Current != 0)
                    {
                        this.logicalDiskWriteTotal += data.Current;
                        this.totalWriteText = "W: " + this.FormatByteSize(5, this.logicalDiskWriteTotal);
                    }
                    break;
                case 2:
                    this.readText = "↓: " + this.FormatByteSize(5, data.Current) + "/s";
                    if (data.Current != 0)
                    {
                        this.logicalDiskReadTotal += data.Current;
                        this.totalReadText = "R: " + this.FormatByteSize(5, this.logicalDiskReadTotal);
                    }
                    break;
            }
        }
    }
}
