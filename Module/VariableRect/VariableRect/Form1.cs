using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace VariableRect
{
    public partial class Form1 : Form
    {
        private const int WIDTH_LINE = 2;
        private readonly Color COLOR_LINE = Color.Red;
        private const int WIDTH_NODE = 3;

        private ShotState m_ShotState = ShotState.None;

        private Point m_StartPoint;
        private Rectangle m_SelectedRect;

        private Point[] m_RectNodes = new Point[8];             // 选区8个结点
        private int m_EditFlag = 8;                             // 编辑标记：0-7：调整大小  8：默认  9：移动
        private bool m_IsStartEditRect = false;
        private Rectangle m_EditExRect;                         // 选区编辑之前的大小

        public Form1()
        {
            InitializeComponent();

            //双缓冲
            SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
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
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
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
                if (m_SelectedRect.Contains(e.Location))
                    MessageBox.Show("截图菜单");
                else
                {
                    m_ShotState = ShotState.None;
                    m_SelectedRect = Rectangle.Empty;
                    Invalidate();
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
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
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (m_SelectedRect != Rectangle.Empty)
            {
                using (Pen redPen = new Pen(COLOR_LINE, WIDTH_LINE))
                {
                    //绘制选中矩形
                    e.Graphics.DrawRectangle(redPen, m_SelectedRect);

                    //绘制选中矩形的8个调整大小的节点
                    m_RectNodes = GetRectNodes(m_SelectedRect);
                    using (SolidBrush redBrush = new SolidBrush(COLOR_LINE))
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

        //获取选区的8个调整大小的结点
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

        //设置选区调整大小的光标
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

        //限制选区不能超过窗体边界
        private void MakeLimitToPoint(ref Point curPos)
        {
            if (curPos.X < WIDTH_LINE)
                curPos.X = WIDTH_LINE;
            if (curPos.X > ClientSize.Width - WIDTH_LINE)
                curPos.X = ClientSize.Width - WIDTH_LINE;
            if (curPos.Y < WIDTH_LINE)
                curPos.Y = WIDTH_LINE;
            if (curPos.Y > ClientSize.Height - WIDTH_LINE)
                curPos.Y = ClientSize.Height - WIDTH_LINE;
        }

        private void ResizeSelectRect(int flag, Point curPos)
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
                    if (m_SelectedRect.X < WIDTH_LINE)
                        m_SelectedRect.X = WIDTH_LINE;
                    if (m_SelectedRect.Y < WIDTH_LINE)
                        m_SelectedRect.Y = WIDTH_LINE;
                    if (m_SelectedRect.Right > ClientSize.Width)
                        m_SelectedRect.X = ClientSize.Width - m_SelectedRect.Width - WIDTH_LINE;
                    if (m_SelectedRect.Bottom > ClientSize.Height - WIDTH_LINE)
                        m_SelectedRect.Y = ClientSize.Height - m_SelectedRect.Height - WIDTH_LINE;

                    m_StartPoint.X = curPos.X;
                    m_StartPoint.Y = curPos.Y;

                    break;
            }


        }

    }
}
