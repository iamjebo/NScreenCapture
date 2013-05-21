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

namespace ScreenShot
{
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

        private Image m_ScreemImg;                              //屏幕原始截图

        private ShotState m_ShotState = ShotState.None;

        private Point m_StartPoint;
        private Rectangle m_SelectedRect = Rectangle.Empty;     //选区

        private Point[] m_RectNodes = new Point[8];             // 选区8个结点
        private int m_EditFlag = 8;                             // 编辑标记：0-7：调整大小  8：默认  9：移动
        private bool m_IsStartEditRect = false;
        private Rectangle m_EditExRect;                         // 选区编辑之前的大小


        #endregion

        #region Property

        public string ImageSaveInitialDirectory { get; set; }

        public string ImageSaveFilename { get; set; }

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
                if (m_SelectedRect == Rectangle.Empty)
                    m_ShotState = ShotState.None;
                else if (m_ShotState == ShotState.CreateRect)    //创建新选区完成，松开鼠标右键进入编辑状态
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
            base.OnMouseMove(e);

            if (m_ShotState == ShotState.CreateRect)
            {
                Point endPoint = e.Location;
                m_SelectedRect = new RECT(m_StartPoint, endPoint).ToRectangle();
            }
            else if (m_ShotState == ShotState.EditRect && m_IsStartEditRect == true)
            {
                ResizeSelectRect(m_EditFlag, e.Location);
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

            if (m_ShotState == ShotState.EditRect && m_SelectedRect.Contains(e.Location))
                SaveSelectRectImage();
        }

        #endregion

        #region Private

        private void Ini()
        {
            ScreenShotFormIni();
        }

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

            m_ScreemImg = GetScreenImg();
            this.BackgroundImage = GetScreenImgWithMask();
            this.Cursor = CURSOR_DEFAULT;
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
                using (SolidBrush maskBrush = new SolidBrush(Color.FromArgb(SCREEN_ALPHA_, 0, 0, 0)))
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
                Rectangle nodeRect = new Rectangle(m_RectNodes[i].X - LINE_NODE_WIDTH,
                                                   m_RectNodes[i].Y - LINE_NODE_WIDTH,
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

        private void MakeLimitToSelectRect()  //限制选区不能超过窗体边界
        {
            if (m_SelectedRect.X < 0)
                m_SelectedRect.X = 0;
            if (m_SelectedRect.Y < 0)
                m_SelectedRect.Y = 0;
            if (m_SelectedRect.Right > ClientSize.Width)
                m_SelectedRect.X = ClientSize.Width - m_SelectedRect.Width;
            if (m_SelectedRect.Bottom > ClientSize.Height)
                m_SelectedRect.Y = ClientSize.Height - m_SelectedRect.Height;
        }

        private void MoveSelectRect(int x, int y)
        {
            m_SelectedRect.Offset(x, y);
            Invalidate();
        }

        private void ResizeSelectRect(int flag, Point curPos)
        {
            RECT rectEx = new RECT(m_EditExRect);

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
                    MoveSelectRect(curPos.X - m_StartPoint.X, curPos.Y - m_StartPoint.Y);
                    m_StartPoint.X = curPos.X;
                    m_StartPoint.Y = curPos.Y;
                    break;
            }
        }

