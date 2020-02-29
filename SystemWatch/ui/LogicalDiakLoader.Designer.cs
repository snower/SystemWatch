using System.Drawing;

namespace SystemWatch
{
    partial class LogicalDiakLoader
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
            this.diskLoadLable = new System.Windows.Forms.Label();
            this.diskReadLoadLable = new System.Windows.Forms.Label();
            this.diskWriteLoadLable = new System.Windows.Forms.Label();
            this.diskReadTotalLable = new System.Windows.Forms.Label();
            this.diskWriteTotalLable = new System.Windows.Forms.Label();
            this.drawImage = new SystemWatch.Canvas(this, 3, 120, 1000);
            this.SuspendLayout();
            // 
            // diskLoadLable
            // 
            this.diskLoadLable.AutoSize = true;
            this.diskLoadLable.BackColor = System.Drawing.Color.Black;
            this.diskLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.diskLoadLable.Location = new System.Drawing.Point(80, 30);
            this.diskLoadLable.Name = "diskLoadLable";
            this.diskLoadLable.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // diskReadLoadLable
            // 
            this.diskReadLoadLable.AutoSize = true;
            this.diskReadLoadLable.BackColor = System.Drawing.Color.Black;
            this.diskReadLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.diskReadLoadLable.Location = new System.Drawing.Point(75, 47);
            this.diskReadLoadLable.Name = "diskReadLoadLable";
            this.diskReadLoadLable.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // diskWriteLoadLable
            // 
            this.diskWriteLoadLable.AutoSize = true;
            this.diskWriteLoadLable.BackColor = System.Drawing.Color.Black;
            this.diskWriteLoadLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.diskWriteLoadLable.Location = new System.Drawing.Point(10, 47);
            this.diskWriteLoadLable.Name = "diskWriteLoadLable";
            this.diskWriteLoadLable.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // diskReadTotalLable
            // 
            this.diskReadTotalLable.AutoSize = true;
            this.diskReadTotalLable.BackColor = System.Drawing.Color.Black;
            this.diskReadTotalLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.diskReadTotalLable.Location = new System.Drawing.Point(75, 65);
            this.diskReadTotalLable.Name = "diskReadTotalLable";
            this.diskReadTotalLable.Font = new System.Drawing.Font("微软雅黑", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // diskWriteTotalLable
            // 
            this.diskWriteTotalLable.AutoSize = true;
            this.diskWriteTotalLable.BackColor = System.Drawing.Color.Black;
            this.diskWriteTotalLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.diskWriteTotalLable.Location = new System.Drawing.Point(10, 65);
            this.diskWriteTotalLable.Name = "diskWriteTotalLable";
            this.diskWriteTotalLable.Font = new System.Drawing.Font("微软雅黑", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // drawImage
            // 
            this.drawImage.BackColor = System.Drawing.Color.Gray;
            this.drawImage.Location = new System.Drawing.Point(10, 85);
            this.drawImage.Name = "drawImage";
            this.drawImage.Size = new System.Drawing.Size(126, 40);
            this.drawImage.RefreshLatestDataEvent += this.Window_RefrshLabel;
            // 
            // LogicalDiakLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(146, 135);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Rectangle screenArea = System.Windows.Forms.Screen.GetBounds(this);
            this.Location = new System.Drawing.Point(screenArea.Width - 180, 225);
            this.Controls.Add(this.diskLoadLable);
            this.Controls.Add(this.diskReadLoadLable);
            this.Controls.Add(this.diskWriteLoadLable);
            this.Controls.Add(this.diskReadTotalLable);
            this.Controls.Add(this.diskWriteTotalLable);
            this.Controls.Add(this.drawImage);
            this.Name = "LogicalDiakLoader";
            this.Text = "Disk";
            this.ResumeLayout(false);

        }
        private Canvas drawImage;
        private System.Windows.Forms.Label diskLoadLable;
        private System.Windows.Forms.Label diskReadLoadLable;
        private System.Windows.Forms.Label diskWriteLoadLable;
        private System.Windows.Forms.Label diskReadTotalLable;
        private System.Windows.Forms.Label diskWriteTotalLable;
        #endregion

    }
}