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
    public class CpuMemoryLoader : Window
    {
        private Canvas canvasView;
        private string cpuText;
        private string memText;

        private Font cpuFont;
        private Brush cpuBrush;
        private Point cpuLocation;

        private Font memFont;
        private Brush memBrush;
        private Point memLocation;

        public CpuMemoryLoader(Point location, Size clientSize) : base(location, clientSize)
        {
            this.cpuFont = new Font("微软雅黑", 9.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.cpuBrush = new SolidBrush(this.NormalColor[0]);
            this.cpuLocation = new Point(75, 27);

            this.memFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.memBrush = new SolidBrush(this.NormalColor[1]);
            this.memLocation = new Point(40, 50);

            this.canvasView = new Canvas(new Point(10, 85), new Size(126, 40), 2, 126, new Color[] { this.NormalColor[0], this.NormalColor[1]}, new bool[2] { false, true});
            this.canvasView.RefreshLatestDataEvent += this.UpdateLatestDatas;

            Program.GetInformation().SetDataToView(Information.DataType.ProcessorLoadPercent, this.canvasView, "_Total", new object[] { 1 });
            Program.GetInformation().SetDataToView(Information.DataType.MemoryLoadPercent, this.canvasView, "", new object[] { 2 });
        }

        protected override void BackgroundPaint(Graphics g)
        {
            base.BackgroundPaint(g);
            this.PaintTitle(g, "CPU");
            this.canvasView.BackgroundPaint(g);
        }

        protected override void Paint(Graphics g)
        {
            base.Paint(g);

            g.DrawString(this.cpuText, this.cpuFont, this.cpuBrush, this.cpuLocation);
            g.DrawString(this.memText, this.memFont, this.memBrush, this.memLocation);
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
                this.cpuText = "C:" + String.Format("{0:0.0}", data[0].percent) + "%";
            }

            if (data[1] != null)
            {
                this.memText = "M:" + String.Format("{0:0}", data[1].current / 1024) + "M/" + String.Format("{0:0}", data[1].total / 1024) + "M";
            }
        }
    }
}
