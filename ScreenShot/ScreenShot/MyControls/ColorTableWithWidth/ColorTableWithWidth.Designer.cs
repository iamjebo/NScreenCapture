namespace ScreenShot
{
    partial class ColorTableWithWidth
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
            this.widthDotBtnMax = new WidthDotButton();
            this.widthDotBtnMeduim = new WidthDotButton();
            this.widthDotBtnMin = new WidthDotButton();
            this.colorTable = new ColorTable();
            this.SuspendLayout();
            // 
            // widthDotBtnMax
            // 
            this.widthDotBtnMax.AutoSize = true;
            this.widthDotBtnMax.BackColor = System.Drawing.Color.Transparent;
            this.widthDotBtnMax.DotWidth = DotWidth.Maximize;
            this.widthDotBtnMax.Location = new System.Drawing.Point(74, 9);
            this.widthDotBtnMax.MinimumSize = new System.Drawing.Size(23, 23);
            this.widthDotBtnMax.Name = "widthDotBtnMax";
            this.widthDotBtnMax.Size = new System.Drawing.Size(23, 23);
            this.widthDotBtnMax.TabIndex = 3;
            this.widthDotBtnMax.UseVisualStyleBackColor = false;
            // 
            // widthDotBtnMeduim
            // 
            this.widthDotBtnMeduim.AutoSize = true;
            this.widthDotBtnMeduim.BackColor = System.Drawing.Color.Transparent;
            this.widthDotBtnMeduim.DotWidth = DotWidth.Medium;
            this.widthDotBtnMeduim.Location = new System.Drawing.Point(41, 9);
            this.widthDotBtnMeduim.MinimumSize = new System.Drawing.Size(23, 23);
            this.widthDotBtnMeduim.Name = "widthDotBtnMeduim";
            this.widthDotBtnMeduim.Size = new System.Drawing.Size(23, 23);
            this.widthDotBtnMeduim.TabIndex = 2;
            this.widthDotBtnMeduim.UseVisualStyleBackColor = false;
            // 
            // widthDotBtnMin
            // 
            this.widthDotBtnMin.AutoSize = true;
            this.widthDotBtnMin.BackColor = System.Drawing.Color.Transparent;
            this.widthDotBtnMin.Checked = true;
            this.widthDotBtnMin.DotWidth = DotWidth.Minimize;
            this.widthDotBtnMin.Location = new System.Drawing.Point(8, 9);
            this.widthDotBtnMin.MinimumSize = new System.Drawing.Size(23, 23);
            this.widthDotBtnMin.Name = "widthDotBtnMin";
            this.widthDotBtnMin.Size = new System.Drawing.Size(23, 23);
            this.widthDotBtnMin.TabIndex = 1;
            this.widthDotBtnMin.TabStop = true;
            this.widthDotBtnMin.UseVisualStyleBackColor = false;
            // 
            // colorTable
            // 
            this.colorTable.Location = new System.Drawing.Point(108, 2);
            this.colorTable.Name = "colorTable";
            this.colorTable.Size = new System.Drawing.Size(177, 37);
            this.colorTable.TabIndex = 0;
            // 
            // ColorTableWithWidth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.widthDotBtnMax);
            this.Controls.Add(this.widthDotBtnMeduim);
            this.Controls.Add(this.widthDotBtnMin);
            this.Controls.Add(this.colorTable);
            this.Name = "ColorTableWithWidth";
            this.Size = new System.Drawing.Size(287, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorTable colorTable;
        private WidthDotButton widthDotBtnMin;
        private WidthDotButton widthDotBtnMeduim;
        private WidthDotButton widthDotBtnMax;
    }
}
