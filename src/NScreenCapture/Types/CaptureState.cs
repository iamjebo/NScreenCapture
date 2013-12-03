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
    /// 截图当前操作状态枚举
    /// </summary>
    internal enum CaptureState
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None,

        /// <summary>
        /// 正在创建新选区
        /// </summary>
        CreatingRect,

        /// <summary>
        /// 选区创建完成
        /// </summary>
        FinishedRect,

        /// <summary>
        /// 调整选区的大小
        /// </summary>
        ResizingRect,

        /// <summary>
        /// 在选区内绘制图形
        /// </summary>
        DrawInRect
    }
}
