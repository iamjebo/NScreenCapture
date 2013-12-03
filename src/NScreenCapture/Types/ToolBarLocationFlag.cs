using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScreenCapture.Types
{
    /****************************************************************
    * 
    *             Dcrp：截图工具栏坐标位置标记枚举
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com             
    *           Update: 2013-7-9
    *
    *****************************************************************/

    /// <summary>
    /// 截图工具栏坐标位置标记枚举
    /// </summary>
    internal enum ToolBarLocationFlag
    {


        /*              5                           1       
         *              *****************************
         *              *                         2 *
         *              *                           *
         *              *                           *
         *              *                           *
         *              *****************************
         *              4                           3
         *  
         */

        /// <summary>
        /// 位于选区右上角的外部
        /// </summary>
        RightTop_Outer = 1,

        /// <summary>
        /// 位于选区右上角的内部
        /// </summary>
        RightTop_Inner = 2,

        /// <summary>
        /// 位于选区右下角的外部
        /// </summary>
        RightBottom_Outer = 3,

        /// <summary>
        /// 位于选区左下角的外部
        /// </summary>
        LeftBottom_Outer = 4,

        /// <summary>
        /// 位于选区左上角外部
        /// </summary>
        LeftTop_Outer = 5
    }
}
