using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace SystemWatch
{
    public class CpuMemoryWidget : Widget
    {
        private double _usedMemoryByte;
        private double _totalMemoyByte;

        private readonly Canvas _canvasView;
        private string _cpuText;
        private string _memText;
        private string _totalMemText;

        private readonly Typeface _cpuFont;
        private readonly Typeface _memFont;

        private readonly Brush _cpuBrush;
        private readonly Brush _memBrush;

        private readonly Point _cpuLocation;
        private readonly Point _memLocation;
        private readonly Point _totalMemLocation;

        public string CpuText => this._cpuText;

        public string MemText => this._memText;

        public string TotalMemText => this._totalMemText;

        public CpuMemoryWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this._cpuText = "C: 0%";
            this._memText = "U: 0B";
            this._totalMemText = "T: 0B";

            this._cpuFont = new Typeface("微软雅黑");
            this._memFont = new Typeface("微软雅黑");

            this._cpuBrush = new SolidColorBrush(this.NormalColor[0]);
            this._memBrush = new SolidColorBrush(this.NormalColor[1]);

            this._cpuLocation = new Point(clientSize.Width / 2 - 12, 6);
            this._memLocation = new Point(12, 34);
            this._totalMemLocation = new Point(clientSize.Width / 2 + 2, 34);

            this._canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 120, 
                new Canvas.DataChannel[]
                {
                    new Canvas.DataChannel(0, this.NormalColor[0], false), 
                    new Canvas.DataChannel(1, this.NormalColor[1])
                }, this.NormalColor[0]);
            this._canvasView.DataUpdateEvent += this.UpdateLatestDatas;
            
            App app = (App) Application.Current;
            this._canvasView.ResetDataUpdateEvent += app.Statistics.CpuWidgetDataUpdateEvent;
            app.Performance.SetDataToView(Performance.DataType.ProcessorLoadPercent, this._canvasView, "_Total", new object[] { 0 });
            app.Performance.SetDataToView(Performance.DataType.MemoryLoadPercent, this._canvasView, "", new object[] { 1 });
        }

        protected override void BackgroundPaint(DrawingContext dc)
        {
            base.BackgroundPaint(dc);
            this.PaintTitle(dc, "CPU");
            this._canvasView.BackgroundPaint(dc);
        }

        protected override void Paint(DrawingContext dc)
        {
            base.Paint(dc);

            dc.DrawText(new FormattedText(this._cpuText, this.CultureInfo, FlowDirection.LeftToRight, this._cpuFont,
                12F, this._cpuBrush), this._cpuLocation);
            dc.DrawText(new FormattedText(this._memText, this.CultureInfo, FlowDirection.LeftToRight, this._memFont, 
                9F, this._memBrush), this._memLocation);
            dc.DrawText(new FormattedText(this._totalMemText, this.CultureInfo, FlowDirection.LeftToRight, this._memFont, 
                9F, this._memBrush), this._totalMemLocation);
            this._canvasView.Paint(dc);
        }

        public override void Close()
        {
            base.Close();
            _canvasView.Close();
        }

        public override string GetShortNoticce()
        {
            return "Cpu:" + this._cpuText.Substring(3) + " Mem:" + this.MemText.Substring(3);
        }

        private void UpdateLatestDatas(object? sender, Canvas.DataUpdateEventArgs e)
        {
            Canvas.Data data = e.Channel.LatestDdata;
            switch (e.Channel.ChannelId)
            {
                case 0:
                    this._cpuText = "C: " + String.Format("{0:0.00}", data.Percent) + "%";
                    break;
                case 1:
                    if (Math.Abs(this._usedMemoryByte - data.Current) > 0.0001d)
                    {
                        this._usedMemoryByte = data.Current;
                        this._memText = "U: " + this.FormatByteSize(5, this._usedMemoryByte);
                    }

                    if (Math.Abs(this._totalMemoyByte - data.Total) > 0.0001d)
                    {
                        this._totalMemoyByte = data.Total;
                        this._totalMemText = "T: " + this.FormatByteSize(5, this._totalMemoyByte);
                    }
                    break;
            }
        }
    }
}
