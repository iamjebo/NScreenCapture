namespace Demo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectSavePath = new System.Windows.Forms.Button();
            this.txtDefaultFileName = new System.Windows.Forms.TextBox();
            this.txtImageSaveInitialDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartCapture = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblColor);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnSelectSavePath);
            this.groupBox1.Controls.Add(this.txtDefaultFileName);
            this.groupBox1.Controls.Add(this.txtImageSaveInitialDirectory);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(454, 246);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "the properity of Capture class";
            // 
            // lblColor
            // 
            this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblColor.Location = new System.Drawing.Point(312, 180);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(41, 31);
            this.lblColor.TabIndex = 17;
            this.lblColor.Click += new System.EventHandler(this.lblColor_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(257, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "set the color of selected rectangle border";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(299, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "set default file name of result image save dialog";
            // 
            // btnSelectSavePath
            // 
            this.btnSelectSavePath.Location = new System.Drawing.Point(369, 61);
            this.btnSelectSavePath.Name = "btnSelectSavePath";
            this.btnSelectSavePath.Size = new System.Drawing.Size(38, 21);
            this.btnSelectSavePath.TabIndex = 12;
            this.btnSelectSavePath.Text = "...";
            this.btnSelectSavePath.UseVisualStyleBackColor = true;
            this.btnSelectSavePath.Click += new System.EventHandler(this.btnSelectSavePath_Click);
            // 
            // txtDefaultFileName
            // 
            this.txtDefaultFileName.Location = new System.Drawing.Point(18, 137);
            this.txtDefaultFileName.Name = "txtDefaultFileName";
            this.txtDefaultFileName.Size = new System.Drawing.Size(335, 21);
            this.txtDefaultFileName.TabIndex = 11;
            // 
            // txtImageSaveInitialDirectory
            // 
            this.txtImageSaveInitialDirectory.Location = new System.Drawing.Point(18, 62);
            this.txtImageSaveInitialDirectory.Name = "txtImageSaveInitialDirectory";
            this.txtImageSaveInitialDirectory.ReadOnly = true;
            this.txtImageSaveInitialDirectory.Size = new System.Drawing.Size(335, 21);
            this.txtImageSaveInitialDirectory.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "set the initial directory of result image save dialog";
            // 
            // btnStartCapture
            // 
            this.btnStartCapture.Location = new System.Drawing.Point(114, 278);
            this.btnStartCapture.Name = "btnStartCapture";
            this.btnStartCapture.Size = new System.Drawing.Size(251, 33);
            this.btnStartCapture.TabIndex = 0;
            this.btnStartCapture.Text = "Start Capture";
            this.btnStartCapture.UseVisualStyleBackColor = true;
            this.btnStartCapture.Click += new System.EventHandler(this.btnStartCapture_Click);
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 328);
            this.Controls.Add(this.btnStartCapture);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DemoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "The demo of NScreenCapture";
            this.Load += new System.EventHandler(this.DemoForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDefaultFileName;
        private System.Windows.Forms.TextBox txtImageSaveInitialDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartCapture;
        private System.Windows.Forms.Button btnSelectSavePath;
        private System.Windows.Forms.Label lblColor;

    }
}