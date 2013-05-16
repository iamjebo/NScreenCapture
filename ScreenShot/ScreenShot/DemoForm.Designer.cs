namespace ScreenShot
{
    partial class DemoForm
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
            this.StartCaptureBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartCaptureBtn
            // 
            this.StartCaptureBtn.Location = new System.Drawing.Point(102, 47);
            this.StartCaptureBtn.Name = "StartCaptureBtn";
            this.StartCaptureBtn.Size = new System.Drawing.Size(75, 23);
            this.StartCaptureBtn.TabIndex = 0;
            this.StartCaptureBtn.Text = "开始截图";
            this.StartCaptureBtn.UseVisualStyleBackColor = true;
            this.StartCaptureBtn.Click += new System.EventHandler(this.StartCaptureBtn_Click);
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 116);
            this.Controls.Add(this.StartCaptureBtn);
            this.Name = "DemoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DemoForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartCaptureBtn;
    }
}