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

namespace ScreenShot
{

    /****************************************************************
    * 
    *             Dcrp：ScreenShot主窗体
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *             Update: 2013-6-13
    *
    *****************************************************************/

    public partial class ScreenShotForm : Form
    {
        #region Const

        private const byte SCREEN_ALPHA_ = 150;                                  //遮罩层背景Alpha值    

        private static readonly Color LINE_COLOR_CUSTOM = Color.Lime;           //自定义截图时选区线条颜色
        private const byte LINE_WIDTH_CUSTOM = 1;                               //自定义截图时选区线条宽度
        private const byte LINE_NODE_WIDTH = 3;                                 //自定义截图时选区线条结点宽度

        private const byte INFO_ALPHA = 160;                                    //信息框背景Alpha值
        private const byte INFO_MOVING_WIDTH = 130;                             //鼠标移动信息框宽度
        private const byte INFO_MOVING_PIC_HEIGHT = 100;                        //鼠标移动信息框上半部分：放大图像高度
        private const byte INFO_MOVING_STR_HEIGHT = 40;                         //鼠标移动信息框下半部分：信息字符串高度
        private const byte INFO_MOVING_RANGE = 10;                              //放大图像的范围

        private const byte INFO_SELECTRECT_WIDTH = 115;                         //选区信息信息框宽度
        private const byte INFO_SELECTRECT_HEIGHT = 45;                         //选区信息信息框高度

        private static readonly Cursor CURSOR_DEFAULT =
            new Cursor(Properties.Resources.cursor_default.Handle);

        #endregion

        #region Field

        private Image m_screenImg;                              //屏幕原始截图
        private Image m_selectImg;                              //选区图片

        private ShotState m_shotState = ShotState.None;

        private Point m_startPoint;
        private Rectangle m_selectedRect = Rectangle.Empty;     //选区

        private Point[] m_rectNodes = new Point[8];             // 选区8个结点
        private int m_editFlag = 8;                             // 编辑标记：0-7：调整大小  8：默认  9：移动
        private Rectangle m_editExRect;                         // 选区编辑之前的大小

        #endregion

        #region Property

        public string ImageSaveInitialDirectory { get; set; }

        public string ImageSaveFilename { get; set; }

        #endregion

        #region Constructor

        public ScreenShotForm()
        {
            InitializeComponent();
            FieldAndFormIni();
            ToolBarEventsIni();
        }

        #endregion

        #region Override

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_startPoint = e.Location;

                if (m_shotState == ShotState.None)  //新选区
                {
                    m_shotState = ShotState.CreatingRect;
                }
                else if (m_shotState == ShotState.FinishedRect) //调整大小
                {
                    m_shotState = ShotState.ResizingRect;
                    m_editExRect = m_selectedRect;
                    m_editFlag = SetSelectRectCursor(e.Location);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_selectedRect == Rectangle.Empty)
                {
                    m_shotState = ShotState.None;
                }
                else if (m_shotState == ShotState.CreatingRect)    //创建新选区完成
                {
                    m_shotState = ShotState.FinishedRect;
                    ShowShotToolBar();
                }
                else if (m_shotState == ShotState.ResizingRect)      //完成调整大小
                {
                    m_shotState = ShotState.FinishedRect;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_shotState == ShotState.None)
                    this.Close();
                else
                {
                    if (m_selectedRect.Contains(e.Location))
                    {
                        MessageBox.Show("截图菜单");
                    }
                    else
                        ClearScrren();
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_shotState == ShotState.CreatingRect)
            {
                Point endPoint = e.Location;
                m_selectedRect = new RECT(m_startPoint, endPoint).ToRectangle();
            }
            else if (m_shotState == ShotState.ResizingRect)
            {
                ResizeSelectRect(m_editFlag, e.Location);
            }

