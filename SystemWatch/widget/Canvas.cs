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
            public Data[] LatestDatas{private set;get;}

            public CanvasRefreshLatestDataEventArgs(Data[] latestDatas){
                this.LatestDatas = latestDatas;
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

        private Point location;
        private Size clientSize;
        private int dataChannel;
        private int dataCount;
        private Data[][] dataQueues;
        private int[] dataIndexs;
        private Color[] colorChannels;
        private bool[] autoHeightChannels;
        private Data[] latestDatas;
        private Point[] paintPoints;
        private Pen[] paintPens;

        private int cx, cy, cw, ch;
        private float ix, iy;
        private double maxHeight;

        public event EventHandler<CanvasRefreshLatestDataEventArgs> RefreshLatestDataEvent;

        public Canvas(Point location, Size clientSize, int dataChannel, int dataCount, Color[] colorChannels, bool[] autoHeightChannels = null)
        {
            this.location = location;
            this.clientSize = clientSize;
            this.dataChannel = dataChannel;
            this.dataCount = dataCount;
            this.colorChannels = colorChannels;
            this.autoHeightChannels = autoHeightChannels;

            this.Init();
        }

        private void Init()
        {
            this.dataQueues = new Data[this.dataChannel][];
            this.dataIndexs = new int[this.dataChannel];
            this.latestDatas = new Data[this.dataChannel];
            this.maxHeight = 0;

            for (int i = 0; i < this.dataChannel; i++)
            {
                this.dataQueues[i] = new Data[this.dataCount];
                this.dataIndexs[i] = 0;
                for (int j = 0; j < this.dataCount; j++){
                
                    this.dataQueues[i][j] = new Data(0,0,0);
                }
                this.latestDatas[i] = this.dataQueues[i][0];
            }

            this.cx = this.location.X + 1;
            this.cy = this.location.Y + 1;
            this.cw = this.clientSize.Width - 2;
            this.ch = this.clientSize.Height - 2;
            this.ix = (float)this.cw / (float)this.dataCount;
            this.iy = (float)this.ch / 100F;

            this.paintPoints = new Point[this.cw];
            this.paintPens = new Pen[this.colorChannels.Length];

            for (int i = 0; i < this.cw; i++)
            {

                this.paintPoints[i] = new Point(0, 0);
            }

            for (int i = 0; i < this.colorChannels.Length; i++)
            {
                this.paintPens[i] = new Pen(this.colorChannels[i], 1.5F);
            }
        }

        public void BackgroundPaint(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(177, 177, 177)), this.cx -1, this.cy - 1, this.cw + 1, this.ch + 1);

            Pen pen = new Pen(Color.FromArgb(150,150,150),1F);
            for(int i=1, count = this.cw / 10; i <= count; i++){
                g.DrawLine(pen, this.cx + i*10, this.cy, this.cx + i*10, this.cy + this.ch);
            }

            for (int i = 1, count = this.ch / 10; i <=count; i++)
            {
                g.DrawLine(pen, this.cx, this.cy + i * 10, this.cx + this.cw , this.cy + i * 10);
            }
        }

        public void Paint(Graphics g)
        {
            for (int i = 0; i < this.dataChannel; i++)
            {
                this.PaintData(g, i);
            }
        }

        private void PaintData(Graphics g, int channel)
        {
            Data[] datas = this.dataQueues[channel];
            int index = this.dataIndexs[channel];

            if(!(this.autoHeightChannels == null || this.autoHeightChannels[channel]))
            {
                for (int i = 0; i < this.cw; i++)
                {
                    Data data = datas[(index + (int)(i / this.ix)) % this.dataCount];
                    this.paintPoints[i].X = this.cx + i;
                    this.paintPoints[i].Y = data.total <= 0 ? this.cy + this.ch : this.cy + this.ch - (int)(this.ch * data.current / data.total);
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
                for (int i = 0; i < this.cw; i++)
                {
                    Data data = datas[(index + (int)(i / this.ix)) % this.dataCount];
                    this.paintPoints[i].X = this.cx + i;
                    this.paintPoints[i].Y = this.cy + this.ch - (int)(this.ch * ((data.total / this.maxHeight * data.current) / this.maxHeight));
                }
            }
            g.DrawLines(this.paintPens[channel % this.paintPens.Length], this.paintPoints);
        }

        public void PushData(double total, double current, double percent, object[] param)
        {
            int channel = (int)param[0] - 1;
            Data data = this.dataQueues[channel][this.dataIndexs[channel]];

            if(this.autoHeightChannels == null || this.autoHeightChannels[channel])
            {
                if(data.total >= this.maxHeight)
                {
                    double maxHeight = 0;
                    foreach (Data[] q in this.dataQueues)
                    {
                        foreach (Data d in q)
                        {
                            if(d == data)
                            {
                                continue;
                            }

                            if (d.total > maxHeight)
                            {
                                maxHeight = d.total;
                            }
                        }
                    }
                    this.maxHeight = maxHeight;
                }

                if (total > this.maxHeight)
                {
                    this.maxHeight = total;
                }
            }
           
            data.Update(total, current, percent);
            this.latestDatas[channel] = data;
            this.dataIndexs[channel]++;
            if(this.dataIndexs[channel] >= this.dataCount)
            {
                this.dataIndexs[channel] = 0;
            }

            this.RefreshLatestDataEvent(this, new CanvasRefreshLatestDataEventArgs(this.latestDatas));
        }

        public void Close()
        {
        }
    }
}
