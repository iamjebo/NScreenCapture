using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Demo
{
    /****************************************************************
    * 
    *             Dcrp：NScreenCapture 截图类库演示
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com
    *           Update: 2013-12-2
    *
    *****************************************************************/


    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();
        }

        private void DemoForm_Load(object sender, EventArgs e)
        {
            // get the default directory
            txtImageSaveInitialDirectory.Text = NScreenCapture.Capture.ImageSaveInitialDirectory;

            // get the default file name
            txtDefaultFileName.Text = NScreenCapture.Capture.ImageSaveFilename;

            // get the default color of selected rectangle border
            lblColor.BackColor = NScreenCapture.Capture.LineColor;
        }

        private void btnSelectSavePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtImageSaveInitialDirectory.Text = folderDialog.SelectedPath;
            }
        }

        private void lblColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lblColor.BackColor = colorDialog.Color;
            }
        }

        private void btnStartCapture_Click(object sender, EventArgs e)
        {
            // set properities
            NScreenCapture.Capture.ImageSaveInitialDirectory = txtImageSaveInitialDirectory.Text;
            NScreenCapture.Capture.ImageSaveFilename = txtDefaultFileName.Text;
            NScreenCapture.Capture.LineColor = lblColor.BackColor;

            NScreenCapture.Capture.BeginCaputre();
        }
    }
}
