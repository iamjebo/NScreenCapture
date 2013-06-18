using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenShot
{
    /****************************************************************
  * 
  *             Dcrp：带有宽度点控件的ColorTable控件
  *           Author：曹江波
  *             Blog: http://www.cnblogs.com/Keep-Silence-/
  *             Date: 2013-6-12
  *
  *****************************************************************/

    public partial class ColorTableWithWidth : UserControl
    {

        /// <summary>
        /// 当前用户选择的线条宽度
        /// </summary>
        public DotWidth LineWidth
        {
            get 
            {
                if (widthDotBtnMin.Checked)
                    return DotWidth.Minimize;
                else if (widthDotBtnMeduim.Checked)
                    return DotWidth.Medium;
                else if (widthDotBtnMax.Checked)
                    return DotWidth.Maximize;
                else
                    return DotWidth.Minimize;
            }
        }

        /// <summary>
        /// 当前用户选择的线条颜色
        /// </summary>
        public Color LineColor
        {
            get
            {
                return colorTable.SelectColor;
            }
        }

        public ColorTableWithWidth()
        {
            InitializeComponent();
            Size size = new Size(287, 40);
            this.MinimumSize = size;
            this.Size = size;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush sbrush = new SolidBrush(MyControlColors.BACKGROUND_COLOR))
            {
                e.Graphics.FillRectangle(sbrush, ClientRectangle);
                using (Pen pen = new Pen(MyControlColors.BORDER_COLOR))
                {
                    e.Graphics.DrawRectangle(pen,
                        new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1));
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Default;
        }
    }
}
