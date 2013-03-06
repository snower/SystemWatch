using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SystemWatch
{
    class UserGraphics
    {
        private Graphics graphic;

        public UserGraphics(Graphics graphic)
        {
            this.graphic = graphic;
        }

        public void DrawArcRadius(Pen pen,Int32 x,Int32 y,Int32 w,Int32 h,Int32 radius)
        {
            this.graphic.DrawLine(pen,x+radius,y,x+w-radius,y);
            this.graphic.DrawLine(pen,x,y+radius,x,y+h-radius);
            this.graphic.DrawLine(pen, x + radius, y + h, x + w - radius, y + h);
            this.graphic.DrawLine(pen,x+w,y+radius,x+w,y+h-radius);

            this.graphic.DrawArc(pen, x, y, 2*radius, 2*radius, 180, 90);
            this.graphic.DrawArc(pen, x, y + h - 2*radius, 2 * radius, 2 * radius, 90, 90);
            this.graphic.DrawArc(pen, x + w -2* radius, y , 2 * radius, 2 * radius, 270, 90);
            this.graphic.DrawArc(pen, x + w - 2*radius, y + h - 2*radius, 2 * radius, 2 * radius, 0, 90);
        }

        public void FillArcRadius(Brush brush, Int32 x, Int32 y, Int32 w, Int32 h, Int32 radius)
        {
            this.graphic.FillEllipse(brush, x, y, 2 * radius, 2 * radius);
            this.graphic.FillEllipse(brush, x, y+h-2*radius, 2 * radius, 2 * radius);
            this.graphic.FillEllipse(brush, x+w-2*radius, y, 2 * radius, 2 * radius);
            this.graphic.FillEllipse(brush, x + w - 2 * radius, y + h - 2 * radius, 2 * radius, 2 * radius);

            this.graphic.FillRectangle(brush, x + radius, y, w - 2*radius, radius);
            this.graphic.FillRectangle(brush, x, y+radius, radius, h-2*radius);
            this.graphic.FillRectangle(brush, x +w- radius, y+radius, radius, h-2*radius);
            this.graphic.FillRectangle(brush, x + radius, y+h-radius, w - 2*radius, radius);

            this.graphic.FillRectangle(brush, x + radius, y+radius, w - 2*radius, h-2*radius);
        }
    }
}
