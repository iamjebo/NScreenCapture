using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScreenCapture.Types
{

    /****************************************************************
    * 
    *             Dcrp：自定义控件的状态枚举
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com             
    *           Update: 2013-5-31
    *
    *****************************************************************/

    internal enum MyControlState
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 0,
        /// <summary>
        ///  /鼠标进入
        /// </summary>
        Highlight = 1,
        /// <summary>
        /// 鼠标按下
        /// </summary>
        Down = 2
    }
}
