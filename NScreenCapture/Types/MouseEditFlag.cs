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
        Defalut = 9,
    }
}
