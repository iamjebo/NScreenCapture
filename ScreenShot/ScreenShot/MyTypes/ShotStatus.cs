using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreenShot
{
    /// <summary>
    /// 自定义截图操作状态枚举
    /// </summary>
    public enum ShotStatus
    {
        /// <summary>
        /// 无操作
        /// </summary>
        ssNone,

        /// <summary>
        /// 创建新选区
        /// </summary>
        ssCreate,

        /// <summary>
        /// 编辑选区
        /// </summary>
        ssEidting
    }
}
