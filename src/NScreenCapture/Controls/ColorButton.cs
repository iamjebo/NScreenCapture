#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using NScreenCapture.Types;

namespace NScreenCapture.Controls
{
    /// <summary>
    /// 颜色方块小控件
    /// </summary>
    internal class ColorButton : Button
    {
        #region Field

        private static readonly Color COLOR_BORDAR = Color.Black;
        private static readonly Color COLOR_INNERBORDAR = Color.White;

        private Color m_Color = Color.Red;
        private MyControlState m_State = MyControlState.Normal;
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

            if (m_State == MyControlState.Normal)
                m_State = MyControlState.Highlight;
            else
                m_State = MyControlState.Down;

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                m_State = MyControlState.Down;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (m_State == MyControlState.Down)
                m_State = MyControlState.Down;
            else
                m_State = MyControlState.Normal;

            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            m_State = MyControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (m_IsKeepHighlight)
                m_State = MyControlState.Highlight;

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            rect.Inflate(-1, -1);
            using (SolidBrush sbrush = new SolidBrush(m_Color))
            {
                g.FillRectangle(sbrush, rect);
                using (Pen pen = new Pen(COLOR_BORDAR))
                {
                    if (m_State == MyControlState.Highlight ||
                        m_State == MyControlState.Down)
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
