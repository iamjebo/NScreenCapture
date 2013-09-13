using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ScreenCapture.Types;
using ScreenCapture.Controls;
using ScreenCapture.Helpers;

namespace ScreenCapture
{

    /****************************************************************
    * 
    *             Dcrp：CaptureScreen 主窗体，供截图操作类 Capture 调用
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com
    *           Update: 2013-9-12
    *
    *****************************************************************/

    internal partial class CaptureMainForm : Form
    {
        #region Const

        private const byte SCREEN_ALPHA_ = 150;                                  //遮罩层背景Alpha值    

        private Color LINE_COLOR_CUSTOM = Color.FromArgb(0, 174, 255);          //自定义截图时选区线条颜色
        private const byte LINE_WIDTH_CUSTOM = 1;                               //自定义截图时选区线条宽度
        private const byte LINE_NODE_WIDTH = 3;                                 //自定义截图时选区线条结点宽度

        private const byte INFO_ALPHA = 160;                                    //信息框背景Alpha值
        private const byte INFO_MOVING_WIDTH = 130;                             //鼠标移动信息框宽度
        private const byte INFO_MOVING_PIC_HEIGHT = 100;                        //鼠标移动信息框上半部分：放大图像高度
        private const byte INFO_MOVING_STR_HEIGHT = 40;                         //鼠标移动信息框下半部分：信息字符串高度
        private const byte INFO_MOVING_RANGE = 10;                              //放大图像离鼠标点的范围

        private const byte INFO_SELECTRECT_WIDTH = 115;                         //选区信息信息框宽度
        private const byte INFO_SELECTRECT_HEIGHT = 45;                         //选区信息信息框高度

        //默认截图保存路径与文件名
        private static readonly string DEFAULT_SAVE_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private const string DEFAULT_SAVE_FILENAME = "NScreenCapture";

        // 截图默认鼠标
        private static readonly Cursor CURSOR_DEFAULT = new Cursor(Properties.Resources.cursor_default.Handle);

        #endregion

        #region Field

        private Image m_screenImg;                              //屏幕原始截图
        private Image m_screenImgWithMask;                      //带有遮罩层的原始屏幕截图

        private CaptureState m_captureState;                    //当前截图操作状态

        private Point m_startPoint;
        private Rectangle m_selectRect;                         //选区

        private Point[] m_rectNodes = new Point[8];             // 选区8个结点
        private int m_editFlag = 8;                             // 编辑标记：0-7：调整大小  8：默认  9：移动
        private Rectangle m_editExRect;                         // 选区编辑之前的大小

        // 与选区绘制操作有关的字段
        private DrawStyle m_drawStyle = DrawStyle.None;         //绘制类型
        private ToolBarLocationFlag m_toolBarLocationFlag;      //工具栏坐标位置标记

        private bool m_isStartDraw = false;                     //是否开始在选区内绘制图形
        private Point m_startDrawPoint;                         //绘制图形的起点
        private Point m_endDrawPoint;                           //绘制图形的终点

        private List<Point> m_brushPointList;                   //画刷操作时保存线条路径
        private List<Image> m_UpdatedSelectImgList;             //保存绘制后的选区图片

        #endregion

        #region Constructor

        public CaptureMainForm()
        {
            InitializeComponent();
            FieldAndFormIni();
            TextBoxEventIni();
            ToolBarEventsIni();
        }

        #endregion

        #region Property

        /// <summary>截图保存的默认目录</summary>
        public string ImageSaveInitialDirectory { get; set; }

        /// <summary>截图文件名</summary>
        public string ImageSaveFilename { get; set; }

        #endregion

        #region Override

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_startPoint = e.Location;

