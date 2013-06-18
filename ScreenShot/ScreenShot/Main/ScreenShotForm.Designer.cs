namespace ScreenShot
{
    partial class ScreenShotForm
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.colorTable = new ColorTable();
            this.colorTableWithWidth = new ColorTableWithWidth();
            this.shotToolBar = new ShotToolBar();
            this.colorTableWithFont = new ColorTableWithFont();
            ((System.ComponentModel.ISupportInitialize)(this.shotToolBar)).BeginInit();
            this.SuspendLayout();
            // 
            // colorTable
            // 
            this.colorTable.IsDrawBorder = false;
            this.colorTable.Location = new System.Drawing.Point(43, 98);
            this.colorTable.Name = "colorTable";
            this.colorTable.Size = new System.Drawing.Size(177, 37);
            this.colorTable.TabIndex = 3;
            this.colorTable.Visible = false;
            // 
            // colorTableWithWidth
            // 
            this.colorTableWithWidth.Location = new System.Drawing.Point(43, 167);
            this.colorTableWithWidth.MinimumSize = new System.Drawing.Size(287, 40);
            this.colorTableWithWidth.Name = "colorTableWithWidth";
            this.colorTableWithWidth.Size = new System.Drawing.Size(287, 40);
            this.colorTableWithWidth.TabIndex = 2;
            this.colorTableWithWidth.Visible = false;
            // 
            // shotToolBarm
            // 
            this.shotToolBar.Location = new System.Drawing.Point(43, 37);
            this.shotToolBar.Name = "shotToolBar";
            this.shotToolBar.Size = new System.Drawing.Size(296, 30);
            this.shotToolBar.TabIndex = 1;
            this.shotToolBar.TabStop = false;
            this.shotToolBar.Visible = false;
            // 
            // colorTableWithFont
            // 
            this.colorTableWithFont.Location = new System.Drawing.Point(43, 222);
            this.colorTableWithFont.MinimumSize = new System.Drawing.Size(287, 40);
            this.colorTableWithFont.Name = "colorTableWithFont";
            this.colorTableWithFont.Size = new System.Drawing.Size(287, 40);
            this.colorTableWithFont.TabIndex = 0;
            this.colorTableWithFont.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 285);
            this.Controls.Add(this.colorTable);
            this.Controls.Add(this.colorTableWithWidth);
            this.Controls.Add(this.shotToolBar);
            this.Controls.Add(this.colorTableWithFont);
            this.Name = "Main";
            this.Text = "NScreenShot";
            ((System.ComponentModel.ISupportInitialize)(this.shotToolBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ColorTableWithFont colorTableWithFont;
        private ShotToolBar shotToolBar;
        private ColorTableWithWidth colorTableWithWidth;
        private ColorTable colorTable;
    }
}