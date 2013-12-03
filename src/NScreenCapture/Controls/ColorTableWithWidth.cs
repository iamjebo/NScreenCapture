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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NScreenCapture.Helpers;
using NScreenCapture.Types;

namespace NScreenCapture.Controls
{
    /// <summary>
    /// 带有宽度点控件的颜色表组合控件
    /// </summary>
    internal partial class ColorTableWithWidth : UserControl
    {
        /// <summary>当前用户选择的线条宽度</summary>
        public DotWidth LineWidth
        {
            get
            {
                if (widthDotBtnMin.Checked)
                    return DotWidth.Minimize;
                else if (widthDotBtnMeduim.Checked)
                    return DotWidth.Medium;
                else
                    return DotWidth.Maximize;
            }
        }

        /// <summary>当前用户选择的线条颜色</summary>
        public Color LineColor { get { return colorTable.SelectColor; } }

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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!DesignMode)
                {
                    cp.ExStyle |= Win32.WS_CLIPCHILDREN;
                }
                return cp;
            }
        }

        /// <summary>重置控件颜色</summary>
        public void Reset()
        {
            widthDotBtnMin.Checked = true;
            colorTable.SelectColor = Color.Red;
        }
    }
}
