using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace ScreenShot
{

    /****************************************************************
    * 
    *             Dcrp：ScreenShot主窗体 工具栏事件处理部分
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *             Update: 2013-6-13
    *
    *****************************************************************/

    public partial class ScreenShotForm
    {
        private const byte YOFFSET_TOOLBAR_COLORTABLE = 3;      //ColorTable ColorTableWithFont等控件与工具栏垂直的距离
        private DrawStyle m_drawStyle = DrawStyle.None;         //绘制类型

        /* 避免当显示ColorTable等控件时其可见部分超过屏幕最底端 */
        private void UpdateToolBarLocation()
        {
            Point location = Point.Empty;
            int colorTableHeight = 40;                  //ColorTable等控件的高度都为40
            int yoffset_ColorTable_ScreenBottm = 3;     //ColorTable等控件底部与屏幕底端的距离
            int yoffset_Toolbar_SelectRect = 5;         //ToolBar 与选区的垂直距离

            if (this.Height - shotToolBar.Bottom < YOFFSET_TOOLBAR_COLORTABLE + colorTableHeight + yoffset_ColorTable_ScreenBottm)
                location = new Point(m_selectedRect.Right - shotToolBar.Width,
                                     m_selectedRect.Top - shotToolBar.Height - yoffset_Toolbar_SelectRect -
                                     yoffset_ColorTable_ScreenBottm - colorTableHeight);
            else
                location = new Point(m_selectedRect.Right - shotToolBar.Width,
                                     m_selectedRect.Bottom + yoffset_Toolbar_SelectRect);

            shotToolBar.Location = location;
        }

        private void ToolBarEventsIni()
        {
            shotToolBar.RectTool.Click += new EventHandler(OnRectToolClick);
            shotToolBar.EllipseTool.Click += new EventHandler(OnEllipseToolClick);
            shotToolBar.ArrowTool.Click += new EventHandler(OnArrowToolClick);
            shotToolBar.BrushTool.Click += new EventHandler(OnBrushToolClick);
            shotToolBar.TextTool.Click += new EventHandler(OnTextToolClick);
            shotToolBar.UndoTool.Click += new EventHandler(OnUndoToolClick);
            shotToolBar.SaveTool.Click += new EventHandler(OnSaveToolClick);
            shotToolBar.LoadImgToMSpaintTool.Click += new EventHandler(OnLoadImgToMSpaintToolClick);
            shotToolBar.CopyImgTool.Click += new EventHandler(OnCopyImgToolClick);
            shotToolBar.ExitShotTool.Click += new EventHandler(OnExitToolClick);
        }

        private void OnRectToolClick(object sender, EventArgs e)
        {
            if (shotToolBar.RectTool.IsDown)    //绘制矩形按钮处于未按下状态
            {
                m_shotState = ShotState.DrawInRect;
                m_drawStyle = DrawStyle.DrawRectangle;

                UpdateToolBarLocation();
                colorTableWithWidth.Location = new Point(shotToolBar.Left,
                                                         shotToolBar.Bottom + YOFFSET_TOOLBAR_COLORTABLE);
                colorTableWithWidth.Visible = true;
                colorTable.Visible = false;
                colorTableWithFont.Visible = false;
            }
            else
            {
                m_shotState = ShotState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void OnEllipseToolClick(object sender, EventArgs e)
        {
            if (shotToolBar.EllipseTool.IsDown)
            {
                m_shotState = ShotState.DrawInRect;
                m_drawStyle = DrawStyle.DrawEllipse;

                UpdateToolBarLocation();
                colorTableWithWidth.Location = new Point(shotToolBar.Left,
                                                         shotToolBar.Bottom + YOFFSET_TOOLBAR_COLORTABLE);
                colorTableWithWidth.Visible = true;
                colorTableWithFont.Visible = false;
                colorTable.Visible = false;
            }
            else
            {
                m_shotState = ShotState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void OnArrowToolClick(object sender, EventArgs e)
        {
            if (shotToolBar.ArrowTool.IsDown)
            {
                m_shotState = ShotState.DrawInRect;
                m_drawStyle = DrawStyle.DrawArrow;

                UpdateToolBarLocation();
                colorTable.Location = new Point(shotToolBar.Left,
                                                shotToolBar.Bottom + YOFFSET_TOOLBAR_COLORTABLE);
                colorTable.Visible = true;
                colorTableWithWidth.Visible = false;
                colorTableWithFont.Visible = false;

            }
            else
            {
                m_shotState = ShotState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTable.Visible = false;
            }
        }

        private void OnBrushToolClick(object sender, EventArgs e)
        {
            if (shotToolBar.BrushTool.IsDown)
            {
                m_shotState = ShotState.DrawInRect;
                m_drawStyle = DrawStyle.DrawBrush;

                UpdateToolBarLocation();
                colorTableWithWidth.Location = new Point(shotToolBar.Left,
                                                         shotToolBar.Bottom + YOFFSET_TOOLBAR_COLORTABLE);
                colorTableWithWidth.Visible = true;
                colorTableWithFont.Visible = false;
                colorTable.Visible = false;
            }
            else
            {
                m_shotState = ShotState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithWidth.Visible = false;
            }
        }

        private void OnTextToolClick(object sender, EventArgs e)
        {
            if (shotToolBar.TextTool.IsDown)
            {
                m_shotState = ShotState.DrawInRect;
                m_drawStyle = DrawStyle.DrawText;

                UpdateToolBarLocation();
                colorTableWithFont.Location = new Point(shotToolBar.Left,
                                                shotToolBar.Bottom + YOFFSET_TOOLBAR_COLORTABLE);
                colorTableWithFont.Visible = true;
                colorTableWithWidth.Visible = false;
                colorTable.Visible = false;

            }
            else
            {
                m_shotState = ShotState.FinishedRect;
                m_drawStyle = DrawStyle.None;
                colorTableWithFont.Visible = false;
            }
        }

        private void OnUndoToolClick(object sender, EventArgs e)
        {
            ClearScrren();
        }

        private void OnSaveToolClick(object sender, EventArgs e)
        {
            colorTable.Visible = false;
            colorTableWithWidth.Visible = false;
            colorTableWithFont.Visible = false;

            bool isSucessed = SaveSelectImage();
            if (isSucessed)
            {
                this.Close();
            }
            else
            {
                m_drawStyle = DrawStyle.None;
                m_shotState = ShotState.FinishedRect;
            }
        }

        private void OnLoadImgToMSpaintToolClick(object sender, EventArgs e)
        {
            string tempDir = Environment.GetEnvironmentVariable("TEMP");
            string mspaintDir = Environment.SystemDirectory + @"\mspaint.exe";
            if (Directory.Exists(tempDir) && File.Exists(mspaintDir))
            {
                m_selectImg = GetSelectImage();
                string imgPath = tempDir + @"\WrysmileTemp.bmp";
                m_selectImg.Save(imgPath);
                Process.Start(mspaintDir, imgPath);
                this.Close();
            }
            else
                MessageBox.Show("Load selected image to mspaint.exe faild.");
        }

        private void OnCopyImgToolClick(object sender, EventArgs e)
        {
            m_selectImg = GetSelectImage();
            if (m_selectImg != null)
            {
                Clipboard.SetDataObject(m_selectImg, true);
                this.Close();
            }
        }

        private void OnExitToolClick(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
