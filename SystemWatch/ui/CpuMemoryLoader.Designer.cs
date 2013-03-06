namespace SystemWatch
{
    partial class CpuMemoryLoader
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

            this.cpuLoadLable = new System.Windows.Forms.Label();
            this.memLoadLable = new System.Windows.Forms.Label();
            this.cpuCoreOneLoadLable = new System.Windows.Forms.Label();
            this.cpuCoreTwoLoadLable = new System.Windows.Forms.Label();
            this.drawImage = new SystemWatch.Canvas(this, 4, 120, 1000);
            this.SuspendLayout();
            // 
            // cpuLoadLable
            // 
            this.cpuLoadLable.AutoSize = true;
            this.cpuLoadLable.BackColor = System.Drawing.Color.Black;
            this.cpuLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.cpuLoadLable.Location = new System.Drawing.Point(75, 27);
            this.cpuLoadLable.Name = "cpuLoadLable";
            this.cpuLoadLable.Font = new System.Drawing.Font("微软雅黑", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // memLoadLable
            // 
            this.memLoadLable.AutoSize = true;
            this.memLoadLable.BackColor = System.Drawing.Color.Black;
            this.memLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.memLoadLable.Location = new System.Drawing.Point(45, 47);
            this.memLoadLable.Name = "memLoadLable";
            this.memLoadLable.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // cpuCoreOneLoadLable
            // 
            this.cpuCoreOneLoadLable.AutoSize = true;
            this.cpuCoreOneLoadLable.BackColor = System.Drawing.Color.Black;
            this.cpuCoreOneLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.cpuCoreOneLoadLable.Location = new System.Drawing.Point(10, 65);
            this.cpuCoreOneLoadLable.Name = "cpuCoreOneLoadLable";
            this.cpuCoreOneLoadLable.Font = new System.Drawing.Font("微软雅黑", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // cpuCoreTwoLoadLable
            // 
            this.cpuCoreTwoLoadLable.AutoSize = true;
            this.cpuCoreTwoLoadLable.BackColor = System.Drawing.Color.Black;
            this.cpuCoreTwoLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.cpuCoreTwoLoadLable.Location = new System.Drawing.Point(70, 65);
            this.cpuCoreTwoLoadLable.Name = "memLoadLable";
            this.cpuCoreTwoLoadLable.Font = new System.Drawing.Font("微软雅黑", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // drawImage
            // 
            this.drawImage.BackColor = System.Drawing.Color.Gray;
            this.drawImage.Location = new System.Drawing.Point(10, 85);
            this.drawImage.Name = "drawImage";
            this.drawImage.Size = new System.Drawing.Size(126, 40);
            this.drawImage.RefreshLatestDataEvent += this.Window_RefrshLabel;
            // 
            // CpuMemoryLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(146, 135);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(1200, 100);
            this.Controls.Add(this.cpuLoadLable);
            this.Controls.Add(this.memLoadLable);
            this.Controls.Add(this.cpuCoreOneLoadLable);
            this.Controls.Add(this.cpuCoreTwoLoadLable);
            this.Controls.Add(this.drawImage);
            this.Name = "CpuMemoryLoader";
            this.Text = "CPU";
            this.ResumeLayout(false);

        }

        private Canvas drawImage;
        private System.Windows.Forms.Label cpuLoadLable;
        private System.Windows.Forms.Label memLoadLable;
        private System.Windows.Forms.Label cpuCoreOneLoadLable;
        private System.Windows.Forms.Label cpuCoreTwoLoadLable;

        #endregion

    }
}