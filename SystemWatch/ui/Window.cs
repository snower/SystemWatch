using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace SystemWatch
{
    public partial class Window : Form
    {
        public enum MouseStatus
        {
            Up=0,Down=1
        };

        private Point mouseOffsetPosition;
        private MouseStatus mouseStatus = MouseStatus.Up;
        private Timer timer;
        protected Color[] NormalColor = new Color[]{
            Color.FromArgb(0,255,255),
            Color.FromArgb(0,255,0),
            Color.FromArgb(192,255,32),
            Color.FromArgb(192,255,255)
        };

        public String Text
        {
            get
            {
                return this.titleLable.Text;
            }
            set
            {
                this.titleLable.Text = value;
            }
        }

        public Window()
        {
            this.timer = new Timer();
            InitializeComponent();
        }

        private void Window_Load(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                c.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Window_MouseDown);
                c.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Window_MouseMove);
                c.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Window_MouseUp);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetMouseStatus(MouseStatus status,Point point)
        {
            this.mouseStatus = status;
            if (status == MouseStatus.Up)
            {
                this.mouseOffsetPosition = new Point(0, 0);
            }
            else
            {
                point = this.PointToScreen(point);
                this.mouseOffsetPosition = new Point(point.X - this.Left, point.Y - this.Top);
            }
        }

        public void DropMove(Point point)
        {
            if (this.mouseStatus == MouseStatus.Down)
            {
                if(!this.mouseOffsetPosition.IsEmpty)
                {
                    point = this.PointToScreen(point);
                    this.Location = new Point(point.X - this.mouseOffsetPosition.X, point.Y - this.mouseOffsetPosition.Y);
                }
            }
        }

        protected void Window_MouseDown(object sender, MouseEventArgs e)
        {
            this.SetMouseStatus(MouseStatus.Down,e.Location);
        }

        protected void Window_MouseUp(object sender, MouseEventArgs e)
        {
            this.SetMouseStatus(MouseStatus.Up,e.Location);
        }

        protected void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.DropMove(e.Location);
        }

        protected void Window_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            UserGraphics ug=new UserGraphics(g);
            Pen pen=new Pen(Color.Gray,2);
            Brush brush = new SolidBrush(Color.Black);
            ug.FillArcRadius(brush, 0, 21, this.Width-1, this.Height-22, 5);
            ug.DrawArcRadius(pen, 2, 23, this.Width - 6, this.Height - 27, 5);
        }

        public virtual void Close()
        {
            base.Close();
        }

        protected String FormatByteSize(int len, double value,int type=0)
        {
            string[] units=new String[]{"B","K","M","G"};
            for (int i = type, count = units.Length; i < count; i++)
            {
                if (value < 1024)
                {
                    string result = Convert.ToString(value);
                    len = len > result.Length ? result.Length : len;
                    return result.Substring(0, len) + units[i];
                }
                value /= 1024;
            }
            return "0B";
        }
    }
}
