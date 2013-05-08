using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ScreenShot
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Point topLeft, Point bottomRight)
        {
            Left = topLeft.X;
            Top = topLeft.Y;
            Right = bottomRight.X;
            Bottom = bottomRight.Y;
        }

        public RECT(Rectangle rect)
        {
            Left = rect.Left;
            Top = rect.Top;
            Right = rect.Right;
            Bottom = rect.Bottom;
        }

        public Point TopLeft
        {
            get
            {
                return new Point(Left, Top);
            }
        }

        public Point BottomRight
        {
            get
            {
                return new Point(Right, Bottom);
            }
        }

        public Size Size
        {
            get
            {
                return new Size(Math.Abs(Right - Left), Math.Abs(Bottom - Top));
            }
        }

        public static RECT FromXYWH(int x, int y, int width, int height)
        {
            return new RECT(x,
                            y,
                            x + width,
                            y + height);
        }

        public static RECT FromRectangle(Rectangle rect)
        {
            return new RECT(rect.Left,
                             rect.Top,
                             rect.Right,
                             rect.Bottom);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(
               Math.Min(Left, Right),
               Math.Min(Bottom, Top),
               Math.Abs(Right - Left),
               Math.Abs(Bottom - Top));
        }
    }
}
