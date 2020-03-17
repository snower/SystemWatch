namespace SystemWatch.ui
{
    partial class StatisticsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsWindow));
            this.todayDiskChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.todayNetworkChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.todayCpuMemChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cpuChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.memChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.diskChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.networkChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.todayDiskChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.todayNetworkChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.todayCpuMemChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diskChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.networkChart)).BeginInit();
            this.SuspendLayout();
            // 
            // todayDiskChart
            // 
            chartArea1.Name = "ChartArea1";
            this.todayDiskChart.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            legend2.Enabled = false;
            legend2.Name = "Legend2";
            this.todayDiskChart.Legends.Add(legend1);
            this.todayDiskChart.Legends.Add(legend2);
            this.todayDiskChart.Location = new System.Drawing.Point(480, 0);
            this.todayDiskChart.Name = "todayDiskChart";
            this.todayDiskChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.IsValueShownAsLabel = true;
            series1.Label = "#VALX: #VALMB";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.todayDiskChart.Series.Add(series1);
            this.todayDiskChart.Size = new System.Drawing.Size(240, 240);
            this.todayDiskChart.TabIndex = 0;
            this.todayDiskChart.Text = "chart1";
            // 
            // todayNetworkChart
            // 
            chartArea2.Name = "ChartArea1";
            this.todayNetworkChart.ChartAreas.Add(chartArea2);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.todayNetworkChart.Legends.Add(legend3);
            this.todayNetworkChart.Location = new System.Drawing.Point(720, 0);
            this.todayNetworkChart.Name = "todayNetworkChart";
            this.todayNetworkChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series2.IsValueShownAsLabel = true;
            series2.Label = "#VALX:#VALMB";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.todayNetworkChart.Series.Add(series2);
            this.todayNetworkChart.Size = new System.Drawing.Size(240, 240);
            this.todayNetworkChart.TabIndex = 1;
            this.todayNetworkChart.Text = "chart2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(550, 245);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "今日磁盘读写";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(803, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "今日网络流量";
            // 
            // todayCpuMemChart
            // 
            chartArea3.Name = "ChartArea1";
            this.todayCpuMemChart.ChartAreas.Add(chartArea3);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.todayCpuMemChart.Legends.Add(legend4);
            this.todayCpuMemChart.Location = new System.Drawing.Point(0, 60);
            this.todayCpuMemChart.Name = "todayCpuMemChart";
            this.todayCpuMemChart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.Cyan,
        System.Drawing.Color.Lime};
            series3.BorderWidth = 4;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            series4.BorderWidth = 4;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Legend = "Legend1";
            series4.Name = "Series2";
            series4.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.todayCpuMemChart.Series.Add(series3);
            this.todayCpuMemChart.Series.Add(series4);
            this.todayCpuMemChart.Size = new System.Drawing.Size(480, 170);
            this.todayCpuMemChart.TabIndex = 4;
            this.todayCpuMemChart.Text = "chart3";
            // 
            // timeComboBox
            // 
            this.timeComboBox.FormattingEnabled = true;
            this.timeComboBox.Items.AddRange(new object[] {
            "分钟",
            "小时",
            "每天"});
            this.timeComboBox.Location = new System.Drawing.Point(47, 280);
            this.timeComboBox.Name = "timeComboBox";
            this.timeComboBox.Size = new System.Drawing.Size(121, 23);
            this.timeComboBox.TabIndex = 5;
            this.timeComboBox.Text = "分钟";
            this.timeComboBox.SelectedIndexChanged += new System.EventHandler(this.timeComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(166, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "过去1小时CPU内存使用";
            // 
            // cpuChart
            // 
            chartArea4.Name = "ChartArea1";
            this.cpuChart.ChartAreas.Add(chartArea4);
            legend5.Enabled = false;
            legend5.Name = "Legend1";
            this.cpuChart.Legends.Add(legend5);
            this.cpuChart.Location = new System.Drawing.Point(0, 330);
            this.cpuChart.Name = "cpuChart";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.cpuChart.Series.Add(series5);
            this.cpuChart.Size = new System.Drawing.Size(960, 200);
            this.cpuChart.TabIndex = 7;
            this.cpuChart.Text = "chart4";
            // 
            // memChart
            // 
            chartArea5.Name = "ChartArea1";
            this.memChart.ChartAreas.Add(chartArea5);
            legend6.Enabled = false;
            legend6.Name = "Legend1";
            this.memChart.Legends.Add(legend6);
            this.memChart.Location = new System.Drawing.Point(0, 570);
            this.memChart.Name = "memChart";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.memChart.Series.Add(series6);
            this.memChart.Size = new System.Drawing.Size(960, 200);
            this.memChart.TabIndex = 8;
            this.memChart.Text = "chart5";
            // 
            // diskChart
            // 
            chartArea6.Name = "ChartArea1";
            this.diskChart.ChartAreas.Add(chartArea6);
            legend7.Enabled = false;
            legend7.Name = "Legend1";
            this.diskChart.Legends.Add(legend7);
            this.diskChart.Location = new System.Drawing.Point(0, 810);
            this.diskChart.Name = "diskChart";
            series7.BorderWidth = 4;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series7.Legend = "Legend1";
            series7.Name = "Series1";
            series8.BorderWidth = 4;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series8.Legend = "Legend1";
            series8.Name = "Series2";
            this.diskChart.Series.Add(series7);
            this.diskChart.Series.Add(series8);
            this.diskChart.Size = new System.Drawing.Size(960, 200);
            this.diskChart.TabIndex = 9;
            this.diskChart.Text = "chart6";
            // 
            // networkChart
            // 
            chartArea7.Name = "ChartArea1";
            this.networkChart.ChartAreas.Add(chartArea7);
            legend8.Enabled = false;
            legend8.Name = "Legend1";
            this.networkChart.Legends.Add(legend8);
            this.networkChart.Location = new System.Drawing.Point(0, 1050);
            this.networkChart.Name = "networkChart";
            series9.BorderWidth = 4;
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series9.Legend = "Legend1";
            series9.Name = "Series1";
            series10.BorderWidth = 4;
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series10.Legend = "Legend1";
            series10.Name = "Series2";
            this.networkChart.Series.Add(series9);
            this.networkChart.Series.Add(series10);
            this.networkChart.Size = new System.Drawing.Size(960, 200);
            this.networkChart.TabIndex = 10;
            this.networkChart.Text = "chart7";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(462, 535);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "CPU";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(456, 775);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "内存(MB)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(442, 1015);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "磁盘写读(MB)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(435, 1255);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "网络上传下载(MB)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(443, 1302);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 15);
            this.label8.TabIndex = 15;
            // 
            // StatisticsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1003, 593);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.networkChart);
            this.Controls.Add(this.diskChart);
            this.Controls.Add(this.memChart);
            this.Controls.Add(this.cpuChart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.timeComboBox);
            this.Controls.Add(this.todayCpuMemChart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.todayNetworkChart);
            this.Controls.Add(this.todayDiskChart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(480, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StatisticsWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "性能统计";
            ((System.ComponentModel.ISupportInitialize)(this.todayDiskChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.todayNetworkChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.todayCpuMemChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diskChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.networkChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart todayDiskChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart todayNetworkChart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataVisualization.Charting.Chart todayCpuMemChart;
        private System.Windows.Forms.ComboBox timeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataVisualization.Charting.Chart cpuChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart memChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart diskChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart networkChart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}