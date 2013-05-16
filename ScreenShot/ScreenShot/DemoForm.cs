using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenShot
{
    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();
        }

        private void StartCaptureBtn_Click(object sender, EventArgs e)
        {
            using (ScreenShotForm capture = new ScreenShotForm())
            {
                capture.ImageSaveFilename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                capture.ImageSaveFilename = "WrySmile.jpg";
                capture.ShowDialog();
            }
        }
    }
}
