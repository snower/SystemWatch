using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace SystemWatch.Widgets
{
    public class Canvas : IPushData
    {
        private readonly string[] ByteUnits = new String[] { "B", "K", "M", "G", "T", "P", "E" };
        public class DataUpdateEventArgs : EventArgs
        {
            public DataChannel Channel { private set; get; }

            public DataUpdateEventArgs(DataChannel channel){
                this.Channel = channel;
            }
        };

        public class Data
        {
            public DateTime Time;
            public double Total;
            public double Current;
            public double Percent;

            public Data(DateTime time, double total, double current, double percent)
            {
                this.Time = time;
                this.Total = total;
                this.Current = current;
                this.Percent = percent;
            }

            public void Update(DateTime time, double total, double current, double percent)
            {
                this.Time = time;
                this.Total = total;
                this.Current = current;
                this.Percent = percent;
            }
        };

        public class DataChannel
        {
            public readonly int ChannelId;
            public Data[] Datas;
            public int DataCount;
            public int CurrentIndex;
            public Color PaintColor;
            public Point[] Points;
            public readonly bool CalcuMaxHeight;
            public double DataSum;
            public Data LatestDdata;
            public DataUpdateEventArgs DataUpdateEventArgs;

            public DataChannel(int channelId, Color paintColor, bool calcuMaxHeight = true)
            {
                this.ChannelId = channelId;
                this.PaintColor = paintColor;
                this.CalcuMaxHeight = calcuMaxHeight;
            }

            public void Init(int dataCount, int paintCount, Rect clientBounds)
            {
                this.DataSum = 0;
                this.DataCount = dataCount;
                this.Datas = new Data[this.DataCount];
                for(int i = 0; i < this.DataCount; i++)
                {
                    this.Datas[i] = new Data(new DateTime(0), 0, 0, 0);
                }
                this.CurrentIndex = 0;
                this.LatestDdata = this.Datas[0];
                this.Points = new Point[paintCount];
                for (int i = 0; i < paintCount; i++)
                {
                    this.Points[i] = new Point(0, 0);
                }
                this.DataUpdateEventArgs = new DataUpdateEventArgs(this);
            }
        }

        private readonly Point _location;
        private readonly Size _clientSize;
        private readonly DataChannel[] _channels;
        private readonly Pen[] _channelPens;
        private readonly StreamGeometry[] _channelGeometries;
        private readonly int _dataCount;

        private int _cx, _cy, _cw, _ch;
        private float _ix, _iy;
        private double _maxHeight;
        private string _maxHeightText;
        private readonly Typeface _maxHeightFont;
        private readonly Color _maxHeightColor;
        private Brush _maxHeightBrush;
        private Point _maxHeightPoint;
        private readonly CultureInfo _cultureInfo;

        public event EventHandler<DataUpdateEventArgs> DataUpdateEvent;
        public event EventHandler<DataUpdateEventArgs> ResetDataUpdateEvent;

        public Canvas(Point location, Size clientSize, int dataCount, DataChannel[] channels, Color maxHeightColor)
        {
            this._location = location;
            this._clientSize = clientSize;
            this._dataCount = dataCount;
            this._channels = channels;
            this._channelPens = new Pen[channels.Length];
            this._channelGeometries = new StreamGeometry[channels.Length];
            this._maxHeightFont = new Typeface("微软雅黑");
            this._maxHeightColor = maxHeightColor;
            this._maxHeight = 0;
            this._cx = (int) this._location.X + 1;
            this._cy = (int) this._location.Y + 1;
            this._cw = (int) this._clientSize.Width - 2;
            this._ch = (int) this._clientSize.Height - 2;
            this._ix = (float)this._cw / (float)this._dataCount;
            this._iy = (float)this._ch / 100F;
            this._maxHeightBrush = new SolidColorBrush(this._maxHeightColor);
            this._maxHeightPoint = new Point(this._cx + 2, this._cy);
            this._cultureInfo = CultureInfo.CurrentCulture;

            for (int i=0; i<this._channels.Length; i++)
            {
                this._channels[i].Init(this._dataCount, this._cw, new Rect(this._clientSize));
                this._channelPens[i] = new Pen(new SolidColorBrush(this._channels[i].PaintColor), 1.5)
                {
                    LineJoin = PenLineJoin.Bevel // 或 PenLineJoin.Round
                };
                this._channelGeometries[i] = new StreamGeometry();
            }
        }

        public void BackgroundPaint(DrawingContext dc)
        {
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(175, 175, 175)), null, 
                new Rect(this._cx -1, this._cy - 1, this._cw + 1, this._ch + 1));
            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(155, 155, 155)), 1);
            for(int i=1, count = this._cw / 10; i < count; i++){
                dc.DrawLine(pen, new Point(this._cx + i * 10, this._cy), new Point(this._cx + i * 10, this._cy + this._ch));
            }
            for (int i = 1, count = this._ch / 10; i < count; i++)
            {
                dc.DrawLine(pen, new Point(this._cx, this._cy + i * 10), new Point(this._cx + this._cw , this._cy + i * 10));
            }
        }

        public void Paint(DrawingContext dc)
        {
            for (int i = 0; i < this._channels.Length; i++)
            {
                this.PaintData(dc, this._channels[i], this._channelPens[i], this._channelGeometries[i]);
            }
            if(this._maxHeight > 0)
            {
                var formattedText = new FormattedText(this._maxHeightText, this._cultureInfo, FlowDirection.LeftToRight, 
                    this._maxHeightFont, 7, this._maxHeightBrush, 1.25);
                dc.DrawText(formattedText, this._maxHeightPoint);
            }
        }

        private void PaintData(DrawingContext dc, DataChannel channel, Pen pen, StreamGeometry geometry)
        {
            Data[] datas = channel.Datas;
            int index = channel.CurrentIndex;
            Point[] paintPoints = channel.Points;

            if(!channel.CalcuMaxHeight)
            {
                for (int i = 0; i < this._cw; i++)
                {
                    Data data = datas[(index + i) % this._dataCount];
                    paintPoints[i].X = this._cx + i;
                    paintPoints[i].Y = (int)((data.Total <= 0 ? this._cy + this._ch : this._cy + this._ch * (1D - data.Current / data.Total)) + 0.5);
                }
            } else if (this._maxHeight <= 0) {
                for (int i = 0; i < this._cw; i++)
                {
                    paintPoints[i].X = this._cx + i;
                    paintPoints[i].Y = this._cy + this._ch;
                }
            } else {
                for (int i = 0; i < this._cw; i++)
                {
                    Data data = datas[(index + i) % this._dataCount];
                    paintPoints[i].X = this._cx + i;
                    paintPoints[i].Y = (int)(this._cy + this._ch * (1D - data.Current / this._maxHeight) + 0.5);
                }
            }

            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(paintPoints[0], isFilled: false, isClosed: false);
                ctx.PolyLineTo(new ArraySegment<Point>(paintPoints, 1, paintPoints.Length - 1), isStroked: true, isSmoothJoin: false);
            }
            dc.DrawGeometry(null, pen, geometry);
        }

        public void PushData(DateTime now, double total, double current, double percent, object[] param)
        {
            int channelId = (int)param[0];
            DataChannel channel = this._channels[channelId];
            Data data = channel.Datas[channel.CurrentIndex];
            double ototal = data.Total;

            data.Update(now, total, current, percent);
            channel.DataSum += total - ototal;
            channel.LatestDdata = data;
            if (channel.CalcuMaxHeight)
            {
                if (total >= this._maxHeight)
                {
                    if(Math.Abs(total - this._maxHeight) > 0.0001d)
                    {
                        this.FormatMaxHeight(total);
                    }
                }
                else if (ototal >= this._maxHeight)
                {
                    double maxHeight = 0;
                    foreach (DataChannel c in this._channels)
                    {
                        foreach (Data d in c.Datas)
                        {
                            if (d.Total > maxHeight)
                            {
                                maxHeight = d.Total;
                            }
                        }
                    }
                    this.FormatMaxHeight(maxHeight);
                }
            }
           
            channel.CurrentIndex++;
            if(channel.CurrentIndex >= this._dataCount)
            {
                channel.CurrentIndex = 0;
                this.DataUpdateEvent.Invoke(this, channel.DataUpdateEventArgs);
                this.ResetDataUpdateEvent.Invoke(this, channel.DataUpdateEventArgs);
            } else
            {
                this.DataUpdateEvent.Invoke(this, channel.DataUpdateEventArgs);
            }
        }

        public void Close()
        {
            foreach(DataChannel channel in this._channels)
            {
                this.ResetDataUpdateEvent.Invoke(this, channel.DataUpdateEventArgs);
            }
        }

        protected void FormatMaxHeight(double maxHeight)
        {
            this._maxHeight = maxHeight;
            for (int i = 0, count = this.ByteUnits.Length; i < count; i++)
            {
                if (maxHeight < 100)
                { 
                    if((maxHeight * 10) % 1 >= 0.5)
                    {
                        maxHeight += 0.1;
                    }
                    this._maxHeightText = String.Format("{0:0.0}", maxHeight) + this.ByteUnits[i];
                    return;
                }
                maxHeight /= 1024;
            }
            this._maxHeightText = "0B";
        }
    }
}
