using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ScreenCapture.Helpers
{

    /****************************************************************
   * 
   *             Dcrp：封装Win32平台的API函数与常量
   *           Author：曹江波
   *             Blog: http://www.cnblogs.com/Keep-Silence-/
   *           E-mail: caojiangbocn@gmail.com             
   *             Date: 2013-5-18
   *
   *****************************************************************/

    internal class Win32
    {

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_NCLBUTTONDOWN = 0x00A1;

        public const int HTCAPTION = 2;

        public const int WS_CLIPCHILDREN = 0x02000000;

        [DllImport("gdi32.dll")]
        public static extern int CreateRoundRectRgn(int x1, int y1, int x2, int y2, int x3, int y3);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hwnd, int hRgn, Boolean bRedraw);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject", CharSet = CharSet.Ansi)]
        public static extern int DeleteObject(int hObject);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);

    }
}