                if (m_captureState == CaptureState.None)  //新选区
                {
                    m_captureState = CaptureState.CreatingRect;
                }
                else if (m_captureState == CaptureState.FinishedRect) //调整大小
                {
                    m_captureState = CaptureState.ResizingRect;
                    m_editExRect = m_selectRect;
                    m_editFlag = SetSelectRectCursor(e.Location);
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

            if (m_captureState == CaptureState.CreatingRect)
            {
                Point endPoint = e.Location;
                m_selectRect = new RECT(m_startPoint, endPoint).ToRectangle();
            }
            else if (m_captureState == CaptureState.ResizingRect)
            {
                if (m_UpdatedSelectImgList.Count == 0)    //未在选区内绘制图形可移动和调整选区
                    ResizeSelectRect(m_editFlag, e.Location);
            }
            else if (m_captureState == CaptureState.DrawInRect)
            {
                if (m_isStartDraw)
                {
                    m_endDrawPoint = e.Location;
                    SetEndPointBounds(ref m_endDrawPoint, m_selectRect);

                    if (m_drawStyle == DrawStyle.Brush)
                        m_brushPointList.Add(m_endDrawPoint);
                }
            }

            Invalidate();
            SetSelectRectCursor(e.Location);
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
                        SetEndPointBounds(ref m_endDrawPoint, m_selectRect);

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

            DrawSelectRect(g);
            ShowCaptureToolBar();
            DrawSelectRectInfo(g);
            DrawMouseMoveInfo(g);

            if (m_captureState == CaptureState.DrawInRect && m_isStartDraw)
            {
                DrawOperateion(g, false);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ProcessKeyDownEnvent(e);
            base.OnKeyDown(e);
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
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private Draw Method

        private void DrawSelectRect(Graphics g)
        {
            if (m_selectRect != Rectangle.Empty)
            {
                SetSelectRectBounds();

                //draw image in selected rectangle
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

                DrawSelectRectBorder(g);
            }
        }

        private void DrawSelectRectBorder(Graphics g)
        {
            //绘制选区矩形与结点
            using (Pen redPen = new Pen(LINE_COLOR_CUSTOM, LINE_WIDTH_CUSTOM))
            {
                g.DrawRectangle(redPen, m_selectRect);

                m_rectNodes = GetRectNodes(m_selectRect);
                using (SolidBrush redBrush = new SolidBrush(LINE_COLOR_CUSTOM))
                {
                    foreach (Point node in m_rectNodes)
                        g.FillRectangle(
                            redBrush,
                            new Rectangle(
                                node.X - LINE_NODE_WIDTH,
                                node.Y - LINE_NODE_WIDTH,
                                2 * LINE_NODE_WIDTH,
                                2 * LINE_NODE_WIDTH));
                }
            }
        }

        private void DrawSelectRectInfo(Graphics g)
        {
            if (m_selectRect != Rectangle.Empty)
            {
                Rectangle infoRect = new Rectangle();
                infoRect.Width = INFO_SELECTRECT_WIDTH;
                infoRect.Height = INFO_SELECTRECT_HEIGHT;
                int offset = 3;
                infoRect.X = m_selectRect.X + offset;

                //上边界检查
                if (m_selectRect.Y < INFO_SELECTRECT_HEIGHT + LINE_WIDTH_CUSTOM)
                    infoRect.Y = m_selectRect.Y + offset;
                else
                    infoRect.Y = m_selectRect.Y - INFO_SELECTRECT_HEIGHT - offset;

                //绘制alpha 背景
                using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(INFO_ALPHA, 0, 0, 0)))
                {
                    g.FillRectangle(sbrush, infoRect);
                    sbrush.Color = Color.White;
                    string infoStr = string.Format("大小：{0} x {1} \n双击复制图像",
                                                    m_selectRect.Width,
                                                    m_selectRect.Height);

                    using (Font fontStr = new Font("微软雅黑", 9))
                    {
                        g.DrawString(infoStr, fontStr, sbrush, new Point(infoRect.X + 5, infoRect.Y + 5));
                    }

                }
            }
        }

        private void DrawMouseMoveInfo(Graphics g)
        {
            if (m_captureState == CaptureState.None ||
                m_captureState == CaptureState.CreatingRect)
            {
                Rectangle infoRect = new Rectangle();
                infoRect.Width = INFO_MOVING_WIDTH;
                infoRect.Height = INFO_MOVING_PIC_HEIGHT + INFO_MOVING_STR_HEIGHT;
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
                    INFO_MOVING_WIDTH,
                    INFO_MOVING_PIC_HEIGHT);
                //鼠标rang范围矩形
                Rectangle mouseRangeRect = new Rectangle(Control.MousePosition.X - INFO_MOVING_RANGE,
                    Control.MousePosition.Y - INFO_MOVING_RANGE,
                    2 * INFO_MOVING_RANGE,
                    2 * INFO_MOVING_RANGE);

                using (Pen picPen = new Pen(Color.Black, 1))
                {
                    //放大图
                    using (Bitmap bmpSrc = new Bitmap(2 * INFO_MOVING_RANGE, 2 * INFO_MOVING_RANGE))
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
                        using (Bitmap bmpMagnified = MagnifyImage(bmpSrc, 4))
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
                    picPen.Color = LINE_COLOR_CUSTOM;
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
                    infoRect.Y + INFO_MOVING_PIC_HEIGHT,
                    INFO_MOVING_WIDTH,
                    INFO_MOVING_STR_HEIGHT);

                Color currentColor = MethodHelper.GetColor(m_screenImg, Control.MousePosition.X, Control.MousePosition.Y);
                string infoStr = string.Format("大小：{0} x {1} \nRGB：({2},{3},{4})",
                                                    m_selectRect.Width,
                                                    m_selectRect.Height,
                                                    currentColor.R,
                                                    currentColor.G,
                                                    currentColor.B);

                //绘制字符串
                using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(INFO_ALPHA, 0, 0, 0)))
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

