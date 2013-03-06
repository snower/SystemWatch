namespace SystemWatch
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyMenu = new System.Windows.Forms.ContextMenu();
            this.notifyMenuWindow = new System.Windows.Forms.MenuItem();
            this.notifyMenuExit = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenu = this.notifyMenu;
            this.notifyIcon.Text = "SystemWatch";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // notifyMenu
            // 
            this.notifyMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.notifyMenuWindow,
            this.notifyMenuExit});
            // 
            // notifyMenuWindow
            // 
            this.notifyMenuWindow.Index = 0;
            this.notifyMenuWindow.Text = "&Window";
            // 
            // notifyMenuExit
            // 
            this.notifyMenuExit.Index = 1;
            this.notifyMenuExit.Text = "&Exit";
            this.notifyMenuExit.Click += new System.EventHandler(this.notifyMenuExit_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu notifyMenu;
        private System.Windows.Forms.MenuItem notifyMenuExit;
        private System.Windows.Forms.MenuItem notifyMenuWindow;
        #endregion

    }
}