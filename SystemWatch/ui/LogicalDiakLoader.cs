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
    public class LogicalDiakLoader : Window
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

        public LogicalDiakLoader(Point location, Size clientSize) : base(location, clientSize)
        {
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

            this.canvasView = new Canvas(new Point(10, 85), new Size(126, 40), 3, 126, new Color[] { this.NormalColor[0], this.NormalColor[1], this.NormalColor[1] });
            this.canvasView.RefreshLatestDataEvent += this.UpdateLatestDatas;

            Program.GetInformation().SetDataToView(Information.DataType.LogicalDiskLoadPercent, this.canvasView, "_Total", new object[] { 1 });
            Program.GetInformation().SetDataToView(Information.DataType.LogicalDiskReadLoadPercent, this.canvasView, "_Total", new object[] { 2 });
            Program.GetInformation().SetDataToView(Information.DataType.LogicalDiskWriteLoadPercent, this.canvasView, "_Total", new object[] { 3 });
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

        private void UpdateLatestDatas(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            if (data[0] != null)
            {
                this.writeReadText = "T:" + this.FormatByteSize(5, data[0].current) + "/s";
            }

            if (data[1] != null)
            {
                this.readText = "R:" + this.FormatByteSize(5, data[1].current) + "/s";
                this.logicalDiskReadTotal += data[1].current;
                this.totalReadText = "RT:" + this.FormatByteSize(5, this.logicalDiskReadTotal);
            }

            if (data[2] != null)
            {
                this.writeText = "W:" + this.FormatByteSize(5, data[2].current) + "/s";
                this.logicalDiskWriteTotal += data[2].current;
                this.totalWriteText = "WT:" + this.FormatByteSize(5, this.logicalDiskWriteTotal);
            }
        }
    }
}
