using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScreenCapture.CaptureForm;
using System.Drawing;

namespace NScreenCapture
{
    /****************************************************************
    * 
    *             Dcrp：截图操作类，外部调用接口
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com
    *           Update: 2013-09-11
    *
    *****************************************************************/

    /// <summary>
    /// 截图操作类
    /// </summary>
    public class Capture
    {
        private static readonly CaptureMainForm captureForm = new CaptureMainForm();
        private static bool isStartCapture = false;

        private Capture() { }

        /// <summary>截图文件保存的默认目录</summary>
        public static string ImageSaveInitialDirectory
        {
            set { captureForm.ImageSaveInitialDirectory = value; }
            get { return captureForm.ImageSaveInitialDirectory; }
        }

        /// <summary>截图文件名</summary>
        public static string ImageSaveFilename
        {
            set { captureForm.ImageSaveFilename = value; }
            get { return captureForm.ImageSaveFilename; }
        }

        /// <summary>截图过程中选框矩形的颜色</summary>
        public static Color LineColor
        {
            get { return captureForm.LineColor; }
            set { captureForm.LineColor = value; }
        }

        /// <summary>开始截图</summary>
        public static void BeginCaputre()
        {
            if (!isStartCapture)
            {
                captureForm.ResetCapture();
                captureForm.ResetWindowsList();
                captureForm.ShowDialog();
                isStartCapture = true;
            }
        }

        /// <summary>结束截图</summary>
        public static void EndCapture()
        {
            if (isStartCapture && captureForm != null)
            {
                captureForm.Close();
                isStartCapture = false;
            }
        }
    }
}
