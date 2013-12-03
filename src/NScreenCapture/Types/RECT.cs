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
    *             Dcrp：Win32平台的RECT矩形结构
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com     
    *           Update: 2013-12-2
    *
    *****************************************************************/

    /// <summary>
    ///  Win32平台的RECT矩形结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        /// <summary> 从2个点构建RECT.</summary>
        public RECT(Point topLeft, Point bottomRight)
        {
            Left = Math.Min(topLeft.X, bottomRight.X);
            Top = Math.Min(topLeft.Y, bottomRight.Y);
            Right = Math.Max(topLeft.X, bottomRight.X);
            Bottom = Math.Max(topLeft.Y, bottomRight.Y);
        }

        /// <summary> 从2个点的坐标分量构建RECT</summary>
        public RECT(int x1,int y1,int x2,int y2)
        {
            Left = Math.Min(x1,x2);
            Top = Math.Min(y1,y2);
            Right = Math.Max(x1,x2);
            Bottom = Math.Max(y1,y2);
        }

        /// <summary>从指定的Rectangle构建RECT.</summary>
        public RECT(Rectangle rect)
        {
            Left = rect.Left;
            Top = rect.Top;
            Right = rect.Right;
            Bottom = rect.Bottom;
        }

        /// <summary> 大小 </summary>
        public Size Size
        {
            get{return new Size(Math.Abs(Right - Left), Math.Abs(Bottom - Top));}
        }

        /// <summary>将RECT类型的矩形转换为Rectangle矩形。</summary>
        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Size.Width, Size.Height);
        }
    }
}
