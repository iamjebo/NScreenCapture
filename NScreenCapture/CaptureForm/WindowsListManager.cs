using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using NScreenCapture.Helpers;
using NScreenCapture.Types;

namespace NScreenCapture.CaptureForm
{
    /// <summary>
    /// 枚举桌面所有窗体管理器，实现鼠标移动自动选框功能
    /// </summary>
    internal class WindowsListManager
    {
        #region Field

        private List<WindowInfo> m_windowsList;             // 桌面所有窗体信息列表
        private List<WindowInfo> m_windowsFilterList;       // 筛选过滤之后的窗体信息列表
        private bool m_isFiltering = false;                 // 是否开始筛选过滤
        private IntPtr m_desktopHandle = IntPtr.Zero;       // 系统桌面句柄

        #endregion

        #region IniWindowsList

        private void AddWindowInfo(IntPtr hwnd)
        {
            RECT rect = new RECT(Point.Empty, Point.Empty);
            Win32.GetWindowRect(hwnd, out rect);

            WindowInfo window = new WindowInfo(hwnd, rect);
            m_windowsList.Add(window);
        }

        /// <summary>初始化窗体链表,获取所有窗体所在矩形</summary>
        public void GetWindowsList()
        {
            m_desktopHandle = GetDesktopHandle();
            m_windowsList.Clear();
            Win32.EnumWindows(new Win32.EnumWindowsProc(this.EnumWindowsProc), IntPtr.Zero);
        }

        /// <summary> 是否为正常窗体，过滤掉桌面、不可见等不需要的窗体 </summary>
        private bool IsNormalWindow(IntPtr hwnd)
        {
            bool result = false;

            StringBuilder caption = new StringBuilder(256);
            StringBuilder className = new StringBuilder(100);

            Win32.GetWindowText(hwnd, caption, caption.Capacity);
            Win32.GetClassName(hwnd, className, className.Capacity);

            string captionStr = caption.ToString();
            string classNameStr = className.ToString();

            result = !((captionStr == "Program Manager" && classNameStr == "Progman")
                      || (captionStr == "" && classNameStr == "SHELLDLL_DefView")
                      || (captionStr == "FolderView" && classNameStr == "SysListView32") && hwnd == m_desktopHandle);

            RECT winRect, outRect;
            Win32.GetWindowRect(hwnd, out winRect);

            RECT screenRect = new RECT(Screen.PrimaryScreen.Bounds);

            result = result && (hwnd != IntPtr.Zero)
                            && Win32.IsWindowVisible(hwnd)
                            && Win32.IsWindowEnabled(hwnd)
                            && (!Win32.IsIconic(hwnd))
                            && (!Win32.IsRectEmpty(ref winRect))
                            && Win32.IntersectRect(out outRect, ref screenRect, ref winRect); ;
            return result;
        }

        /// <summary> 枚举子窗体回调函数</summary>
        private bool EnumChildProc(IntPtr hwnd, IntPtr lParam)
        {
            if (IsNormalWindow(hwnd)) { AddWindowInfo(hwnd); }
            return true;
        }

        /// <summary> 枚举所有顶级窗体回调函数</summary>
        private bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam)
        {
            if (IsNormalWindow(hwnd)) { AddWindowInfo(hwnd); }
            Win32.EnumChildWindows(hwnd, new Win32.EnumWindowsProc(this.EnumChildProc), lParam);
            return true;
        }

        /// <summary> 获取系统桌面句柄</summary>
        private IntPtr GetDesktopHandle()
        {
            IntPtr hwnd_parent = Win32.FindWindow("Progman", "Program Manager");
            IntPtr hwnd_shelldll_defview = Win32.FindWindowEx(hwnd_parent, IntPtr.Zero, "SHELLDLL_DefView", null);
            return Win32.FindWindowEx(hwnd_shelldll_defview, IntPtr.Zero, "SysListView32", "FolderView");
        }



        #endregion

        #region Filter

        /// <summary>获取给定句柄窗体的父窗体句柄</summary>
        private IntPtr GetParentHandle(IntPtr hwnd)
        {
            return Win32.GetWindowLong(hwnd, Win32.GWL_HWNDPARENT);
        }

        /// <summary>判断2个窗体是否存在父子关系</summary>
        private bool IsParent(WindowInfo child, WindowInfo parent)
        {
            bool result = false;
            IntPtr childHwnd = child.Handle;
            IntPtr parentHwnd = parent.Handle;
            IntPtr hwnd = GetParentHandle(childHwnd);

            while (hwnd != IntPtr.Zero)
            {
                if (hwnd == parentHwnd)
                {
                    result = true;
                    break;
                }
                hwnd = GetParentHandle(hwnd);
            }

            return result;
        }

        /// <summary>对获取到的所有窗体列表进行筛选过滤</summary>
        private void FilterWindowsList()
        {
            m_windowsFilterList.Clear();

            foreach (var window in m_windowsList)
            {
                if (window.Rect.ToRectangle().Contains(Cursor.Position))
                    m_windowsFilterList.Add(window);
            }
        }

        /// <summary>递归筛选过滤</summary>
        private void DoFilter(List<WindowInfo> list)
        {
            // Root(Cut) -> A -> C -> D -> B(Cut) -> E(Cut)
            // A -> C -> D(Cut)
            // A(Cut) -> C
            // C
            //          Root
            //           /\\
            //     (Top)A B E
            //         /\
            //  (Top)C  D
            //
            // 递归修剪树直到Count = 1, 修剪完毕, 返回

            if (list.Count <= 1)
            {
                return;
            }
            else
            {
                WindowInfo info = list[0];
                int j = list.Count;

                for (int i = 1; i < list.Count - 1; i++)
                {
                    if (!IsParent(list[i], info))
                    {
                        j = i;
                        break;
                    }
                }
                for (int i = list.Count - 1; i >= j; i--)
                    list.Remove(list[i]);

                if (list.Count > 1)
                    list.Remove(list[0]);

                DoFilter(list);
            }
        }

        #endregion

        #region GetRect

        /// <summary>获取光标所在的窗体矩形</summary>
        public Rectangle GetCursorWindowRect()
        {
            Rectangle rect = Rectangle.Empty;

            WindowInfo window = GetCursorWindow();
            if (window != null)
            {
                rect = window.Rect.ToRectangle();
                rect.Intersect(Screen.PrimaryScreen.Bounds); // 去掉与桌面不相交的部分
            }
            return rect; ;
        }

        /// <summary>获取光标所在的窗体</summary>
        private WindowInfo GetCursorWindow()
        {
            WindowInfo window = null;
            if (m_isFiltering) { return null; }

            m_isFiltering = true;
            try
            {
                FilterWindowsList();
                DoFilter(m_windowsFilterList);
                if (m_windowsFilterList.Count == 1)
                    window = m_windowsFilterList[0];
            }
            finally
            {
                m_isFiltering = false;
            }

            return window;
        }

        #endregion

        #region Constructor Deconstructor

        public WindowsListManager()
        {
            m_windowsList = new List<WindowInfo>();
            m_windowsFilterList = new List<WindowInfo>();
        }

        ~WindowsListManager()
        {
            m_windowsList.Clear();
            m_windowsList = null;
            m_windowsFilterList.Clear();
            m_windowsFilterList = null;
        }

        #endregion
    }
}
