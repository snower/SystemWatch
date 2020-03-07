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
        public class CanvasRefreshLatestDataEventArgs : EventArgs
        {
            public Data[] LatestDatas{ private set; get;}
            public int Channel { private set; get; }

            public CanvasRefreshLatestDataEventArgs(Data[] latestDatas, int channel){
                this.LatestDatas = latestDatas;
                this.Channel = channel;
            }
        };

        public class Data
        {
            public double total;
            public double current;
            public double percent;

            public Data(double total, double current, double percent)
            {
                this.total = total;
                this.current = current;
                this.percent = percent;
            }

            public void Update(double total, double current, double percent)
            {
                this.total = total;
                this.current = current;
                this.percent = percent;
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

            public DataChannel(int channelId, Color paintColor, bool calcuMaxHeight = true)
            {
                this.ChannelID = channelId;
                this.PaintColor = paintColor;
                this.CalcuMaxHeight = calcuMaxHeight;
            }

            public void Init(int dataCount)
            {
                this.DataCount = dataCount;
                this.Datas = new Data[this.DataCount];
                for(int i = 0; i < this.DataCount; i++)
                {
                    this.Datas[i] = new Data(0, 0, 0);
                }
                this.CurrentIndex = 0;
                this.PaintPen = new Pen(this.PaintColor, 1.5F);
            }
        }

        private Point location;
        private Size clientSize;
        private DataChannel[] channels;
        private int dataCount;
        private Data[] latestDatas;
        private Point[] paintPoints;

        private int cx, cy, cw, ch;
        private float ix, iy;
        private double maxHeight;

        public event EventHandler<CanvasRefreshLatestDataEventArgs> RefreshLatestDataEvent;

        public Canvas(Point location, Size clientSize, int dataCount, DataChannel[] channels)
        {
            this.location = location;
            this.clientSize = clientSize;
            this.dataCount = dataCount;
            this.channels = channels;

            this.Init();
        }

        private void Init()
        {
            this.latestDatas = new Data[this.channels.Length];
            this.maxHeight = 0;

            foreach(DataChannel channel in this.channels)
            {
                channel.Init(this.dataCount);
                this.latestDatas[channel.ChannelID] = channel.Datas[0];
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
        }

        public void BackgroundPaint(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(177, 177, 177)), this.cx -1, this.cy - 1, this.cw + 1, this.ch + 1);

            Pen pen = new Pen(Color.FromArgb(150,150,150),1F);
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
                    y = data.total <= 0 ? this.cy + this.ch : this.cy + this.ch * (1D - data.current / data.total);
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
                    y = this.cy + this.ch * (1D - data.current / this.maxHeight);
                    this.paintPoints[i].Y = y % 1 >= 0.5 ? (int)y + 1 : (int)y;
                }
            }

            g.DrawLines(channel.PaintPen, this.paintPoints);
        }

        public void PushData(double total, double current, double percent, object[] param)
        {
            int channelId = (int)param[0];
            DataChannel channel = this.channels[channelId];
            Data data = channel.Datas[channel.CurrentIndex];
            double ototal = data.total;

            data.Update(total, current, percent);
            if (channel.CalcuMaxHeight)
            {
                if (total >= this.maxHeight)
                {
                    this.maxHeight = total;
                }
                else if (ototal >= this.maxHeight)
                {
                    double maxHeight = 0;
                    foreach (DataChannel c in this.channels)
                    {
                        foreach (Data d in c.Datas)
                        {
                            if (d.total > maxHeight)
                            {
                                maxHeight = d.total;
                            }
                        }
                    }
                    this.maxHeight = maxHeight;
                }
            }
           
            this.latestDatas[channelId] = data;
            channel.CurrentIndex++;
            if(channel.CurrentIndex >= this.dataCount)
            {
                channel.CurrentIndex = 0;
            }

            this.RefreshLatestDataEvent(this, new CanvasRefreshLatestDataEventArgs(this.latestDatas, channelId));
        }

        public void Close()
        {
        }
    }
}
