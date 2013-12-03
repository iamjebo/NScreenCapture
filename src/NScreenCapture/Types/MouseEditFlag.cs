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
using System.Windows.Forms;

namespace NScreenCapture.Types
{
    /// <summary>
    /// 鼠标编辑操作枚举：0-7：调整大小  8：移动  9：默认
    /// </summary>
    internal enum MouseEditFlag
    {

        //0-7：调整大小  8：移动  9：默认

        /*      0         7         6
         *      *********************
         *      *                   *
         *    1 *         8         * 5
         *      *                   *
         *      *********************
         *      2         3         4
         */

        // 东：East 南：South 西：West 北：North

        /// <summary>西北 </summary>
        WestNorth = 0,

        /// <summary>西 </summary>
        West = 1,

        /// <summary> 西南</summary>
        WestSouth = 2,

        /// <summary> 南 </summary>
        South = 3,

        /// <summary> 东南</summary>
        EastSouth = 4,

        /// <summary>东 </summary>
        East = 5,

        /// <summary> 东北</summary>
        EastNorth = 6,

        /// <summary> 北</summary>
        North = 7,

        /// <summary> 移动</summary>
        SizeAll = 8,

        /// <summary> 默认</summary>
        Defalut = 9
    }
}