        private Image GetSelectRectImage()
        {
            Bitmap selectBmp = new Bitmap(m_SelectedRect.Width,
                                          m_SelectedRect.Height,
                                          PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(selectBmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(m_ScreemImg,
                                      new Rectangle(0, 0, selectBmp.Width, selectBmp.Height),
                                      m_SelectedRect,
                                      GraphicsUnit.Pixel);
            }
            return selectBmp;
        }

        private void SaveSelectRectImage()
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = ImageSaveInitialDirectory;
                saveDialog.FileName = ImageSaveFilename;
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".jpg";
                saveDialog.Filter = "JPEG|*.jpg;*.jgeg|BMP|*.bmp|PNG|*.png|GIF|*.gif";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format;
                    switch (saveDialog.FilterIndex)
                    {
                        case 1:
                            format = ImageFormat.Jpeg;
                            break;
                        case 2:
                            format = ImageFormat.Bmp;
                            break;
                        case 3:
                            format = ImageFormat.Png;
                            break;
                        case 4:
                            format = ImageFormat.Gif;
                            break;
                        default:
                            format = ImageFormat.Jpeg;
                            break;
                    }
                    Image selectImg = GetSelectRectImage();
                    selectImg.Save(saveDialog.FileName, format);
                    this.Close();
                }
            }
        }

        private void ProcessKeyDownEnvent(KeyEventArgs e)
        {
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
                    SaveSelectRectImage();
            }
            if (m_SelectedRect != Rectangle.Empty)
            {
                if (e.KeyCode == Keys.Up)
                {
                    if (e.Modifiers == Keys.Shift)   //Shift + ↑ 向上调整大小
                    {
                        //上边界禁止调整大小
                        if (m_SelectedRect.Y != 0)
                            m_SelectedRect = new Rectangle(m_SelectedRect.X,
                                                           m_SelectedRect.Y - 1,
                                                           m_SelectedRect.Width,
                                                           m_SelectedRect.Height + 1);
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
                        if (m_SelectedRect.Bottom != Height)
                            m_SelectedRect = new Rectangle(m_SelectedRect.X,
                                                           m_SelectedRect.Y,
                                                           m_SelectedRect.Width,
                                                           m_SelectedRect.Height + 1);
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
                        if (m_SelectedRect.X != 0)
                            m_SelectedRect = new Rectangle(m_SelectedRect.X - 1,
                                                           m_SelectedRect.Y,
                                                           m_SelectedRect.Width + 1,
                                                           m_SelectedRect.Height);
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
                        if (m_SelectedRect.Right != Width)
                            m_SelectedRect = new Rectangle(m_SelectedRect.X,
                                                           m_SelectedRect.Y,
                                                           m_SelectedRect.Width + 1,
                                                           m_SelectedRect.Height);
                        Invalidate();
                    }
                    else
                        MoveSelectRect(1, 0);
                }
            }
        }

        private void DrawSelectRect(Graphics g)
        {
            if (m_SelectedRect != Rectangle.Empty)
            {
                MakeLimitToSelectRect();

                //将选区的图片以屏幕原图突出显示出来
                g.DrawImage(m_ScreemImg, m_SelectedRect, m_SelectedRect, GraphicsUnit.Pixel);

                //绘制选区矩形与结点
                using (Pen redPen = new Pen(LINE_COLOR_CUSTOM, LINE_WIDTH_CUSTOM))
                {
                    //绘制选中矩形
                    g.DrawRectangle(redPen, m_SelectedRect);

                    //绘制选中矩形的8个调整大小的节点
                    m_RectNodes = GetRectNodes(m_SelectedRect);
                    using (SolidBrush redBrush = new SolidBrush(LINE_COLOR_CUSTOM))
                    {
                        foreach (Point node in m_RectNodes)
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
            if (m_SelectedRect != Rectangle.Empty)
            {
                Rectangle infoRect = new Rectangle();
                infoRect.Width = INFO_SELECTRECT_WIDTH;
                infoRect.Height = INFO_SELECTRECT_HEIGHT;
                int offset = 3;
                infoRect.X = m_SelectedRect.X + offset;

                //上边界检查
                if (m_SelectedRect.Y < INFO_SELECTRECT_HEIGHT + LINE_WIDTH_CUSTOM)
                    infoRect.Y = m_SelectedRect.Y + offset;
                else
                    infoRect.Y = m_SelectedRect.Y - INFO_SELECTRECT_HEIGHT - offset;

                //绘制alpha 背景
                using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(INFO_ALPHA, 0, 0, 0)))
                {
                    g.FillRectangle(sbrush, infoRect);
                    sbrush.Color = Color.White;
                    string infoStr = string.Format("大小：{0} x {1} \n双击可保存图像",
                                                    m_SelectedRect.Width,
                                                    m_SelectedRect.Height);

                    using (Font fontStr = new Font("微软雅黑", 9))
                    {
                        g.DrawString(infoStr, fontStr, sbrush, new Point(infoRect.X + 5, infoRect.Y + 5));
                    }

                }
            }
        }

        private void DrawMouseMoveInfo(Graphics g)
        {
            if (m_ShotState != ShotState.EditRect)
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
                    g.DrawImage(m_ScreemImg, picRect, rangeRect, GraphicsUnit.Pixel);

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

                Color currentColor = MethodHelper.GetColor(m_ScreemImg, Control.MousePosition.X, Control.MousePosition.Y);
                string infoStr = string.Format("大小：{0} x {1} \nRGB：({2},{3},{4})",
                                                    m_SelectedRect.Width,
                                                    m_SelectedRect.Height,
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

        #endregion
    }
}
