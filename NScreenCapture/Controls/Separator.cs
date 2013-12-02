using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

using NScreenCapture.Helpers;

namespace NScreenCapture.Controls
{
    /****************************************************************
    * 
    *             Dcrp：自定义空间分割符 
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com
    *           Update: 2013-09-11
    *
    *****************************************************************/

    internal class Separator : PictureBox
    {
        private Image m_separatorImg;

        public Separator()
        {
            m_separatorImg = Properties.Resources.Separator;
            this.Width = 1;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (m_separatorImg != null)
            {
                MethodHelper.DrawImageWithNineRect(
                    pe.Graphics,
                    m_separatorImg,
                    ClientRectangle,
                    new Rectangle(Point.Empty, m_separatorImg.Size));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                m_separatorImg.Dispose();

            base.Dispose(disposing);
        }
    }
}
