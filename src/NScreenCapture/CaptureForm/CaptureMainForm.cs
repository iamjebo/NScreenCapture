using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

using NScreenCapture.Types;
using NScreenCapture.Controls;
using NScreenCapture.Helpers;


namespace NScreenCapture.CaptureForm
{

    /****************************************************************
    * 
    *             Dcrp：CaptureScreen 截图主窗体
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com
    *           Update: 2013-12-2
    *
    *****************************************************************/


    /// <summary>
    ///  截图主窗体
    /// </summary>
    internal partial class CaptureMainForm : Form
    {
        #region Field

        private Bitmap m_screenImg;                             // 屏幕原始截图
        private Bitmap m_screenImgWithMask;                     // 带有遮罩层的原始屏幕截图

        private CaptureState m_captureState;                    // 当前截图操作状态

        private Point m_startPoint;                             // 第一次创建选区鼠标起点
        private Rectangle m_selectRect;                         // 选区

        private Rectangle m_editExRect;                         // 选区编辑之前的大小
        private MouseEditFlag m_mouseEditFlag;                  // 鼠标编辑标记

        // 与选区绘制操作有关的字段
        private DrawStyle m_drawStyle = DrawStyle.None;         // 绘制类型
        private ToolBarLocationFlag m_toolBarLocationFlag;      // 工具栏坐标位置标记

        private bool m_isStartDraw = false;                     // 是否开始在选区内绘制图形
        private Point m_startDrawPoint;                         // 绘制图形的起点
        private Point m_endDrawPoint;                           // 绘制图形的终点

        private List<Point> m_brushPointList;                   // 画刷操作时保存线条路径
        private List<Bitmap> m_UpdatedSelectImgList;            // 保存绘制后的选区图片

        // 自动选框
        private WindowsListManager m_windowListManager;         // 枚举桌面所有窗体管理器
        private Rectangle m_autoRect = Rectangle.Empty;         // 自动选框矩形

        private HotKey m_hotkey;                                // 系统热键辅助类

        private string m_resultImgDefaultSavePath;              // 截图文件默认保存路径
        private string m_resultImgFileName;                     // 截图文件保存文件名

        #endregion

        #region Property

        /// <summary>截图文件保存的默认目录</summary>
        public string ImageSaveInitialDirectory
        {
            get { return m_resultImgDefaultSavePath; }
            set { m_resultImgDefaultSavePath = value; }
        }

        /// <summary>截图文件名</summary>
        public string ImageSaveFilename
        {
            get { return m_resultImgFileName; }
            set { m_resultImgFileName = value; }
        }

        /// <summary>截图过程中选框矩形的颜色</summary>
        public Color LineColor
        {
            get { return DrawArgsManager.LINE_COLOR_AUTO; }
            set { DrawArgsManager.LINE_COLOR_AUTO = value; }
        }

        #endregion

        #region Override

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_startPoint = e.Location;

                if (m_captureState == CaptureState.None)  //新选区
                {
                    // 自动选框
                    if (m_autoRect != Rectangle.Empty)
                    {
                        m_selectRect = m_autoRect;
                        m_autoRect = Rectangle.Empty;
                        this.Invalidate();
                    }
                    m_captureState = CaptureState.CreatingRect;
                }
                else if (m_captureState == CaptureState.FinishedRect) //调整大小
                {
                    m_captureState = CaptureState.ResizingRect;
                    m_editExRect = m_selectRect;
                    m_mouseEditFlag = GetMouseEditFlag(e.Location);
                }
                else if (m_captureState == CaptureState.DrawInRect)   //在选区绘制图形
                {
                    if (m_selectRect.Contains(e.Location))
                    {
                        m_isStartDraw = true;
                        m_startDrawPoint = e.Location;
                        m_endDrawPoint = e.Location;

                        if (m_drawStyle == DrawStyle.Brush)
                        {
                            m_brushPointList.Clear();
                            m_brushPointList.Add(m_startDrawPoint);
                        }
                    }
                }

            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_captureState == CaptureState.None)
            {
                m_autoRect = m_windowListManager.GetCursorWindowRect();
            }
            else if (m_captureState == CaptureState.CreatingRect)
            {
                Point endPoint = e.Location;
                m_selectRect = new RECT(m_startPoint, endPoint).ToRectangle();
            }
            else if (m_captureState == CaptureState.ResizingRect)
            {
                if (m_UpdatedSelectImgList.Count == 0)    //未在选区内绘制图形可移动和调整选区
                    EditSelectRect(m_mouseEditFlag, e.Location.X, e.Location.Y);
            }
            else if (m_captureState == CaptureState.DrawInRect)
            {
                if (m_isStartDraw)
                {
                    m_endDrawPoint = e.Location;

                    if (m_drawStyle == DrawStyle.Brush)
                        m_brushPointList.Add(m_endDrawPoint);
                }
            }

            SetCursor();
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            textBoxString.Visible = false;

