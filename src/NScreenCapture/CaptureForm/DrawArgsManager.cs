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
using System.Drawing;

namespace NScreenCapture.CaptureForm
{
    /// <summary>
    /// 截图绘制参数管理类
    /// </summary>
    internal class DrawArgsManager
    {
        #region 屏幕透明参数

        /// <summary>遮罩层背景Alpha值</summary>
        public const byte SCREEN_ALPHA = 100;

        #endregion

        #region 鼠标移动自动选框参数

        /// <summary>自动选框时绘制矩形画笔的宽度</summary>
        public const int LINE_WIDTH_AUTO = 4;

        /// <summary>自动选框时绘制矩形画笔的颜色</summary>
        public static Color LINE_COLOR_AUTO = Color.FromArgb(0, 174, 255);

        #endregion

        #region 自定义选区参数

        /// <summary>自定义截图时选区线条宽度</summary>
        public const byte LINE_WIDTH_CUSTOM = 1;

        /// <summary>自定义截图时选区线条结点宽度</summary>
        public const byte LINE_NODE_WIDTH = 3;

        #endregion

        #region 选区左上角大小信息框参数

        /// <summary>选区信息信息框宽度</summary>
        public const byte INFO_SELECTRECT_WIDTH = 115;

        /// <summary>选区信息信息框高度</summary>
        public const byte INFO_SELECTRECT_HEIGHT = 40;

        #endregion

        #region 鼠标移动信息框参数

        /// <summary>信息框背景Alpha值</summary>
        public const byte INFO_MOVING_ALPHA = 150;

        /// <summary>鼠标移动信息框宽度</summary>
        public const byte INFO_MOVING_WIDTH = 130;

        /// <summary>鼠标移动信息框上半部分：放大图像高度</summary>
        public const byte INFO_MOVING_PIC_HEIGHT = 100;

        /// <summary>鼠标移动信息框下半部分：信息字符串高度</summary>
        public const byte INFO_MOVING_STR_HEIGHT = 40;

        /// <summary>放大图像离鼠标点的范围</summary>
        public const byte INFO_MOVING_RANGE = 15;

        #endregion
    }
}
