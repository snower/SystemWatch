using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace SystemWatch
{
    public class Widget
    {
        private readonly Point _location;
        private readonly Size _clientSize;
        private readonly Rect _clientRect;
        private readonly Typeface _titleFont;
        private RenderTargetBitmap? _backgroundCache;
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
            dc.FillRectangle(Brushes.Black, new Rect(0, 0, this._clientSize.Width - 2, this._clientSize.Height - 1), 5);
            dc.DrawRectangle(new Pen(Brushes.Gray), new Rect(2, 2, this._clientSize.Width - 6, this._clientSize.Height - 5), 5);
        }

        protected virtual void Paint(DrawingContext dc)
        {

        }

        public virtual void Render(DrawingContext context)
        {
            if (_backgroundCache == null)
            {
                _backgroundCache = new RenderTargetBitmap(new PixelSize((int)_clientSize.Width, (int)_clientSize.Height), new Vector(96, 96));
                using (var backgroundContext = _backgroundCache.CreateDrawingContext(false))
                {
                    this.BackgroundPaint(backgroundContext);
                }
            }
            using  (context.PushTransform(Matrix.CreateTranslation(_location.X, _location.Y))) {
                context.DrawImage(_backgroundCache, _clientRect, _clientRect);
                this.Paint(context);
            }
        }

        public virtual void Close()
        {
            _backgroundCache?.Dispose();
            _backgroundCache = null;
        }
        
        public virtual string GetShortNoticce()
        {
            return "";
        }

        protected void PaintTitle(DrawingContext dc, string title)
        {

            dc.DrawText(new FormattedText(title, this.CultureInfo, FlowDirection.LeftToRight, _titleFont, 
                    12F, Brushes.White),
                new Point(12, 6));
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