        private void DrawOperateion(Graphics g, bool isOffset)
        {
            //如果直接在窗体上绘制的话不需要偏移，
            //如果在选区图片所建的DC上绘制的话需要偏移
            //因此绘制临时的操作不需要偏移，而把操作添加到选区图片上的话需要偏移
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

        #region Private

        private void FieldAndFormIni()
        {
            //字段初始化
            m_screenImg = GetOriginalScreenImg();
            m_screenImgWithMask = GetScreenImgWithMask();
            m_selectRect = Rectangle.Empty;
            m_captureState = CaptureState.None;
            m_UpdatedSelectImgList = new List<Image>();
            m_brushPointList = new List<Point>();

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
            this.Cursor = CURSOR_DEFAULT;
        }

        private Image GetOriginalScreenImg()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenBmp;
        }

        private Image GetScreenImgWithMask()
        {
            Image sceenMaskImg = new Bitmap(m_screenImg);
            using (Graphics g = Graphics.FromImage(sceenMaskImg))
            {
                using (SolidBrush maskBrush = new SolidBrush(Color.FromArgb(SCREEN_ALPHA_, 0, 0, 0)))
                {
                    g.FillRectangle(maskBrush, 0, 0, sceenMaskImg.Width, sceenMaskImg.Height);
                    return sceenMaskImg;
                }
            }
        }

