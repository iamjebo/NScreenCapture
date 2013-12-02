using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScreenCapture.Types
{
    /// <summary>
    /// 窗体信息实体类，用于构造窗体列表。
    /// </summary>
    internal class WindowInfo
    {
        private IntPtr hwnd;
        private RECT rect;

        public WindowInfo() { }

        public WindowInfo(IntPtr hwnd, RECT rect)
        {
            this.hwnd = hwnd;
            this.rect = rect;
        }

        /// <summary>窗体句柄</summary>
        public IntPtr Handle { get { return hwnd; } set { hwnd = value; } }

        /// <summary>窗体所在的矩形(Rect结构)</summary>
        public RECT Rect { get { return rect; } set { rect = value; } }
    }
}
