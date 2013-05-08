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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnStartShot_Click(object sender, EventArgs e)
        {
            ScreenShotForm screenForm = new ScreenShotForm();
            screenForm.Show();
        }
    }
}
