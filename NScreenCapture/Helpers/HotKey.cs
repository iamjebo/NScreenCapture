using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;

namespace NScreenCapture.Helpers
{
    /// <summary>
    /// 热键事件处理方法代理
    /// </summary>
    /// <param name="keyId">注册热键的全局标识符</param>
    public delegate void HotkeyEventHandler();

    /// <summary>
    /// 注册系统全局热键辅助类
    /// </summary>
    public class HotKey : IMessageFilter, IDisposable
    {
        #region HotKey

        private Dictionary<int, HotkeyEventHandler> hotkeyEvets;   // 热键事件处理方法字典

        /// <summary>需要注册热键窗体的句柄</summary>
        public IntPtr Handle { get; set; }

        public HotKey()
        {
            hotkeyEvets = new Dictionary<int, HotkeyEventHandler>();
        }

        /// <summary>
        /// 以指定窗体的句柄创建热键辅助类
        /// </summary>
        /// <param name="hwnd">需要注册热键窗体的句柄</param>
        public HotKey(IntPtr hwnd): this()
        {
            this.Handle = hwnd;
            Application.AddMessageFilter(this);
        }

        /// <summary>
        ///  注册系统全局热键
        /// </summary>
        /// <param name="fsModifers">系统键</param>
        /// <param name="key">虚拟键</param>
        /// <param name="handler">热键处理函数委托</param>
        /// <returns>如果注册成功，则返回热键的全局标识符</returns>
        public int RegisterHotKeys(ModiferFlag fsModifers, Keys key, HotkeyEventHandler handler)
        {
            Guid guid = System.Guid.NewGuid();
            int hotkeyId = GlobalAddAtom(guid.ToString());

            RegisterHotKey(Handle, hotkeyId, (uint)fsModifers, (uint)key);
            hotkeyEvets.Add(hotkeyId, handler);

            return hotkeyId;
        }

        /// <summary>卸载窗体的所有热键</summary>
        public void UnregisterHotKeys()
        {
            Application.RemoveMessageFilter(this);
            foreach (int keyId in hotkeyEvets.Keys)
            {
                UnregisterHotKey(Handle, keyId);
                GlobalDeleteAtom((ushort)keyId);
            }
        }

        #endregion

        #region IMessageFilter

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && hotkeyEvets.Count != 0)
            {
                foreach (int keyid in hotkeyEvets.Keys)
                {
                    if (m.WParam.ToInt32() == keyid)
                    {
                        hotkeyEvets[keyid]();
                        return true;
                    }

                }
            }
            return false;
        }

        #endregion

        #region IDisposable

        private bool disposed;

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    hotkeyEvets.Clear();
                    hotkeyEvets = null;
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code. 
        // This destructor will run only if the Dispose method 
        // does not get called. 
        // It gives your base class the opportunity to finalize. 
        // Do not provide destructors in types derived from this class.
        ~HotKey()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region ModiferFlag

        [Flags]
        public enum ModiferFlag
        {
            /// <summary> 无系统键</summary>
            None= 0,

            /// <summary> Alt键</summary>
            Alt= 0x1,

            /// <summary> Ctrl键 </summary>
            Ctrl = 0x2,

            /// <summary> Shift键</summary>
            Shift = 0x4,

            /// <summary> Windows键</summary>
            Win = 0x8
        }

        #endregion

        #region Win32

        public const int WM_HOTKEY = 0x312;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern ushort GlobalDeleteAtom(ushort nAtom);

        #endregion
    }
}