        // 清除所有的屏幕截图操作，还原最开始的状态 
        private void ClearScreen()
        {
            m_captureState = CaptureState.None;
            m_selectRect = Rectangle.Empty;
            m_editExRect = Rectangle.Empty;
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

        // 获取选区矩形8个控制点
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

        private int SetSelectRectCursor(Point mousePt)
        {
            Cursor[] RectCursors = { Cursors.SizeNWSE,    // 西北
                                     Cursors.SizeWE,      // 西
                                     Cursors.SizeNESW,    // 西南
                                     Cursors.SizeNS,      // 南
                                     Cursors.SizeNWSE,    // 东南
                                     Cursors.SizeWE,      // 东
                                     Cursors.SizeNESW,    // 东北
                                     Cursors.SizeNS,      // 北
                                     CURSOR_DEFAULT,      // 默认
                                     Cursors.SizeAll};    // 移动
            //初始化
            int flag = 8;
            Cursor cur = RectCursors[8];

            if (m_selectRect.Contains(mousePt))
            {
                flag = 9;
                cur = RectCursors[9];

                if (m_captureState == CaptureState.DrawInRect)
                    cur = Cursors.Cross;
            }
            else
            {
                flag = 8;
                cur = RectCursors[8];
            }

            for (int i = 0; i < m_rectNodes.Length; i++)
            {
                Rectangle nodeRect = new Rectangle(m_rectNodes[i].X - LINE_NODE_WIDTH,
                                                   m_rectNodes[i].Y - LINE_NODE_WIDTH,
                                                   2 * LINE_NODE_WIDTH,
                                                   2 * LINE_NODE_WIDTH);
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

        // 限制选区不能超过窗体边界 
        private void SetSelectRectBounds()
        {
            if (m_selectRect.X < 0)
                m_selectRect.X = 0;
            if (m_selectRect.Y < 0)
                m_selectRect.Y = 0;
            if (m_selectRect.Right > ClientSize.Width)
                m_selectRect.X = ClientSize.Width - m_selectRect.Width - 1;
            if (m_selectRect.Bottom > ClientSize.Height)
                m_selectRect.Y = ClientSize.Height - m_selectRect.Height - 1;
        }

        // 限制鼠标移动的范围边界
        private void SetEndPointBounds(ref Point mousePoint, Rectangle bounds)
        {
            if (mousePoint.X < bounds.X)
                mousePoint.X = bounds.X;
            if (mousePoint.X > bounds.Right - 1)     //减1是为了防止右边缘和底边缘线条绘制不出来。
                mousePoint.X = bounds.Right - 1;
            if (mousePoint.Y < bounds.Y)
                mousePoint.Y = bounds.Y;
            if (mousePoint.Y > bounds.Bottom - 1)
                mousePoint.Y = bounds.Bottom - 1;
        }

        private void MoveSelectRect(int x, int y)
        {
            m_selectRect.Offset(x, y);
            Invalidate();
        }

        private void ResizeSelectRect(int flag, Point curPos)
        {
            RECT rectEx = new RECT(m_editExRect);

            switch (flag)   //0-7：调整大小  8：默认  9：移动
            {
                case 0:
                    m_selectRect = new RECT(curPos,
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 1:
                    m_selectRect = new RECT(new Point(curPos.X, rectEx.Top),
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 2:
                    m_selectRect = new RECT(new Point(curPos.X, rectEx.Top),
                                              new Point(rectEx.Right, curPos.Y)).ToRectangle();
                    break;
                case 3:
                    m_selectRect = new RECT(rectEx.TopLeft,
                                              new Point(rectEx.Right, curPos.Y)).ToRectangle();
                    break;
                case 4:
                    m_selectRect = new RECT(rectEx.TopLeft, curPos).ToRectangle();
                    break;
                case 5:
                    m_selectRect = new RECT(rectEx.TopLeft, new Point(curPos.X, rectEx.Bottom)).ToRectangle();
                    break;
                case 6:
                    m_selectRect = new RECT(new Point(rectEx.Left, curPos.Y),
                                              new Point(curPos.X, rectEx.Bottom)).ToRectangle();
                    break;
                case 7:
                    m_selectRect = new RECT(new Point(rectEx.Left, curPos.Y),
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 8:
                    break;
                case 9:
                    MoveSelectRect(curPos.X - m_startPoint.X, curPos.Y - m_startPoint.Y);
                    m_startPoint.X = curPos.X;
                    m_startPoint.Y = curPos.Y;
                    break;
            }
        }

        //将图片按照指定的倍数放大
        private Bitmap MagnifyImage(Bitmap bmpSrc, int times)
        {
            Bitmap bmpNew = new Bitmap(bmpSrc.Width * times, bmpSrc.Height * times, PixelFormat.Format32bppArgb);
            BitmapData bmpSrcData = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmpNewData = bmpNew.LockBits(new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] bySrcData = new byte[bmpSrcData.Height * bmpSrcData.Stride];
            Marshal.Copy(bmpSrcData.Scan0, bySrcData, 0, bySrcData.Length);
            byte[] byNewData = new byte[bmpNewData.Height * bmpNewData.Stride];
            Marshal.Copy(bmpNewData.Scan0, byNewData, 0, byNewData.Length);

            for (int y = 0, lenY = bmpSrc.Height; y < lenY; y++)
            {
                for (int x = 0, lenX = bmpSrc.Width; x < lenX; x++)
                {
                    for (int cy = 0; cy < times; cy++)
                    {
                        for (int cx = 0; cx < times; cx++)
                        {
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride)] = bySrcData[x * 4 + y * bmpSrcData.Stride];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 1] = bySrcData[x * 4 + y * bmpSrcData.Stride + 1];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 2] = bySrcData[x * 4 + y * bmpSrcData.Stride + 2];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 3] = bySrcData[x * 4 + y * bmpSrcData.Stride + 3];
                        }
                    }
                }
            }
            Marshal.Copy(byNewData, 0, bmpNewData.Scan0, byNewData.Length);
            bmpSrc.UnlockBits(bmpSrcData);
            bmpNew.UnlockBits(bmpNewData);
            return bmpNew;
        }

        private void ProcessKeyDownEnvent(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (m_captureState == CaptureState.None)
                {
                    this.Close();
                }
                else
                {
                    ClearScreen();
                }
            }

            if (m_selectRect != Rectangle.Empty)
            {
                if (e.KeyCode == Keys.Up)
                {
                    if (e.Modifiers == Keys.Shift)   //Shift + ↑ 向上调整大小
                    {
                        //上边界禁止调整大小
                        if (m_selectRect.Y != 0)
                            m_selectRect = new Rectangle(m_selectRect.X,
                                                           m_selectRect.Y - 1,
                                                           m_selectRect.Width,
                                                           m_selectRect.Height + 1);
                        Invalidate();
                    }
                    else
                        MoveSelectRect(0, -1);  //向上移动选区
                }

                if (e.KeyCode == Keys.Down)
                {
                    if (e.Modifiers == Keys.Shift)
                    {
                        //下边界禁止调整大小
                        if (m_selectRect.Bottom != Height)
                            m_selectRect = new Rectangle(m_selectRect.X,
                                                           m_selectRect.Y,
                                                           m_selectRect.Width,
                                                           m_selectRect.Height + 1);
                        Invalidate();
                    }
                    else
                        MoveSelectRect(0, 1);
                }

                if (e.KeyCode == Keys.Left)
                {
                    if (e.Modifiers == Keys.Shift)
                    {
                        //左边界禁止调整大小
                        if (m_selectRect.X != 0)
                            m_selectRect = new Rectangle(m_selectRect.X - 1,
                                                           m_selectRect.Y,
                                                           m_selectRect.Width + 1,
                                                           m_selectRect.Height);
                        Invalidate();
                    }
                    else
                        MoveSelectRect(-1, 0);
                }

                if (e.KeyCode == Keys.Right)
                {
                    if (e.Modifiers == Keys.Shift)
                    {

                        //右边界禁止调整大小
                        if (m_selectRect.Right != Width)
                            m_selectRect = new Rectangle(m_selectRect.X,
                                                           m_selectRect.Y,
                                                           m_selectRect.Width + 1,
                                                           m_selectRect.Height);
                        Invalidate();
                    }
                    else
                        MoveSelectRect(1, 0);
                }
            }
        }

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


