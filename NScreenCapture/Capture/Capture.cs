using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreenCapture
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
        private static CaptureMainForm captureForm = new CaptureMainForm();

        /// <summary>截图保存的默认目录</summary>
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

        /// <summary>开始截图</summary>
        public static void BeginCaputre()
        {
            captureForm.ResetCapture();
            captureForm.ShowDialog();
        }

        /// <summary>结束截图</summary>
        public static void EndCapture()
        {
            if (captureForm != null)
            {
                captureForm.Close();
            }
        }
    }
}
