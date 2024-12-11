using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

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
            public PointsDrawOperation PointsDrawOperation;
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
                this.PointsDrawOperation = new PointsDrawOperation()
                {
                    PaintPen = new ImmutablePen(new ImmutableSolidColorBrush(this.PaintColor), 1.5F),
                    Paint = new SKPaint()
                    {
                        Color = new SKColor(this.PaintColor.ToUInt32()),
                        StrokeWidth = 1.5f,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke
                    },
                    Points = new SKPoint[paintCount],
                    Bounds = clientBounds,
                };
                for (int i = 0; i < paintCount; i++)
                {
                    this.PointsDrawOperation.Points[i] = new SKPoint(0, 0);
                }
                this.DataUpdateEventArgs = new DataUpdateEventArgs(this);
            }
        }

        private readonly Point _location;
        private readonly Size _clientSize;
        private readonly DataChannel[] _channels;
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
            
            foreach (DataChannel channel in this._channels)
            {
                channel.Init(this._dataCount, this._cw, new Rect(this._clientSize));
            }
        }

        public void BackgroundPaint(DrawingContext dc)
        {
            dc.FillRectangle(new SolidColorBrush(Color.FromRgb(175, 175, 175)), 
                new Rect(this._cx -1, this._cy - 1, this._cw + 1, this._ch + 1));
            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(155, 155, 155)), 1F);
            for(int i=1, count = this._cw / 10; i < count; i++){
                dc.DrawLine(pen, new(this._cx + i * 10, this._cy), new(this._cx + i * 10, this._cy + this._ch));
            }
            for (int i = 1, count = this._ch / 10; i < count; i++)
            {
                dc.DrawLine(pen, new(this._cx, this._cy + i * 10), new(this._cx + this._cw , this._cy + i * 10));
            }
        }

        public void Paint(DrawingContext dc)
        {
            foreach(DataChannel channel in this._channels)
            {
                this.PaintData(dc, channel);
            }
            if(this._maxHeight > 0)
            {
                dc.DrawText(new FormattedText(this._maxHeightText, this._cultureInfo, FlowDirection.LeftToRight, this._maxHeightFont, 
                        7F, this._maxHeightBrush), this._maxHeightPoint);
            }
        }

        private void PaintData(DrawingContext dc, DataChannel channel)
        {
            Data[] datas = channel.Datas;
            int index = channel.CurrentIndex;
            SKPoint[] paintPoints = channel.PointsDrawOperation.Points;

            if(!channel.CalcuMaxHeight)
            {
                double y = 0;
                for (int i = 0; i < this._cw; i++)
                {
                    //Data data = datas[(index + (int)Math.Round((float)i / this.ix, 0, MidpointRounding.AwayFromZero)) % this.dataCount];
                    Data data = datas[(index + i) % this._dataCount];
                    paintPoints[i].X = this._cx + i;
                    y = data.Total <= 0 ? this._cy + this._ch : this._cy + this._ch * (1D - data.Current / data.Total);
                    paintPoints[i].Y = y % 1 >= 0.5 ? (int)y + 1 : (int)y;
                }
            } else if (this._maxHeight <= 0) {
                for (int i = 0; i < this._cw; i++)
                {
                    paintPoints[i].X = this._cx + i;
                    paintPoints[i].Y = this._cy + this._ch;
                }
            } else {
                double y = 0;
                for (int i = 0; i < this._cw; i++)
                {
                    //Data data = datas[(index + (int)Math.Round((float)i / this.ix, 0, MidpointRounding.AwayFromZero)) % this.dataCount];
                    Data data = datas[(index + i) % this._dataCount];
                    paintPoints[i].X = this._cx + i;
                    y = this._cy + this._ch * (1D - data.Current / this._maxHeight);
                    paintPoints[i].Y = y % 1 >= 0.5 ? (int)y + 1 : (int)y;
                }
            }
            dc.Custom(channel.PointsDrawOperation);
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
                this.DataUpdateEvent(this, channel.DataUpdateEventArgs);
                this.ResetDataUpdateEvent(this, channel.DataUpdateEventArgs);
            } else
            {
                this.DataUpdateEvent(this, channel.DataUpdateEventArgs);
            }
        }

        public void Close()
        {
            foreach(DataChannel channel in this._channels)
            {
                this.ResetDataUpdateEvent(this, channel.DataUpdateEventArgs);
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
        
        public class PointsDrawOperation : ICustomDrawOperation
        {
            public ImmutablePen PaintPen;
            public SKPaint Paint;
            public SKPoint[] Points;
            public Rect Bounds { set; get; }
            
            public bool HitTest(Point p)
            {
                return false;
            }

            public void Render(ImmediateDrawingContext context)
            {
                var skiaFeature = context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature));
                if (skiaFeature != null)
                {
                    using (var lease = ((ISkiaSharpApiLeaseFeature) skiaFeature).Lease())
                    {
                        lease.SkCanvas.DrawPoints(SKPointMode.Polygon, Points, Paint);
                        return;
                    }
                }
                
                SKPoint[] points = this.Points;
                for (int i = 1, count = points.Length; i < count; i++)
                {
                    int j = i - 1;
                    context.DrawLine(this.PaintPen, new Point(points[j].X, points[j].Y), new Point(points[i].X, points[i].Y));
                }
            }
            
            public bool Equals(ICustomDrawOperation? other)
            {
                return this == other;
            }

            public void Dispose()
            {
            }
        }
    }
}
