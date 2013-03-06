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
    public partial class Canvas : UserControl,IPushData
    {
        public class CanvasRefreshLatestDataEventArgs : EventArgs
        {
            public Data[] LatestDatas{private set;get;}

            public CanvasRefreshLatestDataEventArgs(Data[] latestDatas){
                this.LatestDatas=latestDatas;
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
        };

        private Form parentWindow;
        private Timer timer;
        private int dataCount,dataChannel,speed;
        private System.Collections.Queue[] dataQueues;
        private Bitmap cache;

        private int cx, cy, cw, ch;
        private float ix, iy;
        private double maxHeight;

        public Color[] colorChannel;
        public bool HeightAuto { set; get; }

        public event EventHandler<CanvasRefreshLatestDataEventArgs> RefreshLatestDataEvent;

        public Canvas()
        {
            this.parentWindow = null;
            this.dataChannel = 1;
            this.dataCount = 120;
            this.speed = 500;

            InitializeComponent();
            this.Init();
        }

        public Canvas(Form parentWindow = null,int dataChannel=1, int dataCount = 120, int speed = 500)
        {
            this.parentWindow = parentWindow;
            this.dataChannel = dataChannel;
            this.dataCount = dataCount;
            this.speed = speed;

            InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            this.HeightAuto = false;
            this.dataQueues = new System.Collections.Queue[this.dataChannel];
            this.colorChannel = new Color[this.dataChannel];
            this.maxHeight = 100;

            for (int i = 0; i < this.dataChannel; i++)
            {
                this.dataQueues[i] = new System.Collections.Queue(this.dataCount);
                for (int j = 0; j < this.dataCount; j++){
                
                    this.dataQueues[i].Enqueue(new Data(0,0,0));
                }
            }

            this.timer = new Timer();
            this.timer.Interval = this.speed;
            this.timer.Tick += new EventHandler(this.TimerEvent);
            this.timer.Start();

            this.cx = 1;
            this.cy = 1;
            this.cw = this.Width - 2;
            this.ch = this.Height - 2;
            this.ix = this.cw / this.dataCount;
            this.iy = (float)this.ch / 100F;

            this.cache = new Bitmap(this.cw, this.ch);
        }

        private void TimerEvent(Object o, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Graphics gc = Graphics.FromImage(this.cache);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.PaintBackGround(gc);
            Data[] latestData = new Data[this.dataChannel];
            object[][] dataArys = this.CalculateMaxTotal();
            for (int i = 0; i < this.dataChannel; i++)
            {
                latestData[i] = this.PaintData(gc, i, dataArys[i]);
            }

            g.DrawImage(this.cache,this.cx,this.cy);
            this.SendRefreshLatestData(latestData);
        }

        private object[][] CalculateMaxTotal()
        {
            Object[][] dataArys = new object[this.dataChannel][];
            this.maxHeight = 0;

            for (int i = 0; i < this.dataChannel; i++)
            {
                dataArys[i] = this.dataQueues[i].ToArray();
                if (!this.HeightAuto) continue;
                for (int j = 0, count = dataArys[i].Length; j < count; j++)
                {
                    Data data=(Data)dataArys[i][j];
                    if (this.maxHeight<data.total)
                    {
                        this.maxHeight = data.total;
                    }
                }
            }
            if (!this.HeightAuto) return dataArys;
            for(int i=0;i<this.dataChannel;i++){
                dataArys[i] = this.CalculatePercent(dataArys[i],this.maxHeight);
            }
            return dataArys;
        }

        private object[] CalculatePercent(Object[] dataAry,double maxTotal)
        {
            for (int i = 0, count = dataAry.Length; i < count; i++)
            {
                Data data = (Data)dataAry[i];
                if (data.total != 0) data.percent = data.current / maxTotal * 100;
                else data.percent = 0;
            }
            return dataAry;
        }

        private void PaintBackGround(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(177, 177, 177)), 0, 0, this.cw, this.ch);
            Pen pen=new Pen(Color.FromArgb(150,150,150),1F);
            for(int i=1,count=this.cw/10;i<=count;i++){
                g.DrawLine(pen, i*10, 0, i*10, this.ch);
            }
            for (int i = 1, count = this.ch / 10; i <=count; i++)
            {
                g.DrawLine(pen, 0, i * 10, this.cw ,i * 10);
            }
        }

        private Data PaintData(Graphics g, int channel,Object[] dataAry)
        {
            Pen pen = new Pen(this.colorChannel[channel], 1.5F);
            Point[] points=new Point[this.cw];
            
            for (int i = 0; i<this.cw ; i++)
            {
                points[i].X = this.cx+i;
                points[i].Y = this.cy+this.ch-(int)(((Data)dataAry[(int)(i/this.ix)]).percent * this.iy);
            }
            g.DrawLines(pen,points);
            return (Data)dataAry[dataAry.Length - 1];
        }

        private void SendRefreshLatestData(Data[] latestData)
        {
            this.RefreshLatestDataEvent(this, new CanvasRefreshLatestDataEventArgs(latestData));
        }

        public Color SetChannelColor(int channel,Color color)
        {
            Color c=this.colorChannel[channel];
            this.colorChannel[channel]=color;
            return c;
        }

        private void Canvas_SizeChanged(object sender, EventArgs e)
        {
            this.cx = 1;
            this.cy = 1;
            this.cw = this.Width - 2;
            this.ch = this.Height - 2;
            this.ix = (float)this.cw / (float)this.dataCount;
            this.iy = (float)this.ch / 100F;

            this.cache = new Bitmap(this.cw, this.ch);
        }

        public void PushData(double total,double current,double percent, object[] param)
        {
            int channel = (int)param[0];
            if (this.dataQueues[channel - 1].Count >= this.dataCount)
            {
                this.dataQueues[channel - 1].Dequeue();
            }
            this.dataQueues[channel - 1].Enqueue(new Data(total,current,percent));
        }

        public void Close()
        {
            this.timer.Stop();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawImage(this.cache, this.cx, this.cy);
        }
    }
}
