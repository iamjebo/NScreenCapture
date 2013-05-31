using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
namespace ScreenShot
{
    /****************************************************************
   * 
   *           Author：苦笑(wrysmile)
   *             Blog: http://www.cnblogs.com/Keep-Silence-/
   *             Date: 2013-5-28
   *
   *****************************************************************/

    public class ColorButton : Button
    {
        #region Field

        private static readonly Color COLOR_BORDAR = Color.Black;
        private static readonly Color COLOR_INNERBORDAR = Color.White;

        private Color m_Color = Color.Red;
        private ControlState m_State = ControlState.Normal;
        private bool m_IsKeepHighlight = false;     //是否长保持高亮状态                   

        #endregion

        #region Constructor

        public ColorButton()
        {
            this.Size = new Size(16, 16);
        }

        public ColorButton(Color color)
        {
            m_Color = color;
            this.Size = new Size(16, 16);
        }

        #endregion

        #region Properity

        public Color Color
        {
            get { return m_Color; }
            set { m_Color = value; Invalidate(); }
        }

        [Description("是否长保持高亮状态")]
        public bool IsKeepHighlight
        {
            get { return m_IsKeepHighlight; }
            set { m_IsKeepHighlight = value; }
        }

        #endregion

        #region Override

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (m_State == ControlState.Normal)
                m_State = ControlState.Highlight;
            else
                m_State = ControlState.Down;

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                m_State = ControlState.Down;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (m_State == ControlState.Down)
                m_State = ControlState.Down;
            else
                m_State = ControlState.Normal;

            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            m_State = ControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (m_IsKeepHighlight)
                m_State = ControlState.Highlight;

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            rect.Inflate(-1, -1);
            using (SolidBrush sbrush = new SolidBrush(m_Color))
            {
                g.FillRectangle(sbrush, rect);
                using (Pen pen = new Pen(COLOR_BORDAR))
                {
                    if (m_State == ControlState.Highlight ||
                        m_State == ControlState.Down)
                    {
                        pen.Color = COLOR_INNERBORDAR;
                        g.DrawRectangle(pen,rect);
                        rect.Inflate(1, 1);
                        pen.Color = COLOR_BORDAR;
                        g.DrawRectangle(pen, rect);
                    }
                    else
                        g.DrawRectangle(pen, rect);
                }
            }
        }

        #endregion
    }
}
