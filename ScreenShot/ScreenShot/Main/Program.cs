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

            CaptureMainForm capture = new CaptureMainForm();
            capture.ImageSaveFilename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            capture.ImageSaveFilename = "Nscreenshot.jpg";

            Application.Run(capture);
        }
    }
}