            Invalidate();
            SetSelectRectCursor(e.Location);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            DrawSelectRect(g);
            ShowShotToolBar();
            DrawSelectRectInfo(g);
            DrawMouseMoveInfo(g);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ProcessKeyDownEnvent(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (m_selectedRect.Contains(e.Location))
            {
               bool isSucessed = SaveSelectImage();
               if (isSucessed)
                   this.Close();
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
                if (m_selectImg != null)
                {
                    m_selectImg.Dispose();
                    m_selectImg = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Public

        public void StartScreenShot()
        {
            this.ShowDialog();
        }

        #endregion

        #region Private

        private void FieldAndFormIni()
        {

            m_screenImg = GetScreenImg();

            //双缓冲
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer,
                     true);


            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.KeyPreview = true;

            this.BackgroundImage = GetScreenImgWithMask();
            this.Cursor = CURSOR_DEFAULT;
        }

        /* 获取原始屏幕截图 */
        private Image GetScreenImg()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenBmp;
        }

        /* 获取带有遮罩层的屏幕截图 */
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

        /* 清除所有的屏幕截图操作，还原最开始的状态 */
        private void ClearScrren()
        {
            m_shotState = ShotState.None;
            m_selectedRect = Rectangle.Empty;
            m_editExRect = Rectangle.Empty;
            m_drawStyle = DrawStyle.None;

            shotToolBar.Visible = false;
            colorTable.Visible = false;
            colorTableWithWidth.Visible = false;
            colorTableWithFont.Visible = false;

            this.BackgroundImage = GetScreenImgWithMask();
            this.Invalidate();
        }

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

            if (m_selectedRect.Contains(mousePt))
            {
                flag = 9;
                cur = RectCursors[9];
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

        /* 限制选区不能超过窗体边界 */
        private void MakeLimitToSelectRect()
        {
            if (m_selectedRect.X < 0)
                m_selectedRect.X = 0;
            if (m_selectedRect.Y < 0)
                m_selectedRect.Y = 0;
            if (m_selectedRect.Right > ClientSize.Width)
                m_selectedRect.X = ClientSize.Width - m_selectedRect.Width;
            if (m_selectedRect.Bottom > ClientSize.Height)
                m_selectedRect.Y = ClientSize.Height - m_selectedRect.Height;
        }

        private void MoveSelectRect(int x, int y)
        {
            m_selectedRect.Offset(x, y);
            Invalidate();
        }

        private void ResizeSelectRect(int flag, Point curPos)
        {
            RECT rectEx = new RECT(m_editExRect);

            switch (flag)   //0-7：调整大小  8：默认  9：移动
            {
                case 0:
                    m_selectedRect = new RECT(curPos,
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 1:
                    m_selectedRect = new RECT(new Point(curPos.X, rectEx.Top),
                                              rectEx.BottomRight).ToRectangle();
                    break;
                case 2:
                    m_selectedRect = new RECT(new Point(curPos.X, rectEx.Top),
                                              new Point(rectEx.Right, curPos.Y)).ToRectangle();
                    break;
                case 3:
                    m_selectedRect = new RECT(rectEx.TopLeft,
                                              new Point(rectEx.Right, curPos.Y)).ToRectangle();
                    break;
                case 4:
                    m_selectedRect = new RECT(rectEx.TopLeft, curPos).ToRectangle();
                    break;
                case 5:
                    m_selectedRect = new RECT(rectEx.TopLeft, new Point(curPos.X, rectEx.Bottom)).ToRectangle();
                    break;
                case 6:
                    m_selectedRect = new RECT(new Point(rectEx.Left, curPos.Y),
                                              new Point(curPos.X, rectEx.Bottom)).ToRectangle();
                    break;
                case 7:
                    m_selectedRect = new RECT(new Point(rectEx.Left, curPos.Y),
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

        private Image GetSelectImage()
        {
            Bitmap selectBmp = new Bitmap(m_selectedRect.Width,
                                          m_selectedRect.Height,
                                          PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(selectBmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(m_screenImg,
                                      new Rectangle(0, 0, selectBmp.Width, selectBmp.Height),
                                      m_selectedRect,
                                      GraphicsUnit.Pixel);
            }
            if (selectBmp != null)
                return selectBmp;
            else
                throw new Exception("Get selected image faild .");
        }

        private bool SaveSelectImage()
        {
            bool result = false;
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
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
                    Image selectImg = GetSelectImage();
                    selectImg.Save(saveDialog.FileName, format);
                    result = true;
                }
            }
            return result;
        }

        private void ProcessKeyDownEnvent(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (m_shotState == ShotState.None)
                    this.Close();
                else
                    ClearScrren();
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (m_shotState == ShotState.DrawInRect)
                    SaveSelectImage();
            }
            if (m_selectedRect != Rectangle.Empty)
            {
                if (e.KeyCode == Keys.Up)
                {
                    if (e.Modifiers == Keys.Shift)   //Shift + ↑ 向上调整大小
                    {
                        //上边界禁止调整大小
                        if (m_selectedRect.Y != 0)
                            m_selectedRect = new Rectangle(m_selectedRect.X,
                                                           m_selectedRect.Y - 1,
                                                           m_selectedRect.Width,
                                                           m_selectedRect.Height + 1);
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
                        if (m_selectedRect.Bottom != Height)
                            m_selectedRect = new Rectangle(m_selectedRect.X,
                                                           m_selectedRect.Y,
                                                           m_selectedRect.Width,
                                                           m_selectedRect.Height + 1);
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
                        if (m_selectedRect.X != 0)
                            m_selectedRect = new Rectangle(m_selectedRect.X - 1,
                                                           m_selectedRect.Y,
                                                           m_selectedRect.Width + 1,
                                                           m_selectedRect.Height);
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
                        if (m_selectedRect.Right != Width)
                            m_selectedRect = new Rectangle(m_selectedRect.X,
                                                           m_selectedRect.Y,
                                                           m_selectedRect.Width + 1,
                                                           m_selectedRect.Height);
                        Invalidate();
                    }
                    else
                        MoveSelectRect(1, 0);
                }
            }
        }

        private void DrawSelectRect(Graphics g)
        {
            if (m_selectedRect != Rectangle.Empty)
            {
                MakeLimitToSelectRect();

                //将选区的图片以屏幕原图突出显示出来
                g.DrawImage(m_screenImg, m_selectedRect, m_selectedRect, GraphicsUnit.Pixel);

                //绘制选区矩形与结点
                using (Pen redPen = new Pen(LINE_COLOR_CUSTOM, LINE_WIDTH_CUSTOM))
                {
                    //绘制选中矩形
                    g.DrawRectangle(redPen, m_selectedRect);

                    //绘制选中矩形的8个调整大小的节点
                    m_rectNodes = GetRectNodes(m_selectedRect);
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
        }

        private void DrawSelectRectInfo(Graphics g)
        {
            if (m_selectedRect != Rectangle.Empty)
            {
                Rectangle infoRect = new Rectangle();
                infoRect.Width = INFO_SELECTRECT_WIDTH;
                infoRect.Height = INFO_SELECTRECT_HEIGHT;
                int offset = 3;
                infoRect.X = m_selectedRect.X + offset;

                //上边界检查
                if (m_selectedRect.Y < INFO_SELECTRECT_HEIGHT + LINE_WIDTH_CUSTOM)
                    infoRect.Y = m_selectedRect.Y + offset;
                else
                    infoRect.Y = m_selectedRect.Y - INFO_SELECTRECT_HEIGHT - offset;

                //绘制alpha 背景
                using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(INFO_ALPHA, 0, 0, 0)))
                {
                    g.FillRectangle(sbrush, infoRect);
                    sbrush.Color = Color.White;
                    string infoStr = string.Format("大小：{0} x {1} \n双击可保存图像",
                                                    m_selectedRect.Width,
                                                    m_selectedRect.Height);

                    using (Font fontStr = new Font("微软雅黑", 9))
                    {
                        g.DrawString(infoStr, fontStr, sbrush, new Point(infoRect.X + 5, infoRect.Y + 5));
                    }

                }
            }
        }

        private void DrawMouseMoveInfo(Graphics g)
        {
            if (m_shotState == ShotState.None ||
                m_shotState == ShotState.CreatingRect)
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

                Rectangle picRect = new Rectangle(infoRect.X,
                    infoRect.Y,
                    INFO_MOVING_WIDTH,
                    INFO_MOVING_PIC_HEIGHT);
                Rectangle rangeRect = new Rectangle(Control.MousePosition.X - INFO_MOVING_RANGE,
                    Control.MousePosition.Y - INFO_MOVING_RANGE,
                    2 * INFO_MOVING_RANGE,
                    2 * INFO_MOVING_RANGE);

                using (Pen picPen = new Pen(Color.Black, 1))
                {
                    //放大图
                    g.DrawImage(m_screenImg, picRect, rangeRect, GraphicsUnit.Pixel);

                    //内外边框
                    g.DrawRectangle(picPen, picRect);
                    picPen.Color = Color.White;
                    picRect.Inflate(-1, -1);
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

                Rectangle strRect = new Rectangle(infoRect.X,
                    infoRect.Y + INFO_MOVING_PIC_HEIGHT,
                    INFO_MOVING_WIDTH,
                    INFO_MOVING_STR_HEIGHT);

                Color currentColor = MethodHelper.GetColor(m_screenImg, Control.MousePosition.X, Control.MousePosition.Y);
                string infoStr = string.Format("大小：{0} x {1} \nRGB：({2},{3},{4})",
                                                    m_selectedRect.Width,
                                                    m_selectedRect.Height,
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

        private void ShowShotToolBar()
        {
            bool isGetSelectRect = (m_shotState == ShotState.FinishedRect ||
                                    m_shotState == ShotState.ResizingRect ||
                                    m_shotState == ShotState.DrawInRect);
            if (isGetSelectRect)
            {
                if (m_drawStyle == DrawStyle.None)
                {
                    SetToolBarLocation();
                    shotToolBar.Visible = true;
                }
            }
            else
            {
                shotToolBar.Visible = false;
            }
        }

        private void SetToolBarLocation()
        {
            Point location = Point.Empty;
            int yoffset = 5;
            if (m_selectedRect.Bottom > Height - yoffset - shotToolBar.Height)
            {
                if (m_selectedRect.Top < shotToolBar.Height + yoffset)
                    location = new Point(m_selectedRect.Right - shotToolBar.Width - yoffset,
                                         m_selectedRect.Top + yoffset);
                else
                    location = new Point(m_selectedRect.Right - shotToolBar.Width,
                                         m_selectedRect.Top - shotToolBar.Height - yoffset);
            }
            else
            {
                location = new Point(m_selectedRect.Right - shotToolBar.Width,
                                     m_selectedRect.Bottom + yoffset);
            }
            if (m_selectedRect.Right < shotToolBar.Width)
                location = new Point(m_selectedRect.Left, location.Y);

            shotToolBar.Location = location;
        }

        #endregion
    }
}
