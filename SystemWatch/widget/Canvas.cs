using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemWatch
{
    public class Canvas : IPushData
    {
        protected string[] ByteUnits = new String[] { "B", "K", "M", "G", "T", "P", "E" };
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
            public int ChannelID;
            public Data[] Datas;
            public int DataCount;
            public int CurrentIndex;
            public Color PaintColor;
            public Pen PaintPen;
            public bool CalcuMaxHeight;
            public double DataSum;
            public Data LatestDdata;
            public DataUpdateEventArgs DataUpdateEventArgs;

            public DataChannel(int channelId, Color paintColor, bool calcuMaxHeight = true)
            {
                this.ChannelID = channelId;
                this.PaintColor = paintColor;
                this.CalcuMaxHeight = calcuMaxHeight;
            }

            public void Init(int dataCount)
            {
                DateTime now = DateTime.Now;
                this.DataSum = 0;
                this.DataCount = dataCount;
                this.Datas = new Data[this.DataCount];
                for(int i = 0; i < this.DataCount; i++)
                {
                    this.Datas[i] = new Data(now, 0, 0, 0);
                }
                this.CurrentIndex = 0;
                this.LatestDdata = this.Datas[0];
                this.PaintPen = new Pen(this.PaintColor, 1.5F);
                this.DataUpdateEventArgs = new DataUpdateEventArgs(this);
            }
        }

        private Point location;
        private Size clientSize;
        private DataChannel[] channels;
        private int dataCount;
        private Point[] paintPoints;

        private int cx, cy, cw, ch;
        private float ix, iy;
        private double maxHeight;
        private string maxHeightText;
        private Font maxHeightFont;
        private Color maxHeightColor;
        private Brush maxHeightBrush;

        public event EventHandler<DataUpdateEventArgs> DataUpdateEvent;
        public event EventHandler<DataUpdateEventArgs> ResetDataUpdateEvent;

        public Canvas(Point location, Size clientSize, int dataCount, DataChannel[] channels, Color maxHeightColor)
        {
            this.location = location;
            this.clientSize = clientSize;
            this.dataCount = dataCount;
            this.channels = channels;
            this.maxHeightColor = maxHeightColor;

            this.Init();
        }

        private void Init()
        {
            this.maxHeight = 0;

            foreach (DataChannel channel in this.channels)
            {
                channel.Init(this.dataCount);
            }

            this.cx = this.location.X + 1;
            this.cy = this.location.Y + 1;
            this.cw = this.clientSize.Width - 2;
            this.ch = this.clientSize.Height - 2;
            this.ix = (float)this.cw / (float)this.dataCount;
            this.iy = (float)this.ch / 100F;

            this.paintPoints = new Point[this.cw];

            for (int i = 0; i < this.cw; i++)
            {

                this.paintPoints[i] = new Point(0, 0);
            }

            this.maxHeightFont = new Font("微软雅黑", 5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.maxHeightBrush = new SolidBrush(this.maxHeightColor);
        }

        public void BackgroundPaint(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(175, 175, 175)), this.cx -1, this.cy - 1, this.cw + 1, this.ch + 1);

            Pen pen = new Pen(Color.FromArgb(155, 155, 155), 1F);
            for(int i=1, count = this.cw / 10; i < count; i++){
                g.DrawLine(pen, this.cx + i * 10, this.cy, this.cx + i * 10, this.cy + this.ch);
            }

            for (int i = 1, count = this.ch / 10; i < count; i++)
            {
                g.DrawLine(pen, this.cx, this.cy + i * 10, this.cx + this.cw , this.cy + i * 10);
            }
        }

        public void Paint(Graphics g)
        {
            foreach(DataChannel channel in this.channels)
            {
                this.PaintData(g, channel);
            }

            if(this.maxHeight > 0)
            {
                g.DrawString(this.maxHeightText, this.maxHeightFont, this.maxHeightBrush, this.cx, this.cy);
            }
        }

        private void PaintData(Graphics g, DataChannel channel)
        {
            Data[] datas = channel.Datas;
            int index = channel.CurrentIndex;

            if(!channel.CalcuMaxHeight)
            {
                double y;
                for (int i = 0; i < this.cw; i++)
                {
                    //Data data = datas[(index + (int)Math.Round((float)i / this.ix, 0, MidpointRounding.AwayFromZero)) % this.dataCount];
                    Data data = datas[(index + i) % this.dataCount];
                    this.paintPoints[i].X = this.cx + i;
                    y = data.Total <= 0 ? this.cy + this.ch : this.cy + this.ch * (1D - data.Current / data.Total);
                    this.paintPoints[i].Y = y % 1 >= 0.5 ? (int)y + 1 : (int)y;
                }
            }
            else if (this.maxHeight <= 0)
            {
                for (int i = 0; i < this.cw; i++)
                {
                    this.paintPoints[i].X = this.cx + i;
                    this.paintPoints[i].Y = this.cy + this.ch;
                }
            }
            else
            {
                double y;
                for (int i = 0; i < this.cw; i++)
                {
                    //Data data = datas[(index + (int)Math.Round((float)i / this.ix, 0, MidpointRounding.AwayFromZero)) % this.dataCount];
                    Data data = datas[(index + i) % this.dataCount];
                    this.paintPoints[i].X = this.cx + i;
                    y = this.cy + this.ch * (1D - data.Current / this.maxHeight);
                    this.paintPoints[i].Y = y % 1 >= 0.5 ? (int)y + 1 : (int)y;
                }
            }

            g.DrawLines(channel.PaintPen, this.paintPoints);
        }

        public void PushData(DateTime now, double total, double current, double percent, object[] param)
        {
            int channelId = (int)param[0];
            DataChannel channel = this.channels[channelId];
            Data data = channel.Datas[channel.CurrentIndex];
            double ototal = data.Total;

            data.Update(now, total, current, percent);
            channel.DataSum += total - ototal;
            channel.LatestDdata = data;
            if (channel.CalcuMaxHeight)
            {
                if (total >= this.maxHeight)
                {
                    if(total != this.maxHeight)
                    {
                        this.FormatMaxHeight(total);
                    }
                }
                else if (ototal >= this.maxHeight)
                {
                    double maxHeight = 0;
                    foreach (DataChannel c in this.channels)
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
            if(channel.CurrentIndex >= this.dataCount)
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
        }

        protected void FormatMaxHeight(double maxHeight)
        {
            this.maxHeight = maxHeight;
            for (int i = 0, count = this.ByteUnits.Length; i < count; i++)
            {
                if (maxHeight < 100)
                { 
                    if((maxHeight * 10) % 1 >= 0.5)
                    {
                        maxHeight += 0.1;
                    }
                    this.maxHeightText = String.Format("{0:0.0}", maxHeight) + this.ByteUnits[i];
                    return;
                }
                maxHeight /= 1024;
            }
            this.maxHeightText = "0B";
        }
    }
}
