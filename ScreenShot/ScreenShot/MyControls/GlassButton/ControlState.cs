using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreenShot
{
    public enum ControlState
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
