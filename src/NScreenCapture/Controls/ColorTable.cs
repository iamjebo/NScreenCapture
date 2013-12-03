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
using NScreenCapture.Types;

namespace NScreenCapture.Controls
{
    /// <summary>
    /// 颜色选择表控件
    /// </summary>
    internal class ColorTable : Panel
    {
        #region Field

        private ColorButton[] m_colorsButtons = new ColorButton[16];   //2 x 8
        private ColorButton m_selectColorButton;
        private const byte m_offset = 1;

        #endregion

        #region Constructor

        public ColorTable()
        {
            PanelIni();
            ColorButtonsIni();
        }

        #endregion

        #region Properity

        /// <summary>
        /// ColorTable中选中的颜色
        /// </summary>
        public Color SelectColor
        {
            get { return m_selectColorButton.Color; }
            set { m_selectColorButton.Color = Color.Red; }
        }

        #endregion

        #region Override

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush sbrush = new SolidBrush(MyControlColors.BACKGROUND_COLOR))
            {
                e.Graphics.FillRectangle(sbrush, ClientRectangle);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region Private

        private void PanelIni()
        {
            this.Size = new Size(177, 37);
            this.BorderStyle = BorderStyle.None;
        }

        private void ColorButtonsIni()
        {
            m_selectColorButton = new ColorButton();
            m_selectColorButton.IsKeepHighlight = true;

            for (int i = 0; i < m_colorsButtons.Length; i++)
                m_colorsButtons[i] = new ColorButton();

            this.Controls.AddRange(m_colorsButtons);
            this.Controls.Add(m_selectColorButton);

            //location
            m_selectColorButton.Size = new Size(33, 33);
            m_selectColorButton.Location = new Point(3, 2);


            for (int i = 0; i < m_colorsButtons.Length; i++)
            {
                if (i == 0)
                    m_colorsButtons[i].Location = new Point(m_selectColorButton.Right + 3,
                                                      m_selectColorButton.Top);
                else if (i == 8)
                    m_colorsButtons[i].Location = new Point(m_colorsButtons[0].Left,
                                                     m_colorsButtons[0].Bottom + m_offset);
                else
                    m_colorsButtons[i].Location = new Point(m_colorsButtons[i - 1].Right + m_offset,
                                                     m_colorsButtons[i - 1].Top);
            }

            //Color
            m_colorsButtons[0].Color = Color.Black;
            m_colorsButtons[1].Color = Color.Gray;
            m_colorsButtons[2].Color = Color.FromArgb(128, 0, 0);
            m_colorsButtons[3].Color = Color.FromArgb(128, 128, 0);
            m_colorsButtons[4].Color = Color.FromArgb(0, 128, 0);
            m_colorsButtons[5].Color = Color.FromArgb(0, 0, 128);
            m_colorsButtons[6].Color = Color.FromArgb(128, 0, 128);
            m_colorsButtons[7].Color = Color.FromArgb(0, 128, 128);

            m_colorsButtons[8].Color = Color.White;
            m_colorsButtons[9].Color = Color.FromArgb(192, 192, 192);
            m_colorsButtons[10].Color = Color.Red;
            m_colorsButtons[11].Color = Color.Yellow;
            m_colorsButtons[12].Color = Color.FromArgb(0, 255, 0);
            m_colorsButtons[13].Color = Color.Blue;
            m_colorsButtons[14].Color = Color.FromArgb(255, 0, 255);
            m_colorsButtons[15].Color = Color.FromArgb(0, 255, 255);

            //events
            for (int i = 0; i < m_colorsButtons.Length; i++)
            {
                m_colorsButtons[i].Click += new EventHandler(OnColorButtonClick);
            }

        }

        private void OnColorButtonClick(object sender, EventArgs e)
        {
            ColorButton selectColor = sender as ColorButton;
            if (selectColor != null)
                m_selectColorButton.Color = selectColor.Color;
        }

        #endregion
    }
}
