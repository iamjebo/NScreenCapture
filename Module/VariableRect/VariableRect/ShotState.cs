using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableRect
{
    /// <summary>
    /// 当前的截图操作状态
    /// </summary>
    public enum ShotState
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None,

        /// <summary>
        /// 创建新选区
        /// </summary>
        CreateRect,

        /// <summary>
        /// 编辑选区
        /// </summary>
        EditRect
    }
}