        private Image GetBaseSelectImg()
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

        private Image GetLastSelectImg()
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

        private bool SaveResultImg()
        {
            bool isSucceed = false;
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                if (string.IsNullOrEmpty(ImageSaveInitialDirectory))
                { ImageSaveInitialDirectory = DEFAULT_SAVE_DIRECTORY; }

                if (String.IsNullOrEmpty(ImageSaveFilename))
                { ImageSaveFilename = DEFAULT_SAVE_FILENAME; }

                saveDialog.InitialDirectory = ImageSaveInitialDirectory;
                saveDialog.FileName = ImageSaveFilename;
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".jpg";
                saveDialog.Filter = "JPEG|*.jpg;*.jgeg|BMP|*.bmp|PNG|*.png|GIF|*.gif";

                int length = saveDialog.FileName.Length;
                string extion = saveDialog.FileName.Substring(length - 3, 3).ToLower();
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format;
                    switch (extion)
                    {
                        case "jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        case "bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case "png":
                            format = ImageFormat.Png;
                            break;
                        case "gif":
                            format = ImageFormat.Gif;
                            break;
                        default:
                            format = ImageFormat.Jpeg;
                            break;
                    }
                    Image resultImg = GetLastSelectImg();
                    if (resultImg != null)
                    {
                        resultImg.Save(saveDialog.FileName, format);
                        isSucceed = true;
                    }
                }
            }
            return isSucceed;
        }

        // 将绘制后的选区图片添加选区图片链表，以便实现撤销操作
        private void AddImgToUpdatedSelectImgList(bool isDrawText)
        {
            Image lastSelectImg = GetLastSelectImg();   //上一个图片(最后的图片)
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


        // 避免当显示ColorTable等控件时其可见部分超过屏幕最底端 
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

        private void ToolBarEventsIni()
        {
            captureToolBar.RectangleTool.Click += new EventHandler(RectangleTool_Click);
            captureToolBar.EllipseTool.Click += new EventHandler(EllipseTool_Click);
            captureToolBar.ArrowTool.Click += new EventHandler(ArrowTool_Click);
            captureToolBar.BrushTool.Click += new EventHandler(BrushTool_Click);
            captureToolBar.TextTool.Click += new EventHandler(TextTool_Click);
            captureToolBar.UndoTool.Click += new EventHandler(UndoTool_Click);
            captureToolBar.SaveTool.Click += new EventHandler(SaveTool_Click);
            captureToolBar.LoadImgToMSpaintTool.Click += new EventHandler(LoadImgToMSpaintTool_Click);
            captureToolBar.ExitCaptureTool.Click += new EventHandler(ExitCaptureTool_Click);
            captureToolBar.FinishCaptureTool.Click += new EventHandler(FinishCaptureTool_Click);
        }

        private void RectangleTool_Click(object sender, EventArgs e)
        {
            if (captureToolBar.RectangleTool.IsDown)    //绘制矩形按钮处于未按下状态
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
            if (captureToolBar.EllipseTool.IsDown)
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
            if (captureToolBar.ArrowTool.IsDown)
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
            if (captureToolBar.BrushTool.IsDown)
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
            if (captureToolBar.TextTool.IsDown)
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
            //未对选取进行绘图操作
            if (m_UpdatedSelectImgList.Count == 0)
            {
                ClearScreen();
            }
            else
            {
                m_UpdatedSelectImgList.RemoveAt(m_UpdatedSelectImgList.Count - 1);
                Invalidate();
            }
        }

        private void SaveTool_Click(object sender, EventArgs e)
        {
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
                Image resultImg = GetLastSelectImg();
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
            Image resultImg = GetLastSelectImg();
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

        #endregion
    }
}
