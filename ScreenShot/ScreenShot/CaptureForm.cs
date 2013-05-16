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
    public partial class CaptureForm : Form
    {
        #region Const

        private const byte ALPHA_SCREEN = 150;                                  //遮罩层背景ALPHA值    

        private static readonly Color COLOR_LINE_CUSTOM = Color.Red;            //自定义截图时选区线条颜色
        private const byte WIDTH_LINE_CUSTOM = 1;                               //自定义截图时选区线条宽度

        private const byte WIDTH_NODE = 3;                                      //选区8个调节大小的结点的宽度

        #endregion

        #region Field

        private Image m_ScreemImg;                              //屏幕原始截图

        private ShotState m_ShotState = ShotState.None;

        private Point m_StartPoint;
        private Rectangle m_SelectedRect;

        private Point[] m_RectNodes = new Point[8];             // 选区8个结点
        private int m_EditFlag = 8;                             // 编辑标记：0-7：调整大小  8：默认  9：移动
        private bool m_IsStartEditRect = false;
        private Rectangle m_EditExRect;                         // 选区编辑之前的大小


        #endregion

        #region Constructor

        public CaptureForm()
        {
            InitializeComponent();
            Ini();
        }

        #endregion

        #region Override

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_StartPoint = e.Location;

                if (m_ShotState == ShotState.None)  //新选区
                {
                    m_ShotState = ShotState.CreateRect;
                }
                else if (m_ShotState == ShotState.EditRect) //编辑选区
                {
                    m_IsStartEditRect = true;
                    m_EditExRect = m_SelectedRect;
                    m_EditFlag = SetSelectRectCursor(e.Location);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_ShotState == ShotState.CreateRect)    //创建新选区完成，松开鼠标右键进入编辑状态
                    m_ShotState = ShotState.EditRect;
                else if (m_ShotState == ShotState.EditRect) //编辑选区时，松开鼠标右键停止编辑选区
                    m_IsStartEditRect = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_ShotState == ShotState.None)
                    this.Close();
                else
                {
                    if (m_SelectedRect.Contains(e.Location))
                        MessageBox.Show("截图菜单");
                    else
                        ClearScrren();
                }

            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_ShotState == ShotState.CreateRect)
            {
                Point endPoint = e.Location;
                MakeLimitToPoint(ref endPoint);
                m_SelectedRect = new RECT(m_StartPoint, endPoint).ToRectangle();
                Invalidate();
            }
            else if (m_ShotState == ShotState.EditRect && m_IsStartEditRect == true)
            {
                ResizeSelectRect(m_EditFlag, e.Location);
                Invalidate();
            }

            SetSelectRectCursor(e.Location);
            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_SelectedRect != Rectangle.Empty)
            {
                Graphics g = e.Graphics;

                //将选区的图片以屏幕原图突出显示出来
                g.DrawImage(m_ScreemImg, m_SelectedRect, m_SelectedRect, GraphicsUnit.Pixel);

                //画出选区矩形与结点
                using (Pen redPen = new Pen(COLOR_LINE_CUSTOM, WIDTH_LINE_CUSTOM))
                {
                    //绘制选中矩形
                    e.Graphics.DrawRectangle(redPen, m_SelectedRect);

                    //绘制选中矩形的8个调整大小的节点
                    m_RectNodes = GetRectNodes(m_SelectedRect);
                    using (SolidBrush redBrush = new SolidBrush(COLOR_LINE_CUSTOM))
                    {
                        foreach (Point node in m_RectNodes)
                            e.Graphics.FillRectangle(
                                redBrush,
                                new Rectangle(
                                    node.X - WIDTH_NODE,
                                    node.Y - WIDTH_NODE,
                                    2 * WIDTH_NODE,
                                    2 * WIDTH_NODE));
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                if (m_ShotState == ShotState.None)
                    this.Close();
                else
                    ClearScrren();
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (m_ShotState == ShotState.EditRect)
                    ShowSaveFileDialog();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (m_ShotState == ShotState.EditRect && m_SelectedRect.Contains(e.Location))
                ShowSaveFileDialog();
            else
                this.Close();

        }

        #endregion

        #region Private

        private void Ini()
        {
            ScreenShotFormIni();
        }

        private void ScreenShotFormIni()     // 截图窗体属性初始化
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

            m_ScreemImg = GetScreenImg();
            this.BackgroundImage = GetScreenImgWithMask();
        }

        private Image GetScreenImg()    //获取原始屏幕截图
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenBmp;
        }

        private Image GetScreenImgWithMask()    //获取带有遮罩层的屏幕截图
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

        private void ClearScrren()  //清除所有的屏幕截图操作，还原最开始的状态
        {
            m_ShotState = ShotState.None;
            m_SelectedRect = Rectangle.Empty;
            m_IsStartEditRect = false;
            m_EditExRect = Rectangle.Empty;

            this.BackgroundImage = GetScreenImgWithMask();
            this.Cursor = new Cursor(Properties.Resources.cursor_new.Handle);
            this.Invalidate();

        }

        private Point[] GetRectNodes(Rectangle rect) //获取选区的8个调整大小的结点
        {
            Point[] nodes = new Point[8];
            nodes[0] = rect.Location;
            nodes[1] = new Point(rect.Left, rect.Top + (rect.Bottom - rect.Top) / 2);
            nodes[2] = new Point(rect.Left, rect.Bottom);
            nodes[3] = new Point(rect.Left + (rect.Right - rect.Left) / 2, rect.Bottom);
            nodes[4] = new Point(rect.Right, rect.Bottom);
            nodes[5] = new Point(rect.Right, rect.Top + (rect.Bottom - rect.Top) / 2);
            nodes[6] = new Point(rect.Right, rect.Top);
            nodes[7] = new Point(rect.Left + (rect.Right - rect.Left) / 2, rect.Top);
            return nodes;
        }

        private int SetSelectRectCursor(Point mousePt)   //设置选区调整大小的光标
        {
            Cursor[] RectCursors = { Cursors.SizeNWSE,    // 西北
                                     Cursors.SizeWE,      // 西
                                     Cursors.SizeNESW,    // 西南
                                     Cursors.SizeNS,      // 南
                                     Cursors.SizeNWSE,    // 东南
                                     Cursors.SizeWE,      // 东
                                     Cursors.SizeNESW,    // 东北
                                     Cursors.SizeNS,      // 北
                                     Cursors.Default,     // 默认
                                     Cursors.SizeAll};    // 移动
            //初始化
            int flag = 8;
            Cursor cur = RectCursors[8];

            if (m_SelectedRect.Contains(mousePt))
            {
                flag = 9;
                cur = RectCursors[9];
            }
            else
            {
                flag = 8;
                cur = RectCursors[8];
            }

            for (int i = 0; i < m_RectNodes.Length; i++)
            {
                Rectangle nodeRect = new Rectangle(m_RectNodes[i].X - WIDTH_NODE,
                                                   m_RectNodes[i].Y - WIDTH_NODE,
                                                   2 * WIDTH_NODE,
                                                   2 * WIDTH_NODE);
                if (nodeRect.Contains(mousePt))
                {
                    flag = i;
                    cur = RectCursors[i];
                    break;
                }
            }

            this.Cursor = cur;
            return flag;
        }

        private void MakeLimitToPoint(ref Point curPos) //限制选区不能超过窗体边界
        {
            if (curPos.X < WIDTH_LINE_CUSTOM)
                curPos.X = WIDTH_LINE_CUSTOM;
            if (curPos.X > ClientSize.Width - WIDTH_LINE_CUSTOM)
                curPos.X = ClientSize.Width - WIDTH_LINE_CUSTOM;
            if (curPos.Y < WIDTH_LINE_CUSTOM)
                curPos.Y = WIDTH_LINE_CUSTOM;
            if (curPos.Y > ClientSize.Height - WIDTH_LINE_CUSTOM)
                curPos.Y = ClientSize.Height - WIDTH_LINE_CUSTOM;
        }

        private void ResizeSelectRect(int flag, Point curPos)   //调整选区的大小
        {
            RECT rectEx = new RECT(m_EditExRect);
            MakeLimitToPoint(ref curPos);

            switch (flag)   //0-7：调整大小  8：默认  9：移动
            {
                case 0:
                    m_SelectedRect = new RECT(curPos,
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 1:
                    m_SelectedRect = new RECT(new Point(curPos.X, rectEx.Top),
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 2:
                    m_SelectedRect = new RECT(new Point(curPos.X, rectEx.Top),
                                              new Point(rectEx.Right, curPos.Y)).ToRectangle();
                    break;
                case 3:
                    m_SelectedRect = new RECT(rectEx.TopLeft,
                                              new Point(rectEx.Right, curPos.Y)).ToRectangle();
                    break;
                case 4:
                    m_SelectedRect = new RECT(rectEx.TopLeft, curPos).ToRectangle();
                    break;
                case 5:
                    m_SelectedRect = new RECT(rectEx.TopLeft, new Point(curPos.X, rectEx.Bottom)).ToRectangle();
                    break;
                case 6:
                    m_SelectedRect = new RECT(new Point(rectEx.Left, curPos.Y),
                                              new Point(curPos.X, rectEx.Bottom)).ToRectangle();
                    break;
                case 7:
                    m_SelectedRect = new RECT(new Point(rectEx.Left, curPos.Y),
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 8:
                    break;
                case 9:
                    m_SelectedRect.Offset(curPos.X - m_StartPoint.X, curPos.Y - m_StartPoint.Y);

                    //边界限制
                    if (m_SelectedRect.X < WIDTH_LINE_CUSTOM)
                        m_SelectedRect.X = WIDTH_LINE_CUSTOM;
                    if (m_SelectedRect.Y < WIDTH_LINE_CUSTOM)
                        m_SelectedRect.Y = WIDTH_LINE_CUSTOM;
                    if (m_SelectedRect.Right > ClientSize.Width)
                        m_SelectedRect.X = ClientSize.Width - m_SelectedRect.Width - WIDTH_LINE_CUSTOM;
                    if (m_SelectedRect.Bottom > ClientSize.Height - WIDTH_LINE_CUSTOM)
                        m_SelectedRect.Y = ClientSize.Height - m_SelectedRect.Height - WIDTH_LINE_CUSTOM;

                    m_StartPoint.X = curPos.X;
                    m_StartPoint.Y = curPos.Y;

                    break;
            }
        }

        private void ShowSaveFileDialog()   //保存截图
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".jpg";
                saveDialog.Filter = "JPEG|*.jpg;*.jgeg|BMP|*.bmp|PNG|*.png|GIF|*.gif";
                saveDialog.ShowDialog();
            }
        }

        #endregion
    }
}
