using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using NScreenCapture;

namespace NScreenCapture
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Capture.LineColor = Color.Red;
            //Capture.ImageSaveInitialDirectory = @"d:\";
            //Capture.ImageSaveFilename = "default.png";
            Capture.BeginCaputre();
        }
    }
}

