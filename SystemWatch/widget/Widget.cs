using System;
using System.Drawing;
using System.Windows.Forms;

namespace SystemWatch
{
    public class Widget
    {
        private Bitmap backgroudView;
        private Bitmap view;
        private Point location;
        private Size clientSize;

        protected Color[] NormalColor = new Color[]{
            Color.FromArgb(0,255,255),
            Color.FromArgb(0,255,0),
            Color.FromArgb(192,255,32),
            Color.FromArgb(192,255,255)
        };

        public Widget(Point location, Size clientSize)
        {
            this.location = location;
            this.clientSize = clientSize;

            this.backgroudView = new Bitmap(clientSize.Width, clientSize.Height);
            this.view = new Bitmap(clientSize.Width, clientSize.Height);
        }

        protected virtual void BackgroundPaint(Graphics g)
        {
            this.FillArcRadius(g, new SolidBrush(Color.Black), 0, 21, this.clientSize.Width-1, this.clientSize.Height-22, 5);
            this.DrawArcRadius(g, new Pen(Color.Gray, 2), 2, 23, this.clientSize.Width - 6, this.clientSize.Height - 27, 5);
        }

        protected virtual void Paint(Graphics g)
        {

        }

        public virtual void Close()
        {
            this.backgroudView.Clone();
            this.view.Clone();
        }

        public virtual void BackgroundUpdate()
        {
            Graphics g = Graphics.FromImage(this.backgroudView);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.BackgroundPaint(g);
        }

        public virtual void Update()
        {
            Graphics g = Graphics.FromImage(this.view);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawImage(this.backgroudView, new Point(0, 0));
            this.Paint(g);
        }

        public virtual void Show(Graphics g)
        {
            g.DrawImage(this.view, this.location);
        }

        public virtual void Hide()
        {

        }

        public virtual string GetShortNoticce()
        {
            return "";
        }

        protected void DrawArcRadius(Graphics g, Pen pen, Int32 x, Int32 y, Int32 w, Int32 h, Int32 radius)
        {
            g.DrawLine(pen, x + radius, y, x + w - radius, y);
            g.DrawLine(pen, x, y + radius, x, y + h - radius);
            g.DrawLine(pen, x + radius, y + h, x + w - radius, y + h);
            g.DrawLine(pen, x + w, y + radius, x + w, y + h - radius);

            g.DrawArc(pen, x, y, 2 * radius, 2 * radius, 180, 90);
            g.DrawArc(pen, x, y + h - 2 * radius, 2 * radius, 2 * radius, 90, 90);
            g.DrawArc(pen, x + w - 2 * radius, y, 2 * radius, 2 * radius, 270, 90);
            g.DrawArc(pen, x + w - 2 * radius, y + h - 2 * radius, 2 * radius, 2 * radius, 0, 90);
        }

        protected void FillArcRadius(Graphics g, Brush brush, Int32 x, Int32 y, Int32 w, Int32 h, Int32 radius)
        {
            g.FillEllipse(brush, x, y, 2 * radius, 2 * radius);
            g.FillEllipse(brush, x, y + h - 2 * radius, 2 * radius, 2 * radius);
            g.FillEllipse(brush, x + w - 2 * radius, y, 2 * radius, 2 * radius);
            g.FillEllipse(brush, x + w - 2 * radius, y + h - 2 * radius, 2 * radius, 2 * radius);

            g.FillRectangle(brush, x + radius, y, w - 2 * radius, radius);
            g.FillRectangle(brush, x, y + radius, radius, h - 2 * radius);
            g.FillRectangle(brush, x + w - radius, y + radius, radius, h - 2 * radius);
            g.FillRectangle(brush, x + radius, y + h - radius, w - 2 * radius, radius);

            g.FillRectangle(brush, x + radius, y + radius, w - 2 * radius, h - 2 * radius);
        }

        protected void PaintTitle(Graphics g, string title)
        {
            Font font = new Font("微软雅黑", 9.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            Color color = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            g.DrawString(title, font, new SolidBrush(color), new Point(10, 27));
        }

        protected String FormatByteSize(int len, double value, int type=0)
        {
            string[] units=new String[]{"B","K","M","G", "T", "P", "E"};
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
