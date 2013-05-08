namespace ScreenShot
{
    partial class MainForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStartShot = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartShot
            // 
            this.btnStartShot.Location = new System.Drawing.Point(82, 60);
            this.btnStartShot.Name = "btnStartShot";
            this.btnStartShot.Size = new System.Drawing.Size(75, 23);
            this.btnStartShot.TabIndex = 0;
            this.btnStartShot.Text = "开始截图";
            this.btnStartShot.UseVisualStyleBackColor = true;
            this.btnStartShot.Click += new System.EventHandler(this.btnStartShot_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 144);
            this.Controls.Add(this.btnStartShot);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "截图演示Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartShot;
    }
}

