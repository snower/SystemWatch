using System.Globalization;
using Avalonia;
using Avalonia.Media;
using SystemWatch.Datas;

namespace SystemWatch.Widgets
{
    public class NetworkInterfaceWidget : Widget
    {
        private double _receivedTotalBytes;
        private double _sentTotalBytes;

        private readonly Canvas _canvasView;
        private string _sentReceivedText;
        private string _receivedText;
        private string _sentText;
        private string _totalReceivedText;
        private string _totalSentText;

        private readonly Typeface _sentReceivedFont;
        private readonly Typeface _sentOrReceivedFont;
        private readonly Typeface _totalNetFont;

        private readonly Brush _sentReceivedBrush;
        private readonly Brush _sentBrush;
        private readonly Brush _receivedBrush;

        private readonly Point _sentReceivedLocation;
        private readonly Point _receivedLocation;
        private readonly Point _sentLoction;
        private readonly Point _totalReceivedLocation;
        private readonly Point _totalSentLocation;

        public string SentReceivedText => this._sentReceivedText;

        public string ReceivedText => this._receivedText;

        public string SentText => this._sentText;

        public string TotalReceivedText => this._totalReceivedText;

        public string TotalSentText => this._totalSentText;

        public NetworkInterfaceWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this._sentReceivedText = "T: 0B/s";
            this._receivedText = "↓: 0B/s";
            this._sentText = "↑: 0B/s";
            this._totalReceivedText = "R: 0B";
            this._totalSentText = "S: 0B";

            this._sentReceivedFont = new Typeface("微软雅黑");
            this._sentOrReceivedFont = new Typeface("微软雅黑");
            this._totalNetFont = new Typeface("微软雅黑");

            this._sentReceivedBrush = new SolidColorBrush(this.NormalColor[0]);
            this._receivedBrush = new SolidColorBrush(this.NormalColor[1]);
            this._sentBrush = new SolidColorBrush(this.NormalColor[2]);

            this._sentReceivedLocation = new Point(clientSize.Width / 2 - 12, 7);
            this._receivedLocation = new Point(17, 28); 
            this._sentLoction = new Point(clientSize.Width / 2 + 2, 28);
            this._totalReceivedLocation = new Point(17, 45);
            this._totalSentLocation = new Point(clientSize.Width / 2 + 2, 45); 

            this._canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 120, 
                new Canvas.DataChannel[] 
                { 
                    new Canvas.DataChannel(0, this.NormalColor[0]),
                    new Canvas.DataChannel(1, Color.FromArgb(180, this.NormalColor[2].R, this.NormalColor[2].G, this.NormalColor[2].B)), 
                    new Canvas.DataChannel(2, Color.FromArgb(180, this.NormalColor[1].R, this.NormalColor[1].G, this.NormalColor[1].B)) 
                }, 
                this.NormalColor[0]);
            this._canvasView.DataUpdateEvent += this.UpdateLatestDatas;
            
            App app = (App) Application.Current;
            this._canvasView.ResetDataUpdateEvent += app.Statistics.NetworkWidgetDataUpdateEvent;
            app.Performance.SetDataToView(Performance.DataType.NetworkInterfaceLoadPercent, this._canvasView, "", new object[] { 0 });
            app.Performance.SetDataToView(Performance.DataType.NetworkInterfaceSentLoadPercent, this._canvasView, "", new object[] { 1 });
            app.Performance.SetDataToView(Performance.DataType.NetworkInterfaceReceivedLoadPercent, this._canvasView, "", new object[] { 2 });
        }

        protected override void BackgroundPaint(DrawingContext dc)
        {
            base.BackgroundPaint(dc);
            this.PaintTitle(dc, "NET");
            this._canvasView.BackgroundPaint(dc);
        }

        protected override void Paint(DrawingContext dc)
        {
            base.Paint(dc);

            dc.DrawText(new FormattedText(this._sentReceivedText, this.CultureInfo, FlowDirection.LeftToRight, this._sentReceivedFont, 
                12F, this._sentReceivedBrush), this._sentReceivedLocation);
            dc.DrawText(new FormattedText(this._receivedText, this.CultureInfo, FlowDirection.LeftToRight, this._sentOrReceivedFont, 
                9F, this._receivedBrush), this._receivedLocation);
            dc.DrawText(new FormattedText(this._sentText, this.CultureInfo, FlowDirection.LeftToRight, this._sentOrReceivedFont, 
                9F, this._sentBrush), this._sentLoction);
            dc.DrawText(new FormattedText(this._totalReceivedText, this.CultureInfo, FlowDirection.LeftToRight, this._totalNetFont, 
                9F, this._receivedBrush), this._totalReceivedLocation);
            dc.DrawText(new FormattedText(this._totalSentText, this.CultureInfo, FlowDirection.LeftToRight, this._totalNetFont, 
                9F, this._sentBrush), this._totalSentLocation);
            this._canvasView.Paint(dc);
        }
        
        public override void Close()
        {
            base.Close();
            _canvasView.Close();
        }
        
        public override string GetShortNoticce()
        {
            return "Net:" + this._sentReceivedText.Substring(3);
        }

        private void UpdateLatestDatas(object? sender, Canvas.DataUpdateEventArgs e)
        {
            Canvas.Data data = e.Channel.LatestDdata;
            switch (e.Channel.ChannelId)
            {
                case 0:
                    this._sentReceivedText = "N: " + this.FormatByteSize(5, data.Current) + "/s";
                    break;
                case 1:
                    this._sentText = "↑: " + this.FormatByteSize(5, data.Current) + "/s";
                    if(data.Current != 0)
                    {
                        this._sentTotalBytes += data.Current;
                        this._totalSentText = "S: " + this.FormatByteSize(5, this._sentTotalBytes);
                    }
                    break;
                case 2:
                    this._receivedText = "↓: " + this.FormatByteSize(5, data.Current) + "/s";
                    if(data.Current != 0)
                    {
                        this._receivedTotalBytes += data.Current;
                        this._totalReceivedText = "R: " + this.FormatByteSize(5, this._receivedTotalBytes);
                    }
                    break;
            }
        }
    }
}
