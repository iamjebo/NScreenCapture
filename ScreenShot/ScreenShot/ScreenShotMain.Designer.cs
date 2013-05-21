namespace ScreenShot
{
    partial class ScreenShotMain
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
            this.shotToolBar = new ScreenShot.ShotToolBar();
            ((System.ComponentModel.ISupportInitialize)(this.shotToolBar)).BeginInit();
            this.SuspendLayout();
            // 
            // shotToolBar
            // 
            this.shotToolBar.Location = new System.Drawing.Point(47, 12);
            this.shotToolBar.Name = "shotToolBar";
            this.shotToolBar.Size = new System.Drawing.Size(296, 30);
            this.shotToolBar.TabIndex = 0;
            this.shotToolBar.TabStop = false;
            this.shotToolBar.Visible = false;
            // 
            // ScreenShotMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 262);
            this.Controls.Add(this.shotToolBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "ScreenShotMain";
            this.Text = "ScreenShotMain";
            ((System.ComponentModel.ISupportInitialize)(this.shotToolBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ShotToolBar shotToolBar;
    }
}