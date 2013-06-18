using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScreenShot
{
    /****************************************************************
    * 
    *             Dcrp：宽度点控件
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *             Date: 2013-5-31
    *
    *****************************************************************/

    public class WidthDotButton : RadioButton
    {
        #region  Fileds

        private MyControlState m_state = MyControlState.Normal;
        private DotWidth m_lineWidth;

        private Image m_glassHotImg;
        private Image m_glassDownImg;
        private Image m_widthDotImg;

        #endregion

        #region Properity

        /// <summary>
        /// 宽度点控件的宽度
        /// </summary>
        public DotWidth DotWidth
        {
            get { return m_lineWidth; }
            set { m_lineWidth = value; }
        }

        #endregion

        #region Constructor

        public WidthDotButton()
        {
            m_glassHotImg = MethodHelper.GetImageFormResourceStream("MyControls.WidthDotButton.Res.glassbtn_hot.png");
            m_glassDownImg = MethodHelper.GetImageFormResourceStream("MyControls.WidthDotButton.Res.glassbtn_down.png");
            m_widthDotImg = MethodHelper.GetImageFormResourceStream("MyControls.WidthDotButton.Res.dot.png");

            this.BackColor = Color.Transparent;

            Size size = new Size(24, 24);
            this.MinimumSize = size;
            this.Size = size;
            this.Text = string.Empty;
        }

        #endregion

        #region override

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (Checked)
                m_state = MyControlState.Down;
            else
                m_state = MyControlState.Highlight;

            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (Checked)
                m_state = MyControlState.Down;
            else
                m_state = MyControlState.Normal;

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (Checked)
                    m_state = MyControlState.Down;
                else
                    m_state = MyControlState.Highlight;

                Invalidate();
            }
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            if (Checked)
                m_state = MyControlState.Down;
            else
                m_state = MyControlState.Normal;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            base.OnPaintBackground(pe);

            //draw dot width image 
            Rectangle dotimgRect = new Rectangle(
                (ClientSize.Width - m_widthDotImg.Width) / 2,
                (ClientSize.Height - m_widthDotImg.Height) / 2,
                4,
                4);

            switch (m_lineWidth)
            {
                case DotWidth.Minimize:
                    break;
                case DotWidth.Medium:
                    dotimgRect.Inflate(2, 2);
                    break;
                case DotWidth.Maximize:
                    dotimgRect.Inflate(4, 4);
                    break;
            }

            MethodHelper.DrawImageWithNineRect(
                        pe.Graphics,
                        m_widthDotImg,
                        dotimgRect,
                        new Rectangle(0, 0, m_widthDotImg.Width, m_widthDotImg.Height));

            //draw glass image
            switch (m_state)
            {
                case MyControlState.Highlight:
                    MethodHelper.DrawImageWithNineRect(
                        pe.Graphics,
                        m_glassHotImg,
                        ClientRectangle,
                        new Rectangle(0, 0, m_glassDownImg.Width, m_glassDownImg.Height));
                    break;

                case MyControlState.Down:
                    MethodHelper.DrawImageWithNineRect(
                        pe.Graphics,
                        m_glassDownImg,
                        ClientRectangle,
                        new Rectangle(0, 0, m_glassDownImg.Width, m_glassDownImg.Height));
                    break;

                default:
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_glassDownImg != null)
                    m_glassDownImg.Dispose();
                if (m_glassHotImg != null)
                    m_glassHotImg.Dispose();
                if (m_widthDotImg != null)
                    m_widthDotImg.Dispose();

                m_glassDownImg = null;
                m_glassHotImg = null;
                m_widthDotImg = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
