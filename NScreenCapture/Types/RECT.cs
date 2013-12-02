using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace NScreenCapture.Types
{
    /****************************************************************
    * 
    *             Dcrp：以2点构造一个矩形，属于Win32 平台的数据结构。
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com     
    *           Update: 2013-5-31
    *
    *****************************************************************/

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        /// <summary>
        /// 从2个任意的点构建RECT.
        /// </summary>
        public RECT(Point topLeft, Point bottomRight)
        {
            Left = Math.Min(topLeft.X, bottomRight.X);
            Top = Math.Min(topLeft.Y, bottomRight.Y);
            Right = Math.Max(topLeft.X, bottomRight.X);
            Bottom = Math.Max(topLeft.Y, bottomRight.Y);
        }

        /// <summary>
        /// 从指定的rectangle构建RECT.
        /// </summary>
        public RECT(Rectangle rect)
        {
            Left = rect.Left;
            Top = rect.Top;
            Right = rect.Right;
            Bottom = rect.Bottom;
        }

        public Point TopLeft
        {
            get { return new Point(Left, Top); }
        }

        public Point BottomRight
        {
            get { return new Point(Right, Bottom); }
        }

        public Size Size
        {
            get
            {
                return new Size(Math.Abs(Right - Left),
                                Math.Abs(Bottom - Top));
            }
        }

        /// <summary>
        /// 将RECT类型的矩形转换为Rectangle矩形。
        /// </summary>
        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Size.Width, Size.Height);
        }
    }
}