            if (e.Button == MouseButtons.Left)
            {
                if (m_selectRect == Rectangle.Empty)
                {
                    m_captureState = CaptureState.None;
                }
                else if (m_captureState == CaptureState.CreatingRect)    //创建新选区完成
                {
                    m_captureState = CaptureState.FinishedRect;
                    ShowCaptureToolBar();
                }
                else if (m_captureState == CaptureState.ResizingRect)      //完成调整大小
                {
                    m_captureState = CaptureState.FinishedRect;
                }
                else if (m_captureState == CaptureState.DrawInRect)
                {
                    if (m_isStartDraw)
                    {
                        m_isStartDraw = false;
                        m_endDrawPoint = e.Location;

                        if (m_drawStyle != DrawStyle.Text)
                            AddImgToUpdatedSelectImgList(false);

                        else
                        {
                            if (m_selectRect.Contains(e.Location))
                                ShowTextBox();
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_captureState == CaptureState.None)
                    this.Close();
                else
                {
                    if (!m_selectRect.Contains(e.Location))
                        ClearScreen();
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            DrawAutoSelectRect(g);
            DrawCustomeSelectRect(g);
            ShowCaptureToolBar();
            DrawSelectRectInfo(g);
            DrawMouseMoveInfo(g);

            if (m_captureState == CaptureState.DrawInRect && m_isStartDraw)
            {
                DrawOperateion(g, false);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            //复制选区图片到剪切板
            if (m_selectRect.Contains(e.Location))
            {
                FinishCaptureTool_Click(this, null);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotkeys();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_screenImg != null)
                {
                    m_screenImg.Dispose();
                    m_screenImg = null;
                }
                if (m_screenImgWithMask != null)
                {
                    m_screenImgWithMask.Dispose();
                    m_screenImgWithMask = null;
                }
                if (m_hotkey != null)
                {
                    m_hotkey.Dispose();
                    m_hotkey = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Constructor

        public CaptureMainForm()
        {
            InitializeComponent();
            FieldAndFormIni();
            TextBoxEventIni();
            ToolBarEventIni();
            RegisterHotkeys();
        }

        #endregion

        #region Draw Method

        /// <summary>绘制自动选框矩形</summary>
        private void DrawAutoSelectRect(Graphics g)
        {
            if (m_autoRect != Rectangle.Empty)
            {
                g.DrawImage(m_screenImg, m_autoRect, m_autoRect, GraphicsUnit.Pixel);

                if (m_captureState == CaptureState.None)
                {
                    using (Pen pen = new Pen(DrawArgsManager.LINE_COLOR_AUTO, DrawArgsManager.LINE_WIDTH_AUTO))
                    {
                        g.DrawRectangle(pen, m_autoRect);
                    }
                }
            }

        }

        /// <summary>绘制自定义选区矩形</summary>
        private void DrawCustomeSelectRect(Graphics g)
        {
            if (m_selectRect != Rectangle.Empty)
            {
                // 限制选区不能超过窗体边界
                if (m_selectRect.X < 0) { m_selectRect.X = 0; }
                if (m_selectRect.Y < 0) { m_selectRect.Y = 0; }
                if (m_selectRect.Right > ClientSize.Width) { m_selectRect.X = ClientSize.Width - m_selectRect.Width - 1; }
                if (m_selectRect.Bottom > ClientSize.Height) { m_selectRect.Y = ClientSize.Height - m_selectRect.Height - 1; }

                // draw image in selected rectangle
                if (m_UpdatedSelectImgList.Count == 0)
                {
                    g.DrawImage(m_screenImg, m_selectRect, m_selectRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(
                    m_UpdatedSelectImgList[m_UpdatedSelectImgList.Count - 1],
                    m_selectRect,
                    new Rectangle(Point.Empty, m_selectRect.Size),
                    GraphicsUnit.Pixel);
                }

                // draw select rectangle border
                DrawSelectRectBorder(g);
            }
        }

        /// <summary>绘制自定义选区矩形边框与结点</summary>
        private void DrawSelectRectBorder(Graphics g)
        {
            using (Pen redPen = new Pen(DrawArgsManager.LINE_COLOR_AUTO, DrawArgsManager.LINE_WIDTH_CUSTOM))
            {
                g.DrawRectangle(redPen, m_selectRect);

                Point[] rectNodes = GetRectNodes(m_selectRect);
                using (SolidBrush redBrush = new SolidBrush(DrawArgsManager.LINE_COLOR_AUTO))
                {
                    foreach (Point node in rectNodes)
                        g.FillRectangle(
                            redBrush,
                            new Rectangle(
                                node.X - DrawArgsManager.LINE_NODE_WIDTH,
                                node.Y - DrawArgsManager.LINE_NODE_WIDTH,
                                2 * DrawArgsManager.LINE_NODE_WIDTH,
                                2 * DrawArgsManager.LINE_NODE_WIDTH));
                }

            }
        }

        /// <summary>绘制选区矩形左上角的选区大小信息框</summary>
        private void DrawSelectRectInfo(Graphics g)
        {
            if (m_selectRect != Rectangle.Empty)
            {
                Rectangle infoRect = new Rectangle();
                infoRect.Width = DrawArgsManager.INFO_SELECTRECT_WIDTH;
                infoRect.Height = DrawArgsManager.INFO_SELECTRECT_HEIGHT;
                int offset = 3;
                infoRect.X = m_selectRect.X + offset;

                //上边界检查
                if (m_selectRect.Y < DrawArgsManager.INFO_SELECTRECT_HEIGHT + DrawArgsManager.LINE_WIDTH_CUSTOM)
                    infoRect.Y = m_selectRect.Y + offset;
                else
                    infoRect.Y = m_selectRect.Y - DrawArgsManager.INFO_SELECTRECT_HEIGHT - offset;

                //绘制alpha 背景
                using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(DrawArgsManager.INFO_MOVING_ALPHA, 0, 0, 0)))
                {
                    g.FillRectangle(sbrush, infoRect);
                    sbrush.Color = Color.White;
                    string infoStr = string.Format("大小：{0} x {1} \n双击复制图像", m_selectRect.Width, m_selectRect.Height);

                    using (Font fontStr = new Font("微软雅黑", 9))
                    {
                        g.DrawString(infoStr, fontStr, sbrush, new Point(infoRect.X + 5, infoRect.Y + 5));
                    }

                }
            }
        }

        /// <summary>绘制鼠标右下方的移动信息框</summary>
        private void DrawMouseMoveInfo(Graphics g)
        {
            if (m_captureState == CaptureState.None ||
                m_captureState == CaptureState.CreatingRect)
            {
                Rectangle infoRect = new Rectangle();
                infoRect.Width = DrawArgsManager.INFO_MOVING_WIDTH;
                infoRect.Height = DrawArgsManager.INFO_MOVING_PIC_HEIGHT + DrawArgsManager.INFO_MOVING_STR_HEIGHT;
                int xoffset = 10;
                int yoffset = 30;
                infoRect.X = Control.MousePosition.X + xoffset;
                infoRect.Y = Control.MousePosition.Y + yoffset;

                //边界检查限制
                if (Control.MousePosition.X > Width - infoRect.Width - xoffset)
                    infoRect.X = Control.MousePosition.X - infoRect.Width - xoffset;
                if (Control.MousePosition.Y > Height - infoRect.Height - yoffset)
                    infoRect.Y = Control.MousePosition.Y - infoRect.Height - yoffset;

                //放大图矩形
                Rectangle picRect = new Rectangle(infoRect.X,
                    infoRect.Y,
                    DrawArgsManager.INFO_MOVING_WIDTH,
                    DrawArgsManager.INFO_MOVING_PIC_HEIGHT);
                //鼠标rang范围矩形
                Rectangle mouseRangeRect = new Rectangle(Control.MousePosition.X - DrawArgsManager.INFO_MOVING_RANGE,
                    Control.MousePosition.Y - DrawArgsManager.INFO_MOVING_RANGE,
                    2 * DrawArgsManager.INFO_MOVING_RANGE,
                    2 * DrawArgsManager.INFO_MOVING_RANGE);

                using (Pen picPen = new Pen(Color.Black, 1))
                {
                    //放大图
                    using (Bitmap bmpSrc = new Bitmap(2 * DrawArgsManager.INFO_MOVING_RANGE, 2 * DrawArgsManager.INFO_MOVING_RANGE))
                    {
                        //获取鼠标rang范围内的原图片
                        using (Graphics g_src = Graphics.FromImage(bmpSrc))
                        {
                            g_src.DrawImage(
                                m_screenImg,
                                new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height),
                                mouseRangeRect,
                                GraphicsUnit.Pixel);
                        }

                        //绘制放大后的图
                        using (Bitmap bmpMagnified = ImageHelper.MagnifyImage(bmpSrc, 4))
                        {
                            g.DrawImage(
                                bmpMagnified,
                                picRect,
                                new Rectangle(0, 0, bmpMagnified.Width, bmpMagnified.Height),
                                GraphicsUnit.Pixel);
                        }
                    }

                    //内外边框
                    g.DrawRectangle(picPen, picRect);
                    picPen.Color = Color.White;
                    picRect.Inflate(-2, -2);
                    picPen.Width = 2;
                    g.DrawRectangle(picPen, picRect);

                    //十字架
                    picPen.Color = Color.FromArgb(200, DrawArgsManager.LINE_COLOR_AUTO);
                    picPen.Width = 4;
                    g.DrawLine(picPen,
                               new Point(picRect.X + picRect.Width / 2, picRect.Y + 2),
                               new Point(picRect.X + picRect.Width / 2, picRect.Bottom - 2));
                    g.DrawLine(picPen,
                               new Point(picRect.X + 2, picRect.Y + picRect.Height / 2),
                               new Point(picRect.Right - 2, picRect.Y + picRect.Height / 2));
                }

                //字符串矩形
                Rectangle strRect = new Rectangle(infoRect.X,
                    infoRect.Y + DrawArgsManager.INFO_MOVING_PIC_HEIGHT,
                    DrawArgsManager.INFO_MOVING_WIDTH,
                    DrawArgsManager.INFO_MOVING_STR_HEIGHT);

                int width = 0;
                int height = 0;
                if (m_autoRect != Rectangle.Empty)
                {
                    width = m_autoRect.Width;
                    height = m_autoRect.Height;
                }
                else
                {
                    width = m_selectRect.Width;
                    height = m_selectRect.Height;
                }

                Color currentColor = ImageHelper.GetColor(m_screenImg, Control.MousePosition.X, Control.MousePosition.Y);
                string infoStr = string.Format("大小：{0} x {1} \nRGB：({2},{3},{4})",
                                                    width,
                                                    height,
                                                    currentColor.R,
                                                    currentColor.G,
                                                    currentColor.B);

                //绘制字符串
                using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                {
                    g.FillRectangle(sbrush, strRect);
                    sbrush.Color = Color.White;

                    using (Font fontStr = new Font("微软雅黑", 9))
                    {
                        g.DrawString(infoStr, fontStr, sbrush, new Point(strRect.X + 5, strRect.Y + 5));
                    }

                }
            }
        }

        /// <summary>绘制矩形、画刷等操作</summary>
        private void DrawOperateion(Graphics g, bool isOffset)
        {
            //如果直接在窗体上绘制的话不需要偏移，
            //如果在选区图片所建的DC上绘制的话需要偏移
            //因此绘制临时的操作不需要偏移，而把操作添加到选区图片上的话需要偏移

            //限制绘制的操作在选区内，减1是为了防止右边缘和底边缘线条绘制不出来。
            if (m_endDrawPoint.X < m_selectRect.X) { m_endDrawPoint.X = m_selectRect.X; };
            if (m_endDrawPoint.X > m_selectRect.Right - 1) { m_endDrawPoint.X = m_selectRect.Right - 1; }
            if (m_endDrawPoint.Y < m_selectRect.Y) { m_endDrawPoint.Y = m_selectRect.Y; }
            if (m_endDrawPoint.Y > m_selectRect.Bottom - 1) { m_endDrawPoint.Y = m_selectRect.Bottom - 1; }

            Rectangle rect = new RECT(m_startDrawPoint, m_endDrawPoint).ToRectangle();
            if (isOffset)
            {
                rect.Offset(-m_selectRect.X, -m_selectRect.Y);
            }

            Color color = colorTableWithWidth.LineColor;
            int width = Convert.ToInt32(colorTableWithWidth.LineWidth);

            using (Pen pen = new Pen(color, width))
            {
                switch (m_drawStyle)
                {
                    case DrawStyle.Rectangle:
                        g.DrawRectangle(pen, rect);
                        break;
                    case DrawStyle.Ellipse:
                        g.DrawEllipse(pen, rect);
                        break;
                    case DrawStyle.Arrow:
                        using (Pen penArrow = new Pen(pen.Color, pen.Width))
                        {
                            penArrow.EndCap = LineCap.Custom;
                            penArrow.CustomEndCap = new AdjustableArrowCap(4, 4, true);

                            Point startDrawPoint = m_startDrawPoint;
                            Point endDrawPoint = m_endDrawPoint;
                            if (isOffset)
                            {
                                startDrawPoint.Offset(-m_selectRect.X, -m_selectRect.Y);
                                endDrawPoint.Offset(-m_selectRect.X, -m_selectRect.Y);
                            }
                            g.DrawLine(penArrow, startDrawPoint, endDrawPoint);
                        }
                        break;
                    case DrawStyle.Brush:
                        if (m_brushPointList.Count > 2)
                        {
                            Point[] brushPointArray = m_brushPointList.ToArray();
                            if (isOffset)
                            {
                                Point[] offsetPointArray = new Point[brushPointArray.Length];
                                Point tempPoint;
                                for (int i = 0; i < brushPointArray.Length; i++)
                                {
                                    tempPoint = brushPointArray[i];
                                    tempPoint.Offset(-m_selectRect.X, -m_selectRect.Y);
                                    offsetPointArray[i] = tempPoint;
                                }
                                brushPointArray = offsetPointArray;
                            }
                            g.DrawLines(pen, brushPointArray);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Private Method

        /// <summary>字段与窗体属性初始化</summary>
        private void FieldAndFormIni()
        {
            //字段初始化
            m_screenImg = GetOriginalScreenImg();
            m_screenImgWithMask = GetScreenImgWithMask();
            m_selectRect = Rectangle.Empty;
            m_editExRect = Rectangle.Empty;
            m_captureState = CaptureState.None;
            m_mouseEditFlag = MouseEditFlag.Defalut;
            m_UpdatedSelectImgList = new List<Bitmap>();
            m_brushPointList = new List<Point>();
            m_windowListManager = new WindowsListManager();
            m_windowListManager.GetWindowsList();
            m_resultImgDefaultSavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            m_resultImgFileName = "NScreenCapture.png";

            //双缓冲
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer,
                     true);


            //Form属性初始化
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.KeyPreview = true;

            this.BackgroundImage = m_screenImgWithMask;
            this.Cursor = new Cursor(Properties.Resources.cursor_default.Handle);
        }

        /// <summary>清除所有的屏幕截图操作，还原最开始的状态 </summary>
        private void ClearScreen()
        {
            m_captureState = CaptureState.None;
            m_mouseEditFlag = MouseEditFlag.Defalut;
            m_autoRect = Rectangle.Empty;
            m_selectRect = Rectangle.Empty;
            m_drawStyle = DrawStyle.None;
            m_isStartDraw = false;
            m_brushPointList.Clear();
            m_UpdatedSelectImgList.Clear();


            textBoxString.Visible = false;
            textBoxString.Text = string.Empty;
            captureToolBar.Visible = false;
            captureToolBar.Reset();
            colorTableWithWidth.Visible = false;
            colorTableWithFont.Visible = false;

            this.BackgroundImage = m_screenImgWithMask;
            this.Invalidate();
        }

        /// <summary>获取不带遮罩层的屏幕截图</summary>
        private Bitmap GetOriginalScreenImg()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenBmp;
        }

        /// <summary>获取带有遮罩层的屏幕截图</summary>
        private Bitmap GetScreenImgWithMask()
        {
            Bitmap sceenMaskImg = new Bitmap(m_screenImg);
            using (Graphics g = Graphics.FromImage(sceenMaskImg))
            {
                using (SolidBrush maskBrush = new SolidBrush(Color.FromArgb(DrawArgsManager.SCREEN_ALPHA, 0, 0, 0)))
                {
                    g.FillRectangle(maskBrush, 0, 0, sceenMaskImg.Width, sceenMaskImg.Height);
                    return sceenMaskImg;
                }
            }
        }

        /// <summary>获取指定矩形的8个控制节点</summary>
        private Point[] GetRectNodes(Rectangle rect)
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

        /// <summary>根据鼠标的位置获取鼠标编辑标记</summary>
        private MouseEditFlag GetMouseEditFlag(Point mousePt)
        {
            // 初始化
            MouseEditFlag flag = MouseEditFlag.Defalut;

            if (m_selectRect.Contains(mousePt))
                flag = MouseEditFlag.SizeAll;
            else
                flag = MouseEditFlag.Defalut;

            Point[] rectNodes = GetRectNodes(m_selectRect);
            for (int i = 0; i < rectNodes.Length; i++)
            {
                Rectangle nodeRect = new Rectangle(rectNodes[i].X - DrawArgsManager.LINE_NODE_WIDTH,
                                                   rectNodes[i].Y - DrawArgsManager.LINE_NODE_WIDTH,
                                                   2 * DrawArgsManager.LINE_NODE_WIDTH,
                                                   2 * DrawArgsManager.LINE_NODE_WIDTH);
                if (nodeRect.Contains(mousePt))
                {
                    flag = (MouseEditFlag)i;
                    break;
                }

            }
            return flag;
        }

        /// <summary>设置窗体的鼠标图像</summary>
        private void SetCursor()
        {
            MouseEditFlag flag = GetMouseEditFlag(Cursor.Position);

            if (m_selectRect.Contains(Cursor.Position) && m_captureState == CaptureState.DrawInRect)
            {
                this.Cursor = Cursors.Cross;
                return;
            }

            Cursor[] RectCursors = { Cursors.SizeNWSE,    // 西北
                                     Cursors.SizeWE,      // 西
                                     Cursors.SizeNESW,    // 西南
                                     Cursors.SizeNS,      // 南
                                     Cursors.SizeNWSE,    // 东南
                                     Cursors.SizeWE,      // 东
                                     Cursors.SizeNESW,    // 东北
                                     Cursors.SizeNS,      // 北
                                     Cursors.SizeAll,     // 移动
                                     Cursors.Default      // 默认
                                     };

            RectCursors[9] = new Cursor(Properties.Resources.cursor_default.Handle);

            Cursor cur = RectCursors[(int)flag];
            this.Cursor = cur;
        }

        /// <summary> 根据偏移量移动选区 </summary>
        private void MoveSelectRect(int x, int y)
        {
            if (m_selectRect != Rectangle.Empty &&
               (m_captureState == CaptureState.FinishedRect || m_captureState == CaptureState.ResizingRect))
            {
                m_selectRect.Offset(x, y);
                m_editExRect = m_selectRect;
                Invalidate();
            }
        }

        /// <summary> 根据鼠标编辑标记对选区进行编辑 </summary>
        private void EditSelectRect(MouseEditFlag flag, int offsetX, int offsetY)
        {
            //0-7：调整大小  8：移动  9：默认

            /*      0         7         6
             *      *********************
             *      *                   *
             *    1 *         8         * 5
             *      *                   *
             *      *********************
             *      2         3         4
             */

            if (m_selectRect == Rectangle.Empty) { return; }

            RECT rectEx = new RECT(m_editExRect);

            switch (flag)
            {
                case MouseEditFlag.WestNorth:
                    m_selectRect = new RECT(offsetX, offsetY, rectEx.Right, rectEx.Bottom).ToRectangle();
                    break;
                case MouseEditFlag.West:
                    m_selectRect = new RECT(offsetX, rectEx.Top, rectEx.Right, rectEx.Bottom).ToRectangle();
                    break;
                case MouseEditFlag.WestSouth:
                    m_selectRect = new RECT(offsetX, rectEx.Top, rectEx.Right, offsetY).ToRectangle();
                    break;
                case MouseEditFlag.South:
                    m_selectRect = new RECT(rectEx.Left, rectEx.Top, rectEx.Right, offsetY).ToRectangle();
                    break;
                case MouseEditFlag.EastSouth:
                    m_selectRect = new RECT(rectEx.Left, rectEx.Top, offsetX, offsetY).ToRectangle();
                    break;
                case MouseEditFlag.East:
                    m_selectRect = new RECT(rectEx.Left, rectEx.Top, offsetX, rectEx.Bottom).ToRectangle();
                    break;
                case MouseEditFlag.EastNorth:
                    m_selectRect = new RECT(rectEx.Left, offsetY, offsetX, rectEx.Bottom).ToRectangle();
                    break;
                case MouseEditFlag.North:
                    m_selectRect = new RECT(rectEx.Left, offsetY, rectEx.Right, rectEx.Bottom).ToRectangle();
                    break;
                case MouseEditFlag.SizeAll:
                    MoveSelectRect(offsetX - m_startPoint.X, offsetY - m_startPoint.Y);
                    m_startPoint.X = offsetX;
                    m_startPoint.Y = offsetY;
                    break;
                case MouseEditFlag.Defalut:
                    break;
            }
            m_editExRect = m_selectRect;
            this.Invalidate();
        }

        /// <summary> 注册系统热键 </summary>
        private void RegisterHotkeys()
        {
            if (m_hotkey == null) { m_hotkey = new HotKey(this.Handle); }

            // ESC 退出截图
            HotkeyEventHandler escHandler = delegate() { ExitCaptureTool_Click(null, null); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.None, Keys.Escape, escHandler);

            // Enter 完成截图
            HotkeyEventHandler enterHandler = delegate() { FinishCaptureTool_Click(null, null); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.None, Keys.Enter, enterHandler);

            // Ctrl + Z 撤销
            HotkeyEventHandler undoHandler = delegate() { UndoTool_Click(null, null); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.Ctrl, Keys.Z, undoHandler);

            // Ctrl + S 保存
            HotkeyEventHandler saveHandler = delegate() { SaveTool_Click(null, null); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.Ctrl, Keys.S, saveHandler);

            // Up键上移
            HotkeyEventHandler upMoveHandler = delegate() { MoveSelectRect(0, -1); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.None, Keys.Up, upMoveHandler);

            // Down键下移
            HotkeyEventHandler downMoveHanler = delegate() { MoveSelectRect(0, 1); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.None, Keys.Down, downMoveHanler);

            // Left键左移
            HotkeyEventHandler leftMoveHandler = delegate() { MoveSelectRect(-1, 0); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.None, Keys.Left, leftMoveHandler);

            // Right键右移
            HotkeyEventHandler rightMoveHandler = delegate() { MoveSelectRect(1, 0); };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.None, Keys.Right, rightMoveHandler);

            // Shift + Up 向上拉伸
            HotkeyEventHandler upSizeHandler = delegate()
            {
                if (m_selectRect.Y > 0)  //上边界禁止调整大小
                    EditSelectRect(MouseEditFlag.North, m_selectRect.X, m_selectRect.Y - 1);
            };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.Shift, Keys.Up, upSizeHandler);

            // Shift + Down 向下拉伸
            HotkeyEventHandler downSizeHandler = delegate()
            {
                if (m_selectRect.Bottom < Height)
                    EditSelectRect(MouseEditFlag.South, m_selectRect.X, m_selectRect.Bottom + 1);
            };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.Shift, Keys.Down, downSizeHandler);

            // Shift + Left 向左拉伸
            HotkeyEventHandler leftSizeHandler = delegate()
            {
                if (m_selectRect.X > 0)
                    EditSelectRect(MouseEditFlag.West, m_selectRect.X - 1, m_selectRect.Y);
            };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.Shift, Keys.Left, leftSizeHandler);

            // Shift + Right 向右拉伸
            HotkeyEventHandler rightSizeHandler = delegate()
            {
                if (m_selectRect.Right < Width)
                    EditSelectRect(MouseEditFlag.East, m_selectRect.Right + 1, m_selectRect.Y);
            };
            m_hotkey.RegisterHotKeys(HotKey.ModiferFlag.Shift, Keys.Right, rightSizeHandler);
        }

        /// <summary> 卸载已经注册的系统热键 </summary>
        private void UnregisterHotkeys()
        {
            if (m_hotkey != null) { m_hotkey.UnregisterHotKeys(); }
        }

        /// <summary> 获取屏幕原始选区截图</summary>
        private Bitmap GetBaseSelectImg()
        {
            if (m_selectRect.Size != Size.Empty)
            {
                Bitmap baseSelectImg = new Bitmap(m_selectRect.Width, m_selectRect.Height);
                Graphics g = Graphics.FromImage(baseSelectImg);
                g.DrawImage(
                    m_screenImg,
                    new Rectangle(Point.Empty, baseSelectImg.Size),
                    m_selectRect,
                    GraphicsUnit.Pixel);

                return baseSelectImg;
            }
            else
            {
                return null;
            }
        }

        /// <summary> 获取最终结果截图</summary>
        private Bitmap GetResultImg()
        {
            if (m_UpdatedSelectImgList.Count == 0)
            {
                return GetBaseSelectImg();
            }
            else
            {
                return m_UpdatedSelectImgList[m_UpdatedSelectImgList.Count - 1];
            }
        }

        /// <summary> 保存截图</summary>
        private bool SaveResultImg()
        {
            bool isSucceed = false;
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = m_resultImgDefaultSavePath;
                saveDialog.FileName = m_resultImgFileName;
                saveDialog.AddExtension = true;
                saveDialog.Filter = "BMP|*.bmp|PNG|*.png|GIF|*.gif|JPEG|*.jpg;*.jgeg";
                saveDialog.DefaultExt = "png";
                saveDialog.FilterIndex = 2;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    int length = saveDialog.FileName.Length;
                    string extion = saveDialog.FileName.Substring(length - 3, 3).ToLower();
                    ImageFormat format;
                    switch (extion)
                    {
                        case "bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case "png":
                            format = ImageFormat.Png;
                            break;
                        case "gif":
                            format = ImageFormat.Gif;
                            break;
                        case "jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        default:
                            format = ImageFormat.Png;
                            break;
                    }

                    Bitmap resultBmp = GetResultImg();
                    if (resultBmp != null)
                    {
                        resultBmp.Save(saveDialog.FileName, format);
                        isSucceed = true;
                    }
                }
            }
            return isSucceed;
        }

