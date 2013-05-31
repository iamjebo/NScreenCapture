using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScreenShot
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

            ScreenShotMain capture = new ScreenShotMain();
            capture.ImageSaveFilename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            capture.ImageSaveFilename = "WrySmile.jpg";

            Application.Run(capture);
        }
    }
}
