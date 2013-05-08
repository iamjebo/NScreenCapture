using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenShot
{
    public partial class ScreenShotForm : Form
    {
        #region Const

        private const byte ALPHA_SCREEN = 120;                                  //遮罩层背景ALPHA值    

        private static readonly Color COLOR_LINE_CUSTOM = Color.LightGreen;     //自定义截图时选区线条颜色
        private const byte WIDTH_LINE_CUSTOM = 1;                               //自定义截图时选区线条宽度

        private const byte WIDTH_NODE = 3;                                      //选区8个调节大小的结点的宽度

        #endregion

        #region Field

        private Image m_ScreemImg;                                      //屏幕原始截图

        private bool m_MouseDown = false;
        private bool m_IsSelectRect = false;
        private Point m_StartPoint;                                     // 鼠标开始坐标
        private Point m_EndPoint;                                       // 鼠标结束坐标
        private RECT m_SelectRect;                                      // 选区

        #endregion

        #region Constructor

        public ScreenShotForm()
        {
            InitializeComponent();
            Ini();
        }

        #endregion

        #region Override

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !m_IsSelectRect)
            {
                m_MouseDown = true;
                m_StartPoint = e.Location;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_MouseDown)
            {
                m_EndPoint = e.Location;

                RECT downRect = new RECT(m_StartPoint, m_EndPoint);
                if (downRect.ToRectangle() != Rectangle.Empty)
                {
                    m_IsSelectRect = true;
                    m_SelectRect = downRect;
                    base.Invalidate();
                }

            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_MouseDown = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_IsSelectRect)
                {
                    if (m_SelectRect.ToRectangle().Contains(e.Location))
                        MessageBox.Show(" mouse up in select rectangle");
                    else
                        ClearScrrenShot();
                }
                else
                    this.Close();
            }

            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_IsSelectRect)
            {
                Graphics g = e.Graphics;

                //将选区的图片以屏幕原图突出显示出来
                g.DrawImage(
                    m_ScreemImg,
                    m_SelectRect.ToRectangle(),
                    m_SelectRect.ToRectangle(),
                    GraphicsUnit.Pixel);

                //画出选区矩形与结点
                using (Pen linePen = new Pen(COLOR_LINE_CUSTOM, WIDTH_LINE_CUSTOM))
                {
                    Point[] nodes = GetRectNodes(m_SelectRect);
                    g.DrawRectangle(linePen, m_SelectRect.ToRectangle());
                    using (SolidBrush sBrush = new SolidBrush(COLOR_LINE_CUSTOM))
                    {
                        foreach (Point node in nodes)
                        {
                            g.FillRectangle(
                                sBrush,
                                new Rectangle(
                                    node.X - WIDTH_NODE,
                                    node.Y - WIDTH_NODE,
                                    2 * WIDTH_NODE,
                                    2 * WIDTH_NODE));
                        }
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        #endregion

        #region Private

        private void Ini()
        {
            ScreenShotFormIni();
        }

        /// <summary>
        /// 截图窗体属性初始化
        /// </summary>
        private void ScreenShotFormIni()
        {
            //双缓冲
            SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            m_ScreemImg = GetScreenImg_Net();
            this.BackgroundImage = GetScreenImgWithMask();
        }

        /// <summary>
        /// .Net方式获取当前屏幕截图
        /// </summary>
        private Image GetScreenImg_Net()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenBmp;
        }

        /// <summary>
        /// GDI方式获取当前屏幕截图
        /// </summary>
        private Image GetScreenImg_GDI()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                IntPtr gHdc = g.GetHdc();
                IntPtr deskHwnd = Win32.GetDesktopWindow();
                IntPtr deskHdc = Win32.GetDC(deskHwnd);

                Win32.BitBlt(
                    gHdc,
                    0,
                    0,
                    bounds.Width,
                    bounds.Height,
                    deskHdc,
                    0,
                    0,
                    Win32.TernaryRasterOperations.SRCCOPY);

                Win32.ReleaseDC(deskHwnd, deskHdc);
                g.ReleaseHdc(gHdc);
            }
            return screenBmp;
        }

        /// <summary>
        /// 获取带有遮罩层的屏幕截图
        /// </summary>
        private Image GetScreenImgWithMask()
        {
            Image sceenMaskImg = new Bitmap(m_ScreemImg);
            using (Graphics g = Graphics.FromImage(sceenMaskImg))
            {
                using (SolidBrush maskBrush = new SolidBrush(Color.FromArgb(ALPHA_SCREEN, 0, 0, 0)))
                {
                    g.FillRectangle(maskBrush, 0, 0, sceenMaskImg.Width, sceenMaskImg.Height);
                    return sceenMaskImg;
                }
            }
        }

        /// <summary>
        /// 清除所有的屏幕截图操作，还原最开始的状态
        /// </summary>
        private void ClearScrrenShot()
        {
            m_MouseDown = false;
            m_IsSelectRect = false;
            m_SelectRect = new RECT(0, 0, 0, 0);

            this.BackgroundImage = GetScreenImgWithMask();
            this.Cursor = new Cursor(Properties.Resources.cursor_new.Handle);
            this.Invalidate();
        }

        /// <summary>
        /// 获取选区的8个调整大小的结点
        /// </summary>
        private Point[] GetRectNodes(RECT rect)
        {
            Point[] m_Nodes = new Point[8];
            m_Nodes[0] = rect.TopLeft;
            m_Nodes[1] = new Point(rect.Left, rect.Top + (rect.Bottom - rect.Top) / 2);
            m_Nodes[2] = new Point(rect.Left, rect.Bottom);
            m_Nodes[3] = new Point(rect.Left + (rect.Right - rect.Left) / 2, rect.Bottom);
            m_Nodes[4] = rect.BottomRight;
            m_Nodes[5] = new Point(rect.Right, rect.Top + (rect.Bottom - rect.Top) / 2);
            m_Nodes[6] = new Point(rect.Right, rect.Top);
            m_Nodes[7] = new Point(rect.Left + (rect.Right - rect.Left) / 2, rect.Top);
            return m_Nodes;
        }

        #endregion
    }
}
