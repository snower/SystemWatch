using System.Drawing;

namespace SystemWatch
{
    partial class NetworkInterfaceLoader
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
            this.networkInterfaceLoad = new System.Windows.Forms.Label();
            this.networkInterfaceReceivedLoad = new System.Windows.Forms.Label();
            this.networkInterfaceSentLoad = new System.Windows.Forms.Label();
            this.networkInterfaceReceivedTotal = new System.Windows.Forms.Label();
            this.networkInterfaceSentTotal = new System.Windows.Forms.Label();
            this.drawImage = new SystemWatch.Canvas(this, 3, 120, 1000);
            this.SuspendLayout();
            // 
            // networkInterfaceLoad
            // 
            this.networkInterfaceLoad.AutoSize = true;
            this.networkInterfaceLoad.BackColor = System.Drawing.Color.Black;
            this.networkInterfaceLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.networkInterfaceLoad.Location = new System.Drawing.Point(80, 30);
            this.networkInterfaceLoad.Name = "networkInterfaceLoad";
            this.networkInterfaceLoad.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // networkInterfaceReceivedLoad
            // 
            this.networkInterfaceReceivedLoad.AutoSize = true;
            this.networkInterfaceReceivedLoad.BackColor = System.Drawing.Color.Black;
            this.networkInterfaceReceivedLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.networkInterfaceReceivedLoad.Location = new System.Drawing.Point(75, 47);
            this.networkInterfaceReceivedLoad.Name = "networkInterfaceReceivedLoad";
            this.networkInterfaceReceivedLoad.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // networkInterfaceSentLoad
            // 
            this.networkInterfaceSentLoad.AutoSize = true;
            this.networkInterfaceSentLoad.BackColor = System.Drawing.Color.Black;
            this.networkInterfaceSentLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.networkInterfaceSentLoad.Location = new System.Drawing.Point(10, 47);
            this.networkInterfaceSentLoad.Name = "networkInterfaceSentLoad";
            this.networkInterfaceSentLoad.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // networkInterfaceReceivedTotal
            // 
            this.networkInterfaceReceivedTotal.AutoSize = true;
            this.networkInterfaceReceivedTotal.BackColor = System.Drawing.Color.Black;
            this.networkInterfaceReceivedTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.networkInterfaceReceivedTotal.Location = new System.Drawing.Point(75, 65);
            this.networkInterfaceReceivedTotal.Name = "networkInterfaceReceivedTotal";
            this.networkInterfaceReceivedTotal.Font = new System.Drawing.Font("微软雅黑", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // networkInterfaceSentTotal
            // 
            this.networkInterfaceSentTotal.AutoSize = true;
            this.networkInterfaceSentTotal.BackColor = System.Drawing.Color.Black;
            this.networkInterfaceSentTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.networkInterfaceSentTotal.Location = new System.Drawing.Point(10, 65);
            this.networkInterfaceSentTotal.Name = "networkInterfaceSentTotal";
            this.networkInterfaceSentTotal.Font = new System.Drawing.Font("微软雅黑", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // drawImage
            // 
            this.drawImage.BackColor = System.Drawing.Color.Gray;
            this.drawImage.HeightAuto = false;
            this.drawImage.Location = new System.Drawing.Point(10, 85);
            this.drawImage.Name = "drawImage";
            this.drawImage.Size = new System.Drawing.Size(126, 40);
            this.drawImage.RefreshLatestDataEvent += this.Window_RefrshLabel;
            // 
            // NetworkInterfaceLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(146, 135);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Rectangle screenArea = System.Windows.Forms.Screen.GetBounds(this);
            this.Location = new System.Drawing.Point(screenArea.Width - 180, 370);
            this.Controls.Add(this.networkInterfaceLoad);
            this.Controls.Add(this.networkInterfaceReceivedLoad);
            this.Controls.Add(this.networkInterfaceSentLoad);
            this.Controls.Add(this.networkInterfaceReceivedTotal);
            this.Controls.Add(this.networkInterfaceSentTotal);
            this.Controls.Add(this.drawImage);
            this.Name = "NetworkInterfaceLoader";
            this.Text = "Network";
            this.ResumeLayout(false);

        }
        private Canvas drawImage;
        private System.Windows.Forms.Label networkInterfaceLoad;
        private System.Windows.Forms.Label networkInterfaceReceivedLoad;
        private System.Windows.Forms.Label networkInterfaceSentLoad;
        private System.Windows.Forms.Label networkInterfaceReceivedTotal;
        private System.Windows.Forms.Label networkInterfaceSentTotal;
        #endregion

    }
}