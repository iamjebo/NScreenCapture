using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using ScreenCapture.Types;
using ScreenCapture.Helpers;

namespace ScreenCapture.Controls
{

    /****************************************************************
    * 
    *             Dcrp：玻璃按钮，用以构造截图工具栏控件(ShotToolBar)
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com
    *             Date: 2013-5-22
    *
    *****************************************************************/

    internal class GlassButton : RadioButton
    {
        #region  Fileds

        private bool m_holdingSpace = false;

        private bool m_isDown = false;
        private MyControlState m_state = MyControlState.Normal;
        private Font m_defaultFont;

        private Image m_glassHotImg;
        private Image m_glassDownImg;

        private ToolTip m_toolTip;

        #endregion

        #region Constructor

        public GlassButton()
        {
            m_glassHotImg = Properties.Resources.Glassbtn_hot ;
            m_glassDownImg = Properties.Resources.Glassbtn_down;
            m_defaultFont = new Font("微软雅黑", 9);
            m_toolTip = new ToolTip();

            this.BackColor = Color.Transparent;
            this.Size = new Size(75, 23);
            this.Font = m_defaultFont;
        }

        #endregion

        #region  Properties

        public bool IsDown { get { return m_isDown; } }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        [Description("The text associated with the control.")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        [Description("The font used to display text in the control.")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [Description("当鼠标放在控件可见处的提示文本")]
        public string ToolTipText { get; set; }

        #endregion

        #region Hiding

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }

        #endregion

        #region override

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            //show tool tip 
            if (ToolTipText != string.Empty)
            {
                HideToolTip();
                ShowTooTip(ToolTipText);
            }

            if (Checked)
                m_state = MyControlState.Down;
            else
                m_state = MyControlState.Highlight;

            Invalidate();

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (m_isDown)
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
                if (m_isDown)
                {
                    m_state = MyControlState.Highlight;
                    m_isDown = false;
                }
                else
                {
                    m_isDown = true;
                    m_state = MyControlState.Down;
                }
                Invalidate();
            }
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            if (Checked)
            {
                m_isDown = true;
                m_state = MyControlState.Down;
            }
            else
            {
                m_isDown = false;
                m_state = MyControlState.Normal;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            base.OnPaintBackground(pe);

            Rectangle imageRect, textRect;
            CalculateRect(out imageRect, out textRect);

            if (Image != null)
            {
                pe.Graphics.DrawImage(
                    Image,
                    imageRect,
                    new Rectangle(0, 0, Image.Width, Image.Height),
                    GraphicsUnit.Pixel);
            }
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

            if (Text.Trim().Length != 0)
            {
                TextRenderer.DrawText(
                    pe.Graphics,
                    Text,
                    Font,
                    textRect,
                    SystemColors.ControlText);
            }
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            if (msg.Msg == Win32.WM_KEYUP)
            {
                if (m_holdingSpace)
                {
                    if ((int)msg.WParam == (int)Keys.Space)
                    {
                        OnMouseUp(null);
                        PerformClick();
                    }
                    else if ((int)msg.WParam == (int)Keys.Escape
                        || (int)msg.WParam == (int)Keys.Tab)
                    {
                        m_holdingSpace = false;
                        OnMouseUp(null);
                    }
                }
                return true;
            }
            else if (msg.Msg == Win32.WM_KEYDOWN)
            {
                if ((int)msg.WParam == (int)Keys.Space)
                {
                    m_holdingSpace = true;
                    OnMouseDown(null);
                }
                else if ((int)msg.WParam == (int)Keys.Enter)
                {
                    PerformClick();
                }
                return true;
            }
            else
                return base.PreProcessMessage(ref msg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_defaultFont != null)
                    m_defaultFont.Dispose();
                if (m_glassDownImg != null)
                    m_glassDownImg.Dispose();
                if (m_glassHotImg != null)
                    m_glassHotImg.Dispose();
                if (m_toolTip != null)
                    m_toolTip.Dispose();
            }
            m_defaultFont = null;
            m_glassDownImg = null;
            m_glassHotImg = null;
            m_toolTip = null;
            base.Dispose(disposing);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Refresh();
            base.OnTextChanged(e);
        }

        #endregion

        #region Private

        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            if (Image == null && !string.IsNullOrEmpty(Text))
            {
                textRect = new Rectangle(3, 0, Width - 6, Height);
            }
            else if (Image != null && string.IsNullOrEmpty(Text))
            {
                imageRect = new Rectangle((Width - Image.Width) / 2, (Height - Image.Height) / 2, Image.Width, Image.Height);
            }
            else if (Image != null && !string.IsNullOrEmpty(Text))
            {
                imageRect = new Rectangle(4, (Height - Image.Height) / 2, Image.Width, Image.Height);
                textRect = new Rectangle(imageRect.Right + 1, 0, Width - 4 * 2 - imageRect.Width - 1, Height);
            }
        }

        private void ShowTooTip(string toolTipText)
        {
            m_toolTip.Active = true;
            m_toolTip.SetToolTip(this, toolTipText);
        }

        private void HideToolTip()
        {
            m_toolTip.Active = false;
        }

        #endregion
    }
}

