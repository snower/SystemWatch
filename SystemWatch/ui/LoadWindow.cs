using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemWatch.ui
{
    public partial class LoadWindow : Form
    {
        public LoadWindow()
        {
            InitializeComponent();

            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 200, 80);
        }

        public Graphics GetGraphics()
        {
            return this.CreateGraphics();
        }
    }
}
