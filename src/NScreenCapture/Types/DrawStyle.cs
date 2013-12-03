using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScreenCapture.Types
{
    /****************************************************************
    * 
    *             Dcrp：CaptureToolBar 绘图类型枚举
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com           
    *           Update: 2013-6-12
    *
    *****************************************************************/

    /// <summary>
    /// 工具栏绘制类型枚举
    /// </summary>
    internal enum DrawStyle
    {
        /// <summary>
        /// 无任何绘制
        /// </summary>
        None,

        /// <summary>
        /// 绘制矩形
        /// </summary>
        Rectangle,

        /// <summary>
        /// 绘制椭圆
        /// </summary>
        Ellipse,

        /// <summary>
        /// 绘制箭头
        /// </summary>
        Arrow,

        /// <summary>
        /// 绘制画刷
        /// </summary>
        Brush,

        /// <summary>
        /// 绘制文字
        /// </summary>
        Text
    }
}
