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

namespace NScreenCapture.Types
{
    /// <summary>
    /// 自定义控件绘制所用的颜色类
    /// </summary>
    internal class MyControlColors
    {
        /// <summary>
        /// 自定义控件边框的颜色
        /// </summary>
        public static readonly Color BORDER_COLOR = Color.FromArgb(200, 78, 153, 210);

        /// <summary>
        /// 自定义控件背景色
        /// </summary>
        public static readonly Color BACKGROUND_COLOR = Color.FromArgb(200, 222, 238, 255);
    }
}
