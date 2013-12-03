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

using NScreenCapture.Types;
using NScreenCapture.Helpers;

namespace NScreenCapture.Controls
{
    /// <summary>
    /// 带有字体大小选择的颜色表组合控件
    /// </summary>
    internal partial class ColorTableWithFont : UserControl
    {
        /// <summary> 当前用户选择的字体宽度 </summary>
        public int FontWidth
        {
            get { return Convert.ToInt32(comboBoxFontWidth.Items[comboBoxFontWidth.SelectedIndex].ToString()); }
        }

        /// <summary>当前用户选择的字体颜色</summary>
        public Color FontColor
        {
            get
            {
                return colorTable.SelectColor;
            }
        }

        public ColorTableWithFont()
        {
            InitializeComponent();
            Size size = new Size(287, 40);
            this.MinimumSize = size;
            this.Size = size;

            this.comboBoxFontWidth.SelectedIndex = 0;   //默认的宽度
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
        {//解决工具栏因子控件太多重绘时严重闪烁问题。
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

        /// <summary>重置颜色</summary>
        public void Reset()
        {
            comboBoxFontWidth.SelectedIndex = 0;
            colorTable.SelectColor = Color.Red;
        }
    }
}
