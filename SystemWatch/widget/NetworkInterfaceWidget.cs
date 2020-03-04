﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemWatch
{
    public class NetworkInterfaceWidget : Widget
    {
        private double receivedTotalBytes;
        private double sentTotalBytes;

        private Canvas canvasView;
        private string sentReceivedText;
        private string receivedText;
        private string sentText;
        private string totalReceivedText;
        private string totalSentText;

        private Font sentReceivedFont;
        private Font sentOrReceivedFont;
        private Font totalNetFont;

        private Brush sentReceivedBrush;
        private Brush sentBrush;
        private Brush receivedBrush;

        private Point sentReceivedLocation;
        private Point receivedLocation;
        private Point sentLoction;
        private Point totalReceivedLocation;
        private Point totalSentLocation;

        public string SentReceivedText
        {
            get
            {
                return this.sentReceivedText;
            }
        }

        public string ReceivedText
        {
            get
            {
                return this.receivedText;
            }
        }

        public string SentText
        {
            get
            {
                return this.sentText;
            }
        }

        public string TotalReceivedText
        {
            get
            {
                return this.totalReceivedText;
            }
        }

        public string TotalSentText
        {
            get
            {
                return this.totalSentText;
            }
        }

        public NetworkInterfaceWidget(Point location, Size clientSize) : base(location, clientSize)
        {
            this.sentReceivedText = "T: 0B/s";
            this.receivedText = "R: 0B/s";
            this.sentText = "S: 0B/s";
            this.totalReceivedText = "TR: 0B/s";
            this.totalSentText = "TS: 0B/s";

            this.sentReceivedFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.sentOrReceivedFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.totalNetFont = new Font("微软雅黑", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            this.sentReceivedBrush = new SolidBrush(this.NormalColor[0]);
            this.sentBrush = new SolidBrush(this.NormalColor[1]);
            this.receivedBrush = new SolidBrush(this.NormalColor[2]);

            this.sentReceivedLocation = new Point(clientSize.Width / 2 - 8, 7);
            this.receivedLocation = new Point(clientSize.Width / 2 + 6, 28);
            this.sentLoction = new Point(17, 28);
            this.totalReceivedLocation = new Point(clientSize.Width / 2 + 6, 45);
            this.totalSentLocation = new Point(17, 45);

            this.canvasView = new Canvas(new Point(12, 65), new Size(clientSize.Width - 24, clientSize.Height - 78), 3, 120, new Color[] { this.NormalColor[0], this.NormalColor[1], this.NormalColor[1]});
            this.canvasView.RefreshLatestDataEvent += this.UpdateLatestDatas;

            Program.GetInformation().SetDataToView(Performance.DataType.NetworkInterfaceLoadPercent, this.canvasView, "", new object[] { 1 });
            Program.GetInformation().SetDataToView(Performance.DataType.NetworkInterfaceReceivedLoadPercent, this.canvasView, "", new object[] { 2 });
            Program.GetInformation().SetDataToView(Performance.DataType.NetworkInterfaceSentLoadPercent, this.canvasView, "", new object[] { 3 });
        }

        protected override void BackgroundPaint(Graphics g)
        {
            base.BackgroundPaint(g);
            this.PaintTitle(g, "Net");
            this.canvasView.BackgroundPaint(g);
        }

        protected override void Paint(Graphics g)
        {
            base.Paint(g);

            g.DrawString(this.sentReceivedText, this.sentReceivedFont, this.sentReceivedBrush, this.sentReceivedLocation);
            g.DrawString(this.receivedText, this.sentOrReceivedFont, this.receivedBrush, this.receivedLocation);
            g.DrawString(this.sentText, this.sentOrReceivedFont, this.sentBrush, this.sentLoction);
            g.DrawString(this.totalReceivedText, this.totalNetFont, this.receivedBrush, this.totalReceivedLocation);
            g.DrawString(this.totalSentText, this.totalNetFont, this.sentBrush, this.totalSentLocation);
            this.canvasView.Paint(g);
        }

        public override void Close()
        {
            base.Close();
            this.canvasView.Close();
        }

        public override string GetShortNoticce()
        {
            return "Net:" + this.sentReceivedText.Substring(3);
        }

        private void UpdateLatestDatas(object sender, Canvas.CanvasRefreshLatestDataEventArgs e)
        {
            Canvas.Data[] data = e.LatestDatas;
            switch (e.Channel)
            {
                case 1:
                    this.sentReceivedText = "T: " + this.FormatByteSize(5, data[0].current) + "/s";
                    break;
                case 2:
                    this.receivedText = "R: " + this.FormatByteSize(5, data[1].current) + "/s";
                    this.receivedTotalBytes += data[1].current;
                    this.totalReceivedText = "RT: " + this.FormatByteSize(5, this.receivedTotalBytes);
                    break;
                case 3:
                    this.sentText = "S: " + this.FormatByteSize(5, data[2].current) + "/s";
                    this.sentTotalBytes += data[2].current;
                    this.totalSentText = "ST: " + this.FormatByteSize(5, this.sentTotalBytes);
                    break;
            }
        }
    }
}