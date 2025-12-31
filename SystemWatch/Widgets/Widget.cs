using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace SystemWatch.Widgets
{
    public class Widget
    {
        private readonly Point _location;
        private readonly Size _clientSize;
        private readonly Rect _clientRect;
        private readonly Typeface _titleFont;
        protected readonly CultureInfo CultureInfo;

        protected readonly string[] ByteUnits = new String[] { "B", "K", "M", "G", "T", "P", "E" };

        protected readonly Color[] NormalColor = new Color[]{
            Color.FromRgb(0,255,255),
            Color.FromRgb(0,255,0),
            Color.FromRgb(255,255,133),
            Color.FromRgb(192,255,255)
        };

        public Widget(Point location, Size clientSize)
        {
            this._location = location;
            this._clientSize = clientSize;
            this._clientRect = new Rect(clientSize);
            this._titleFont = new Typeface("微软雅黑");
            this.CultureInfo = CultureInfo.CurrentCulture;
        }

        protected virtual void BackgroundPaint(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Black, new Pen(Brushes.Gray, 1), 
                new Rect(0, 0, this._clientSize.Width - 2, this._clientSize.Height - 1));
        }

        protected virtual void Paint(DrawingContext dc)
        {

        }

        public virtual void Render(DrawingContext context)
        {
            context.PushTransform(new TranslateTransform(_location.X, _location.Y));
            this.BackgroundPaint(context);
            this.Paint(context);
            context.Pop();
        }

        public virtual void Close()
        {
        }
        
        public virtual string GetShortNoticce()
        {
            return "";
        }

        protected void PaintTitle(DrawingContext dc, string title)
        {
            var formattedText = new FormattedText(title, this.CultureInfo, FlowDirection.LeftToRight, 
                this._titleFont, 12, Brushes.White, 1.25);
            dc.DrawText(formattedText, new Point(12, 6));
        }

        protected String FormatByteSize(int len, double value, int type = 0)
        {
            if (value == 0)
            {
                string zeroResult = "";
                for (int i = 0; i < len - 3; i++)
                {
                    zeroResult += " ";
                }
                return zeroResult + "0.0B";
            }

            for (int i = type, count = this.ByteUnits.Length; i < count; i++)
            {
                if (value < 1024)
                {
                    string result = Convert.ToString(value);
                    if(len <= result.Length)
                    {
                        return result.Substring(0, len) + this.ByteUnits[i];
                    }
                    
                    for(int j = 0; j < len - result.Length; j++)
                    {
                        result += "0";
                    }
                    return result + this.ByteUnits[i];
                }
                value /= 1024;
            }

            string defaultResult = "";
            for (int i = 0; i < len - 3; i++)
            {
                defaultResult += " ";
            }
            return defaultResult + "0.0B";
        }
    }
}
