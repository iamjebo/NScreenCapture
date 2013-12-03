using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

using NScreenCapture;

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

            // set image default save initial directory .
            // this properity determine the initial directory of image save dialog
            //Capture.ImageSaveInitialDirectory = @"D:\";

            // set image default save file name
            // this properity determine the default file name of image save dialog
            //Capture.ImageSaveFilename = "ImageFileName.png";

            // set color of selected rectangle border 
            //Capture.LineColor = Color.LawnGreen;

            Capture.BeginCaputre();
        }
    }
}
