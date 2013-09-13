using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreenCapture.Types
{
    /****************************************************************
    * 
    *             Dcrp：当前的截图操作状态，方便控制操作流程。
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com         
    *           Update: 2013-5-31
    *
    *****************************************************************/

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
