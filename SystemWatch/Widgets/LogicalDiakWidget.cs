using System.Globalization;
using Avalonia;
using Avalonia.Media;
using SystemWatch.Datas;

namespace SystemWatch.Widgets
{
    public class LogicalDiakWidget : Widget
    {
        private double _logicalDiskReadTotal;
        private double _logicalDiskWriteTotal;

        private readonly Canvas _canvasView;
        private string _writeReadText;
        private string _readText;
        private string _writeText;
        private string _totalReadText;
        private string _totalWriteText;

        private readonly Typeface _writeReadFont;
        private readonly Typeface _writeOrReadFont;
        private readonly Typeface _totalRwFont;

        private readonly Brush _writeReadBrush;
        private readonly Brush _writeBrush;
        private readonly Brush _readBrush;

        private readonly Point _writeReadLocation;
        private readonly Point _readLocation;
        private readonly Point _writeLoction;
        private readonly Point _totalReadLocation;
        private readonly Point _totalWriteLocation;

        public string WriteReadText => this._writeReadText;

        public string ReadText => this._readText;

        public string WriteText => this._writeText;

        public string TotalReadText => this._totalReadText;

        public string TotalWriteText => this._totalWriteText;

        public LogicalDiakWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this._writeReadText = "D: 0B/s";
            this._readText = "↑: 0B/s";
            this._writeText = "↓: 0B/s";
            this._totalReadText = "R: 0B";
            this._totalWriteText = "W: 0B";

            this._writeReadFont = new Typeface("微软雅黑");
            this._writeOrReadFont = new Typeface("微软雅黑");
            this._totalRwFont = new Typeface("微软雅黑");

            this._writeReadBrush = new SolidColorBrush(this.NormalColor[0]);
            this._readBrush = new SolidColorBrush(this.NormalColor[1]);
            this._writeBrush = new SolidColorBrush(this.NormalColor[2]);

            this._writeReadLocation = new Point(clientSize.Width / 2 - 12, 7);
            this._readLocation = new Point(17, 28);
            this._writeLoction = new Point(clientSize.Width / 2 + 2, 28); 
            this._totalReadLocation = new Point(17, 45);
            this._totalWriteLocation = new Point(clientSize.Width / 2 + 2, 45); 

            this._canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 120, 
                new Canvas.DataChannel[]
                {
                    new Canvas.DataChannel(0, this.NormalColor[0]), 
                    new Canvas.DataChannel(1, Color.FromArgb(180, this.NormalColor[2].R, this.NormalColor[2].G, this.NormalColor[2].B)), 
                    new Canvas.DataChannel(2, Color.FromArgb(180, this.NormalColor[1].R, this.NormalColor[1].G, this.NormalColor[1].B))
                }, this.NormalColor[0]);
            this._canvasView.DataUpdateEvent += this.UpdateLatestDatas;
            
            App app = (App) Application.Current;
            this._canvasView.ResetDataUpdateEvent += app.Statistics.DiskWidgetDataUpdateEvent;
            app.Performance.SetDataToView(Performance.DataType.LogicalDiskLoadPercent, this._canvasView, "_Total", new object[] { 0 });
            app.Performance.SetDataToView(Performance.DataType.LogicalDiskWriteLoadPercent, this._canvasView, "_Total", new object[] { 1 });
            app.Performance.SetDataToView(Performance.DataType.LogicalDiskReadLoadPercent, this._canvasView, "_Total", new object[] { 2 });
        }

        protected override void BackgroundPaint(DrawingContext dc)
        {
            base.BackgroundPaint(dc);
            this.PaintTitle(dc, "Disk");
            this._canvasView.BackgroundPaint(dc);
        }

        protected override void Paint(DrawingContext dc)
        {
            base.Paint(dc);

            dc.DrawText(new FormattedText(this._writeReadText, this.CultureInfo, FlowDirection.LeftToRight, this._writeReadFont, 
                12F, this._writeReadBrush), this._writeReadLocation);
            dc.DrawText(new FormattedText(this._readText, this.CultureInfo, FlowDirection.LeftToRight, this._writeOrReadFont, 
                9F, this._readBrush), this._readLocation);
            dc.DrawText(new FormattedText(this._writeText, this.CultureInfo, FlowDirection.LeftToRight, this._writeOrReadFont, 
                9F, this._writeBrush), this._writeLoction);
            dc.DrawText(new FormattedText(this._totalReadText, this.CultureInfo, FlowDirection.LeftToRight, this._totalRwFont, 
                9F, this._readBrush), this._totalReadLocation);
            dc.DrawText(new FormattedText(this._totalWriteText, this.CultureInfo, FlowDirection.LeftToRight, this._totalRwFont, 
                9F, this._writeBrush), this._totalWriteLocation);
            this._canvasView.Paint(dc);
        }
        
        public override void Close()
        {
            base.Close();
            _canvasView.Close();
        }

        public override string GetShortNoticce()
        {
            return "Disk:" + this._writeReadText.Substring(3);
        }

        private void UpdateLatestDatas(object? sender, Canvas.DataUpdateEventArgs e)
        {
            Canvas.Data data = e.Channel.LatestDdata;
            switch (e.Channel.ChannelId)
            {
                case 0:
                    this._writeReadText = "D: " + this.FormatByteSize(5, data.Current) + "/s";
                    break;
                case 1:
                    this._writeText = "↓: " + this.FormatByteSize(5, data.Current) + "/s";
                    if (data.Current != 0)
                    {
                        this._logicalDiskWriteTotal += data.Current;
                        this._totalWriteText = "W: " + this.FormatByteSize(5, this._logicalDiskWriteTotal);
                    }
                    break;
                case 2:
                    this._readText = "↑: " + this.FormatByteSize(5, data.Current) + "/s";
                    if (data.Current != 0)
                    {
                        this._logicalDiskReadTotal += data.Current;
                        this._totalReadText = "R: " + this.FormatByteSize(5, this._logicalDiskReadTotal);
                    }
                    break;
            }
        }
    }
}
