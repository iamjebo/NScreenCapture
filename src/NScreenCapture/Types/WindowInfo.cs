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

namespace NScreenCapture.Types
{
    /// <summary>
    /// 窗体信息实体类，用于构造窗体列表。
    /// </summary>
    internal class WindowInfo
    {
        private IntPtr hwnd;
        private RECT rect;

        public WindowInfo() { }

        public WindowInfo(IntPtr hwnd, RECT rect)
        {
            this.hwnd = hwnd;
            this.rect = rect;
        }

        /// <summary>窗体句柄</summary>
        public IntPtr Handle { get { return hwnd; } set { hwnd = value; } }

        /// <summary>窗体所在的矩形(RECT结构)</summary>
        public RECT Rect { get { return rect; } set { rect = value; } }
    }
}
