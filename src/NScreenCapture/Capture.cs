#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScreenCapture.CaptureForm;
using System.Drawing;

namespace NScreenCapture
{
    /*
     * code use :
     * 
     *       set image default save initial directory .
     *       this properity determine the initial directory of image save dialog
     *       for example :
     *       Capture.ImageSaveInitialDirectory = @"D:\";
     *       
     *       set image default save file name
     *       this properity determine the default file name of image save dialog
     *       for example:
     *       Capture.ImageSaveFilename = "ImageFileName.png";
     *
     *       set color of selected rectangle border
     *       for example:
     *       Capture.LineColor = Color.LawnGreen;
     *
     *       begin to capture screen
     *       Capture.BeginCaputre();
     *       
     *       
     * copyright (c) 2013 曹江波 <caojiangbocn@gmail.com>
     * 
     */


    /// <summary>
    /// 截图操作类
    /// </summary>
    public class Capture
    {
        private static readonly CaptureMainForm captureForm = new CaptureMainForm();

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
            captureForm.ResetCapture();
            captureForm.ResetWindowsList();
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