        /// <summary> 将绘制后的选区图片添加选区图片链表，以便实现撤销操作 </summary> 
        private void AddImgToUpdatedSelectImgList(bool isDrawText)
        {
            Image lastSelectImg = GetResultImg();   //上一个图片(最后的图片)
            if (lastSelectImg != null)
            {
                Bitmap lastImg_Copy = new Bitmap(lastSelectImg.Width, lastSelectImg.Height);
                using (Graphics g = Graphics.FromImage(lastImg_Copy))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;

                    //绘制上一个图片
                    g.DrawImage(
                        lastSelectImg,
                        new Rectangle(Point.Empty, lastImg_Copy.Size),
                        new Rectangle(Point.Empty, lastSelectImg.Size),
                        GraphicsUnit.Pixel);

                    //绘制操作为绘制文本
                    if (isDrawText)
                    {
                        Point textPoint = textBoxString.Location;
                        textPoint.Offset(-m_selectRect.X, -m_selectRect.Y);

                        using (SolidBrush sbrush = new SolidBrush(textBoxString.ForeColor))
                        {
                            g.DrawString(textBoxString.Text.Trim(), textBoxString.Font, sbrush, textPoint);
                        }
                    }
                    else
                    {
                        DrawOperateion(g, true);
                    }
                }

                m_UpdatedSelectImgList.Add(lastImg_Copy);
                Invalidate();
            }
        }

        #endregion

        #region TextBox

        /// <summary>显示文本框</summary>
        private void ShowTextBox()
        {
            int width = 50;
            if (m_selectRect.Right - m_startDrawPoint.X > width)
                textBoxString.Location = m_startDrawPoint;
            else
                textBoxString.Location = new Point(m_selectRect.Right - width, m_startDrawPoint.Y);

            textBoxString.Font = new Font(textBoxString.Font.FontFamily, Convert.ToSingle(colorTableWithFont.FontWidth));
            textBoxString.ForeColor = colorTableWithFont.FontColor;
            textBoxString.Size = new Size(width, textBoxString.Font.Height);
            textBoxString.Text = string.Empty;
            textBoxString.Visible = true;
            textBoxString.Focus();
        }

        /// <summary>文本框事件初始化</summary>
        private void TextBoxEventIni()
        {
            textBoxString.TextChanged += new EventHandler(textBoxString_TextChanged);
            textBoxString.LostFocus += new EventHandler(textBoxString_LostFocus);
            textBoxString.VisibleChanged += new EventHandler(textBoxString_VisibleChanged);
        }

        private void textBoxString_VisibleChanged(object sender, EventArgs e)
        {
            if (!textBoxString.Visible && textBoxString.Text.Trim().Length != 0)
                AddImgToUpdatedSelectImgList(true);
        }

        private void textBoxString_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBoxString.Text, textBoxString.Font);

            if (size.Width < 50)
                size.Width = 50;
            if (size.Height == 0)
                size.Height = textBoxString.Font.Height;

            textBoxString.Size = size;
        }

        private void textBoxString_LostFocus(object sender, EventArgs e)
        {
            textBoxString.Visible = false;

            if (textBoxString.Text.Length != 0)
            {
                AddImgToUpdatedSelectImgList(true);
                textBoxString.Text = string.Empty;
            }
        }

        #endregion

        #region ToolBar

        /// <summary>显示截图工具栏</summary>
        private void ShowCaptureToolBar()
        {
            bool isGetSelectRect = (m_captureState == CaptureState.FinishedRect ||
                                    m_captureState == CaptureState.ResizingRect ||
                                    m_captureState == CaptureState.DrawInRect);
            if (isGetSelectRect)
            {
                if (m_drawStyle == DrawStyle.None)
                {
                    SetToolBarLocation();
                    captureToolBar.Visible = true;
                }
            }
            else
            {
                captureToolBar.Visible = false;
            }
        }

        /// <summary>设置截图工具栏位置</summary>
        private void SetToolBarLocation()
        {
            Point location = Point.Empty;
            int yoffset_SelectRect_ToolBar = 5;     //toolbar与选取矩形底部垂直的距离

            if (m_selectRect.Right > captureToolBar.Width)
            {
                if (m_selectRect.Bottom > Height - yoffset_SelectRect_ToolBar - captureToolBar.Height)
                {
                    if (m_selectRect.Top < captureToolBar.Height + yoffset_SelectRect_ToolBar)
                    {
                        //右上角内部
                        m_toolBarLocationFlag = ToolBarLocationFlag.RightTop_Inner;
                        location = new Point(m_selectRect.Right - captureToolBar.Width - yoffset_SelectRect_ToolBar,
                                             m_selectRect.Top + yoffset_SelectRect_ToolBar);
                    }
                    else
                    {
                        //右上角外部
                        m_toolBarLocationFlag = ToolBarLocationFlag.RightTop_Outer;
                        location = new Point(m_selectRect.Right - captureToolBar.Width,
                                             m_selectRect.Top - captureToolBar.Height - yoffset_SelectRect_ToolBar);
                    }
                }
                else
                {
                    //右下角外部
                    m_toolBarLocationFlag = ToolBarLocationFlag.RightBottom_Outer;
                    location = new Point(m_selectRect.Right - captureToolBar.Width,
                                         m_selectRect.Bottom + yoffset_SelectRect_ToolBar);
                }
            }
            else
            {
                if (Height - m_selectRect.Bottom > captureToolBar.Height + yoffset_SelectRect_ToolBar)
                {
                    //左下角外部
                    m_toolBarLocationFlag = ToolBarLocationFlag.LeftBottom_Outer;
                    location = new Point(m_selectRect.Left,
                        m_selectRect.Bottom + yoffset_SelectRect_ToolBar);

                }
                else
                {
                    //左上角外部
                    m_toolBarLocationFlag = ToolBarLocationFlag.LeftTop_Outer;
                    location = new Point(m_selectRect.Left,
                                         m_selectRect.Top - yoffset_SelectRect_ToolBar - captureToolBar.Height);

                    if (location.Y < captureToolBar.Height + yoffset_SelectRect_ToolBar)
                    {
                        //左上角内部
                        location = new Point(m_selectRect.X + yoffset_SelectRect_ToolBar,
                                             m_selectRect.Top + yoffset_SelectRect_ToolBar);
                    }
                }

            }

            captureToolBar.Location = location;
        }

        /// <summary>避免当显示ColorTable等控件时其可见部分超过屏幕最底端 </summary> 
        private void UpdateToolBarLocation()
        {
            int yoffset_ToolBar_SelectRect = 5;         //ToolBar 与选区的垂直距离
            int yoffset_ToolBar_ColorTable = 3;         //ColorTableWithFont等控件与ToolBar的垂直距离
            int colorTableHeight = 40;                  //ColorTableWithFont等控件的高度都为40
            int yoffset_ColorTable_ScreenBounds = 2;    //ColorTableWithFont等控件底部与屏幕边缘的距离

            int totalOffset = yoffset_ToolBar_ColorTable + colorTableHeight + yoffset_ColorTable_ScreenBounds;

            if (m_toolBarLocationFlag == ToolBarLocationFlag.RightBottom_Outer)
            {
                if (Height - captureToolBar.Bottom > totalOffset)
                {
                    //右下角外部
                    m_toolBarLocationFlag = ToolBarLocationFlag.RightBottom_Outer;
                    captureToolBar.Location = new Point(m_selectRect.Right - captureToolBar.Width,
                                                        m_selectRect.Bottom + yoffset_ToolBar_SelectRect);
                    colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                             captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                    colorTableWithFont.Location = colorTableWithWidth.Location;
                }
                else
                {
                    //右上角外部
                    captureToolBar.Visible = false;
                    captureToolBar.Location = new Point(m_selectRect.Right - captureToolBar.Width,
                                                        m_selectRect.Top - captureToolBar.Height - yoffset_ToolBar_SelectRect);
                    if (captureToolBar.Top > totalOffset)
                    {
                        m_toolBarLocationFlag = ToolBarLocationFlag.RightTop_Outer;
                        colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                                 captureToolBar.Top - colorTableHeight - yoffset_ToolBar_ColorTable);
                        colorTableWithFont.Location = colorTableWithWidth.Location;
                    }
                    else
                    {
                        //右上角内部
                        m_toolBarLocationFlag = ToolBarLocationFlag.RightTop_Inner;
                        captureToolBar.Location = new Point(m_selectRect.Right - captureToolBar.Width - yoffset_ToolBar_SelectRect,
                                                            m_selectRect.Top + yoffset_ToolBar_SelectRect);
                        colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                                 captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                        colorTableWithFont.Location = colorTableWithWidth.Location;
                    }
                    captureToolBar.Visible = true;
                }
            }
            else if (m_toolBarLocationFlag == ToolBarLocationFlag.RightTop_Outer)
            {
                if (captureToolBar.Top > totalOffset)
                {
                    //右上角外部
                    m_toolBarLocationFlag = ToolBarLocationFlag.RightTop_Outer;
                    captureToolBar.Location = new Point(m_selectRect.Right - captureToolBar.Width,
                                                        m_selectRect.Top - captureToolBar.Height - yoffset_ToolBar_SelectRect);
                    colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                             captureToolBar.Top - colorTableHeight - yoffset_ToolBar_ColorTable);
                    colorTableWithFont.Location = colorTableWithWidth.Location;
                }
                else
                {
                    //右上角内部
                    m_toolBarLocationFlag = ToolBarLocationFlag.RightTop_Inner;
                    captureToolBar.Location = new Point(m_selectRect.Right - captureToolBar.Width - yoffset_ToolBar_SelectRect,
                                                        m_selectRect.Top + yoffset_ToolBar_SelectRect);
                    colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                             captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                    colorTableWithFont.Location = colorTableWithWidth.Location;
                }
            }
            else if (m_toolBarLocationFlag == ToolBarLocationFlag.RightTop_Inner)
            {
                //右上角内部
                captureToolBar.Location = new Point(m_selectRect.Right - captureToolBar.Width - yoffset_ToolBar_SelectRect,
                                                    m_selectRect.Top + yoffset_ToolBar_SelectRect);
                colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                         captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                colorTableWithFont.Location = colorTableWithWidth.Location;
            }
            else if (m_toolBarLocationFlag == ToolBarLocationFlag.LeftBottom_Outer)
            {

                if (Height - captureToolBar.Bottom > totalOffset)        //左下角外部
                {
                    captureToolBar.Location = new Point(m_selectRect.X,
                                                        m_selectRect.Bottom + yoffset_ToolBar_SelectRect);
                    colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                             captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                    colorTableWithFont.Location = colorTableWithWidth.Location;
                }
                else
                {
                    if (m_selectRect.Top > totalOffset)    //左上角外部
                    {
                        captureToolBar.Location = new Point(m_selectRect.X,
                                                            m_selectRect.Top - captureToolBar.Height - yoffset_ToolBar_SelectRect);
                        colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                                 captureToolBar.Top - yoffset_ToolBar_ColorTable - colorTableHeight);
                        colorTableWithFont.Location = colorTableWithWidth.Location;
                    }
                    else    //左上角内部
                    {
                        captureToolBar.Location = new Point(m_selectRect.X + yoffset_ToolBar_SelectRect,
                                                        m_selectRect.Top + yoffset_ToolBar_SelectRect);
                        colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                                 captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                        colorTableWithFont.Location = colorTableWithWidth.Location;
                    }
                }
            }
            else if (m_toolBarLocationFlag == ToolBarLocationFlag.LeftTop_Outer)
            {
                if (m_selectRect.Top > totalOffset)    //左上角外部
                {
                    captureToolBar.Location = new Point(m_selectRect.X,
                                                        m_selectRect.Top - captureToolBar.Height - yoffset_ToolBar_SelectRect);
                    colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                             captureToolBar.Top - yoffset_ToolBar_ColorTable - colorTableHeight);
                    colorTableWithFont.Location = colorTableWithWidth.Location;
                }
                else    //左上角内部
                {
                    captureToolBar.Location = new Point(m_selectRect.X + yoffset_ToolBar_SelectRect,
                                                    m_selectRect.Top + yoffset_ToolBar_SelectRect);
                    colorTableWithWidth.Location = new Point(captureToolBar.Left,
                                                             captureToolBar.Bottom + yoffset_ToolBar_ColorTable);
                    colorTableWithFont.Location = colorTableWithWidth.Location;
                }
            }
        }

        private void ShowColorTableWithWidth()
        {
            UpdateToolBarLocation();
            colorTableWithWidth.Visible = true;
            colorTableWithFont.Visible = false;
        }

        private void ShowColorTableWithFont()
        {
            UpdateToolBarLocation();
            colorTableWithFont.Visible = true;
            colorTableWithWidth.Visible = false;
        }

        /// <summary>截图工具栏事件初始化</summary>
        private void ToolBarEventIni()
        {
            captureToolBar.OnRectangleToolClick += new EventHandler(RectangleTool_Click);
            captureToolBar.OnEllipseToolClick += new EventHandler(EllipseTool_Click);
            captureToolBar.OnArrowToolClick += new EventHandler(ArrowTool_Click);
            captureToolBar.OnBrushToolClick += new EventHandler(BrushTool_Click);
            captureToolBar.OnTextToolClick += new EventHandler(TextTool_Click);
            captureToolBar.OnUndoToolClick += new EventHandler(UndoTool_Click);
            captureToolBar.OnSaveToolClick += new EventHandler(SaveTool_Click);
            captureToolBar.OnLoadImgToMSpaintToolClick += new EventHandler(LoadImgToMSpaintTool_Click);
            captureToolBar.OnExitCaptureToolClick += new EventHandler(ExitCaptureTool_Click);
            captureToolBar.OnFinishCaptureToolClick += new EventHandler(FinishCaptureTool_Click);
        }

        private void RectangleTool_Click(object sender, EventArgs e)
        {
            if (captureToolBar.IsRectangleToolDown)    //绘制矩形按钮处于未按下状态m
            {
                m_captureState = CaptureState.DrawInRect;
                m_drawStyle = DrawStyle.Rectangle;

                ShowColorTableWithWidth();
            }
            else
            {
                m_captureState = CaptureState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void EllipseTool_Click(object sender, EventArgs e)
        {
            if (captureToolBar.IsEllipseToolDown)
            {
                m_captureState = CaptureState.DrawInRect;
                m_drawStyle = DrawStyle.Ellipse;

                ShowColorTableWithWidth();
            }
            else
            {
                m_captureState = CaptureState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void ArrowTool_Click(object sender, EventArgs e)
        {
            if (captureToolBar.IsArrowToolDown)
            {
                m_captureState = CaptureState.DrawInRect;
                m_drawStyle = DrawStyle.Arrow;

                ShowColorTableWithWidth();
            }
            else
            {
                m_captureState = CaptureState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void BrushTool_Click(object sender, EventArgs e)
        {
            if (captureToolBar.IsBrushToolDown)
            {
                m_captureState = CaptureState.DrawInRect;
                m_drawStyle = DrawStyle.Brush;

                ShowColorTableWithWidth();
            }
            else
            {
                m_captureState = CaptureState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void TextTool_Click(object sender, EventArgs e)
        {
            if (captureToolBar.IsTextToolDown)
            {
                m_captureState = CaptureState.DrawInRect;
                m_drawStyle = DrawStyle.Text;

                ShowColorTableWithFont();
            }
            else
            {
                m_captureState = CaptureState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithFont.Visible = false;
            }
        }

        private void UndoTool_Click(object sender, EventArgs e)
        {
            colorTableWithFont.Visible = false;
            colorTableWithWidth.Visible = false;

            if (m_captureState == CaptureState.None)
            {
                this.Close();
                return;
            }

            //未对选取进行绘图操作
            if (m_UpdatedSelectImgList.Count == 0)
            {
                ClearScreen();
            }
            else
            {
                m_UpdatedSelectImgList.RemoveAt(m_UpdatedSelectImgList.Count - 1);
                captureToolBar.Reset();
                Invalidate();
            }
        }

        private void SaveTool_Click(object sender, EventArgs e)
        {
            if (m_captureState == CaptureState.None) { return; }

            colorTableWithWidth.Visible = false;
            colorTableWithFont.Visible = false;

            bool isSucessed = SaveResultImg();
            if (isSucessed)
            {
                this.Close();
            }
            else
            {
                m_drawStyle = DrawStyle.None;
                m_captureState = CaptureState.FinishedRect;
            }
        }

        private void LoadImgToMSpaintTool_Click(object sender, EventArgs e)
        {
            //获取画图程序路径
            string tempDir = Environment.GetEnvironmentVariable("TEMP");
            string mspaintDir = Environment.SystemDirectory + @"\mspaint.exe";

            if (Directory.Exists(tempDir) && File.Exists(mspaintDir))
            {
                Image resultImg = GetResultImg();
                string imgPath = tempDir + @"\WrysmileTemp.bmp";
                resultImg.Save(imgPath);
                Process.Start(mspaintDir, imgPath);
                this.Close();
            }
            else
            {
                MessageBox.Show("Load selected image to mspaint.exe faild.");
            }

        }

        private void ExitCaptureTool_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FinishCaptureTool_Click(object sender, EventArgs e)
        {
            if (m_captureState == CaptureState.None) { return; }

            Bitmap resultImg = GetResultImg();
            if (resultImg != null)
            {
                Clipboard.SetDataObject(resultImg, true);
                this.Close();
            }
        }

        #endregion

        #region Public

        /// <summary> 重置截图初始状态 </summary>
        public void ResetCapture()
        {
            m_screenImg = GetOriginalScreenImg();
            m_screenImgWithMask = GetScreenImgWithMask();
            ClearScreen();
        }

        /// <summary> 重新获取桌面所有窗体枚举列表 </summary>
        public void ResetWindowsList()
        {
            m_windowListManager.GetWindowsList();
        }

        #endregion
    }
}
