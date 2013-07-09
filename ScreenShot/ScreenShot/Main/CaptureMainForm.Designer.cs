namespace ScreenShot
{
    partial class CaptureMainForm
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.colorTableWithWidth = new ScreenShot.ColorTableWithWidth();
            this.colorTableWithFont = new ScreenShot.ColorTableWithFont();
            this.textBoxString = new System.Windows.Forms.TextBox();
            this.captureToolBar = new ScreenShot.CaptureToolBar();
            ((System.ComponentModel.ISupportInitialize)(this.captureToolBar)).BeginInit();
            this.SuspendLayout();
            // 
            // colorTableWithWidth
            // 
            this.colorTableWithWidth.Location = new System.Drawing.Point(43, 90);
            this.colorTableWithWidth.MinimumSize = new System.Drawing.Size(287, 40);
            this.colorTableWithWidth.Name = "colorTableWithWidth";
            this.colorTableWithWidth.Size = new System.Drawing.Size(287, 40);
            this.colorTableWithWidth.TabIndex = 2;
            this.colorTableWithWidth.Visible = false;
            // 
            // colorTableWithFont
            // 
            this.colorTableWithFont.Location = new System.Drawing.Point(43, 148);
            this.colorTableWithFont.MinimumSize = new System.Drawing.Size(287, 40);
            this.colorTableWithFont.Name = "colorTableWithFont";
            this.colorTableWithFont.Size = new System.Drawing.Size(287, 40);
            this.colorTableWithFont.TabIndex = 0;
            this.colorTableWithFont.Visible = false;
            // 
            // textBoxString
            // 
            this.textBoxString.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxString.Location = new System.Drawing.Point(84, 218);
            this.textBoxString.Multiline = true;
            this.textBoxString.Name = "textBoxString";
            this.textBoxString.Size = new System.Drawing.Size(100, 21);
            this.textBoxString.TabIndex = 3;
            this.textBoxString.Visible = false;
            // 
            // captureToolBar
            // 
            this.captureToolBar.Location = new System.Drawing.Point(43, 27);
            this.captureToolBar.Name = "captureToolBar";
            this.captureToolBar.Size = new System.Drawing.Size(361, 30);
            this.captureToolBar.TabIndex = 4;
            this.captureToolBar.TabStop = false;
            // 
            // ScreenShotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 285);
            this.Controls.Add(this.captureToolBar);
            this.Controls.Add(this.textBoxString);
            this.Controls.Add(this.colorTableWithWidth);
            this.Controls.Add(this.colorTableWithFont);
            this.Name = "ScreenShotForm";
            this.Text = "NScreenShot";
            ((System.ComponentModel.ISupportInitialize)(this.captureToolBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorTableWithFont colorTableWithFont;
        private ColorTableWithWidth colorTableWithWidth;
        private System.Windows.Forms.TextBox textBoxString;
        private CaptureToolBar captureToolBar;
    }
}