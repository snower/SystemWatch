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
    public class CpuMemoryWidget : Widget
    {
        private double usedMemoryByte;
        private double totalMemoyByte;

        private Canvas canvasView;
        private string cpuText;
        private string memText;
        private string totalMemText;

        private Font cpuFont;
        private Font memFont;

        private Brush cpuBrush;
        private Brush memBrush;

        private Point cpuLocation;
        private Point memLocation;
        private Point totalMemLocation;

        public string CpuText
        {
            get
            {
                return this.cpuText;
            }
        }

        public string MemText
        {
            get
            {
                return this.memText;
            }
        }

        public string TotalMemText
        {
            get
            {
                return this.totalMemText;
            }
        }

        public CpuMemoryWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this.cpuText = "C: 0%";
            this.memText = "U: 0B";
            this.totalMemText = "T: 0B";

            this.cpuFont = new Font("微软雅黑", 9.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.memFont = new Font("微软雅黑", 8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            this.cpuBrush = new SolidBrush(this.NormalColor[0]);
            this.memBrush = new SolidBrush(this.NormalColor[1]);

            this.cpuLocation = new Point(clientSize.Width / 2 - 12, 6);
            this.memLocation = new Point(12, 34);
            this.totalMemLocation = new Point(clientSize.Width / 2 + 2, 34);

            this.canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 120, new Canvas.DataChannel[] { 
                new Canvas.DataChannel(0, this.NormalColor[0], false), new Canvas.DataChannel(1, this.NormalColor[1])}, this.NormalColor[0]);
            this.canvasView.DataUpdateEvent += this.UpdateLatestDatas;
            this.canvasView.ResetDataUpdateEvent += Program.GetStatistics().CpuWidgetDataUpdateEvent;

            Program.GetPerformance().SetDataToView(Performance.DataType.ProcessorLoadPercent, this.canvasView, "_Total", new object[] { 0 });
            Program.GetPerformance().SetDataToView(Performance.DataType.MemoryLoadPercent, this.canvasView, "", new object[] { 1 });
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
            g.DrawString(this.totalMemText, this.memFont, this.memBrush, this.totalMemLocation);
            this.canvasView.Paint(g);
        }

        public override void Close()
        {
            base.Close();
            this.canvasView.Close();
        }

        public override string GetShortNoticce()
        {
            return "Cpu:" + this.cpuText.Substring(3) + " Mem:" + this.MemText.Substring(3);
        }

        private void UpdateLatestDatas(object sender, Canvas.DataUpdateEventArgs e)
        {
            Canvas.Data data = e.Channel.LatestDdata;
            switch (e.Channel.ChannelID)
            {
                case 0:
                    this.cpuText = "C: " + String.Format("{0:0.00}", data.Percent) + "%";
                    break;
                case 1:
                    if (this.usedMemoryByte != data.Current)
                    {
                        this.usedMemoryByte = data.Current;
                        this.memText = "U: " + this.FormatByteSize(5, this.usedMemoryByte);
                    }

                    if (this.totalMemoyByte != data.Total)
                    {
                        this.totalMemoyByte = data.Total;
                        this.totalMemText = "T: " + this.FormatByteSize(5, this.totalMemoyByte);
                    }
                    break;
            }
        }
    }
}
