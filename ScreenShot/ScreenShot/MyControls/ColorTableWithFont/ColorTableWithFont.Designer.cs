namespace ScreenShot
{
    partial class ColorTableWithFont
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorTableWithFont));
            this.colorTable = new ColorTable();
            this.pictureBoxFont = new System.Windows.Forms.PictureBox();
            this.comboBoxFontWidth = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFont)).BeginInit();
            this.SuspendLayout();
            // 
            // colorTable
            // 
            this.colorTable.IsDrawBorder = false;
            this.colorTable.Location = new System.Drawing.Point(108, 2);
            this.colorTable.Name = "colorTable";
            this.colorTable.Size = new System.Drawing.Size(177, 37);
            this.colorTable.TabIndex = 0;
            // 
            // pictureBoxFont
            // 
            this.pictureBoxFont.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxFont.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxFont.Image")));
            this.pictureBoxFont.Location = new System.Drawing.Point(7, 10);
            this.pictureBoxFont.Name = "pictureBoxFont";
            this.pictureBoxFont.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxFont.TabIndex = 1;
            this.pictureBoxFont.TabStop = false;
            // 
            // comboBoxFontWidth
            // 
            this.comboBoxFontWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFontWidth.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBoxFontWidth.FormattingEnabled = true;
            this.comboBoxFontWidth.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "24",
            "36"});
            this.comboBoxFontWidth.Location = new System.Drawing.Point(36, 8);
            this.comboBoxFontWidth.Name = "comboBoxFontWidth";
            this.comboBoxFontWidth.Size = new System.Drawing.Size(54, 25);
            this.comboBoxFontWidth.TabIndex = 2;
            // 
            // ColorTableWithFont
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBoxFontWidth);
            this.Controls.Add(this.pictureBoxFont);
            this.Controls.Add(this.colorTable);
            this.Name = "ColorTableWithFont";
            this.Size = new System.Drawing.Size(287, 40);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFont)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ColorTable colorTable;
        private System.Windows.Forms.PictureBox pictureBoxFont;
        private System.Windows.Forms.ComboBox comboBoxFontWidth;
    }
}
