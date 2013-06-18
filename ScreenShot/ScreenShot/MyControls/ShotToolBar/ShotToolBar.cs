using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenShot
{
    /****************************************************************
    * 
    *             Dcrp：截图工具栏控件，提供对选区绘图操作。
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *             Date: 2013-5-26
    *
    *****************************************************************/

    public partial class ShotToolBar : PictureBox
    {
        #region Field

        private GlassButton m_rectTool;
        private GlassButton m_ellipseTool;
        private GlassButton m_arrowTool;
        private GlassButton m_brushTool;
        private GlassButton m_textTool;
        private GlassButton m_undoTool;
        private GlassButton m_saveTool;
        private GlassButton m_loadImgToMSpaintTool;
        private GlassButton m_copyImgTool;
        private GlassButton m_exitShotTool;

        #endregion

        #region Properity

        public GlassButton RectTool { get { return m_rectTool; } }
        public GlassButton EllipseTool { get { return m_ellipseTool; } }
        public GlassButton ArrowTool { get { return m_arrowTool; } }
        public GlassButton BrushTool { get { return m_brushTool; } }
        public GlassButton TextTool { get { return m_textTool; } }
        public GlassButton UndoTool { get { return m_undoTool; } }
        public GlassButton SaveTool { get { return m_saveTool; } }
        public GlassButton LoadImgToMSpaintTool { get { return m_loadImgToMSpaintTool; } }
        public GlassButton CopyImgTool { get { return m_copyImgTool; } }
        public GlassButton ExitShotTool { get { return m_exitShotTool; } }

        #endregion

        #region Constructor

        public ShotToolBar()
        {
            ShotToolBarIni();
        }

        #endregion

        #region Override

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(MyControlColors.BORDER_COLOR))
            {
                using (SolidBrush sbrush = new SolidBrush(MyControlColors.BACKGROUND_COLOR))
                {
                    e.Graphics.FillRectangle(sbrush, ClientRectangle);
                    e.Graphics.DrawRectangle(pen,
                        new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1));
                }
            }
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            foreach (Control control in Controls)
            {
                GlassButton glassBtn = control as GlassButton;
                if (glassBtn != null)
                    glassBtn.Checked = false;
            }
        }

        #endregion

        #region Private

        private void ShotToolBarIni()
        {
            ToolsIni();
        }

        private void ToolsIni()
        {
            this.m_exitShotTool = new GlassButton();
            this.m_copyImgTool = new GlassButton();
            this.m_loadImgToMSpaintTool = new GlassButton();
            this.m_saveTool = new GlassButton();
            this.m_undoTool = new GlassButton();
            this.m_textTool = new GlassButton();
            this.m_brushTool = new GlassButton();
            this.m_arrowTool = new GlassButton();
            this.m_ellipseTool = new GlassButton();
            this.m_rectTool = new GlassButton();

            this.SuspendLayout();
            // 
            // exitShotTool
            // 
            this.m_exitShotTool.BackColor = System.Drawing.Color.Transparent;
            this.m_exitShotTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_exitShotTool.Location = new System.Drawing.Point(266, 3);
            this.m_exitShotTool.Name = "exitShotTool";
            this.m_exitShotTool.Size = new System.Drawing.Size(24, 24);
            this.m_exitShotTool.TabIndex = 9;
            this.m_exitShotTool.TabStop = false;
            this.m_exitShotTool.ToolTipText = "退出截图";
            this.m_exitShotTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Exit.png");

            // 
            // copyImgTool
            // 
            this.m_copyImgTool.BackColor = System.Drawing.Color.Transparent;
            this.m_copyImgTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_copyImgTool.Location = new System.Drawing.Point(237, 3);
            this.m_copyImgTool.Name = "copyImgTool";
            this.m_copyImgTool.Size = new System.Drawing.Size(24, 24);
            this.m_copyImgTool.TabIndex = 8;
            this.m_copyImgTool.TabStop = false;
            this.m_copyImgTool.ToolTipText = "复制到剪切板";
            this.m_copyImgTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.CopyImg.png");
            // 
            // loadToDrawTool
            // 
            this.m_loadImgToMSpaintTool.BackColor = System.Drawing.Color.Transparent;
            this.m_loadImgToMSpaintTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_loadImgToMSpaintTool.Location = new System.Drawing.Point(208, 3);
            this.m_loadImgToMSpaintTool.Name = "loadToDrawTool";
            this.m_loadImgToMSpaintTool.Size = new System.Drawing.Size(24, 24);
            this.m_loadImgToMSpaintTool.TabIndex = 7;
            this.m_loadImgToMSpaintTool.TabStop = false;
            this.m_loadImgToMSpaintTool.ToolTipText = "导入画图工具中编辑";
            this.m_loadImgToMSpaintTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.LoadToDrawTool.png");
            // 
            // saveTool
            // 
            this.m_saveTool.BackColor = System.Drawing.Color.Transparent;
            this.m_saveTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_saveTool.Location = new System.Drawing.Point(179, 3);
            this.m_saveTool.Name = "saveTool";
            this.m_saveTool.Size = new System.Drawing.Size(24, 24);
            this.m_saveTool.TabIndex = 6;
            this.m_saveTool.TabStop = false;
            this.m_saveTool.ToolTipText = "保存";
            this.m_saveTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Save.png");
            // 
            // undoTool
            // 
            this.m_undoTool.BackColor = System.Drawing.Color.Transparent;
            this.m_undoTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_undoTool.Location = new System.Drawing.Point(150, 3);
            this.m_undoTool.Name = "undoTool";
            this.m_undoTool.Size = new System.Drawing.Size(24, 24);
            this.m_undoTool.TabIndex = 5;
            this.m_undoTool.TabStop = false;
            this.m_undoTool.ToolTipText = "撤销编辑";
            this.m_undoTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Undo.png");
            // 
            // textTool
            // 
            this.m_textTool.BackColor = System.Drawing.Color.Transparent;
            this.m_textTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_textTool.Location = new System.Drawing.Point(121, 3);
            this.m_textTool.Name = "textTool";
            this.m_textTool.Size = new System.Drawing.Size(24, 24);
            this.m_textTool.TabIndex = 4;
            this.m_textTool.TabStop = false;
            this.m_textTool.ToolTipText = "文字工具";
            this.m_textTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Text.png");
            // 
            // brushTool
            // 
            this.m_brushTool.BackColor = System.Drawing.Color.Transparent;
            this.m_brushTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_brushTool.Location = new System.Drawing.Point(92, 3);
            this.m_brushTool.Name = "brushTool";
            this.m_brushTool.Size = new System.Drawing.Size(24, 24);
            this.m_brushTool.TabIndex = 3;
            this.m_brushTool.TabStop = false;
            this.m_brushTool.ToolTipText = "画刷工具";
            this.m_brushTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Brush.png");
            // 
            // arrowTool
            // 
            this.m_arrowTool.BackColor = System.Drawing.Color.Transparent;
            this.m_arrowTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_arrowTool.Location = new System.Drawing.Point(63, 3);
            this.m_arrowTool.Name = "arrowTool";
            this.m_arrowTool.Size = new System.Drawing.Size(24, 24);
            this.m_arrowTool.TabIndex = 2;
            this.m_arrowTool.TabStop = false;
            this.m_arrowTool.ToolTipText = "箭头工具";
            this.m_arrowTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Arrow.png");
            // 
            // ellipseTool
            // 
            this.m_ellipseTool.BackColor = System.Drawing.Color.Transparent;
            this.m_ellipseTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_ellipseTool.Location = new System.Drawing.Point(34, 3);
            this.m_ellipseTool.Name = "ellipseTool";
            this.m_ellipseTool.Size = new System.Drawing.Size(24, 24);
            this.m_ellipseTool.TabIndex = 1;
            this.m_ellipseTool.TabStop = false;
            this.m_ellipseTool.ToolTipText = "椭圆工具";
            this.m_ellipseTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Ellipse.png");
            // 
            // rectTool
            // 
            this.m_rectTool.BackColor = System.Drawing.Color.Transparent;
            this.m_rectTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.m_rectTool.Location = new System.Drawing.Point(5, 3);
            this.m_rectTool.Name = "rectTool";
            this.m_rectTool.Size = new System.Drawing.Size(24, 24);
            this.m_rectTool.TabIndex = 0;
            this.m_rectTool.TabStop = false;
            this.m_rectTool.ToolTipText = "矩形工具";
            this.m_rectTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Rectangle.png");
            // 
            // ShotToolBar
            // 
            this.ClientSize = new System.Drawing.Size(296, 30);
            this.Controls.Add(this.m_exitShotTool);
            this.Controls.Add(this.m_copyImgTool);
            this.Controls.Add(this.m_loadImgToMSpaintTool);
            this.Controls.Add(this.m_saveTool);
            this.Controls.Add(this.m_undoTool);
            this.Controls.Add(this.m_textTool);
            this.Controls.Add(this.m_brushTool);
            this.Controls.Add(this.m_arrowTool);
            this.Controls.Add(this.m_ellipseTool);
            this.Controls.Add(this.m_rectTool);
            this.BorderStyle = BorderStyle.None;
            this.Name = "ShotToolBar";
            this.ResumeLayout(false);
        }


        #endregion

        #region Hiding

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new String ImageLocation { get { return base.ImageLocation; } set { base.ImageLocation = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image ErrorImage { get { return base.ErrorImage; } set { base.ErrorImage = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image InitialImage { get { return base.InitialImage; } set { base.InitialImage = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool WaitOnLoad { get { return base.WaitOnLoad; } set { base.WaitOnLoad = value; } }
        #endregion
    }
}
