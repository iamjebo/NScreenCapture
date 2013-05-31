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
    /// <summary>
    /// 截图工具栏
    /// </summary>
    public partial class ShotToolBar : PictureBox
    {
        #region Field

        private static readonly Color TOOLBAR_BORDER_COLOR = Color.FromArgb(200, 78, 153, 210);
        private static readonly Color TOOLBAR_BACKGROUND_COLOR = Color.FromArgb(200, 222, 238, 255);

        private object EventObject_Rect = new object();
        private object EventObject_Ellipse = new object();
        private object EventObject_Arrow = new object();
        private object EventObject_Brush = new object();
        private object EventObject_Text = new object();
        private object EventObject_Undo = new object();
        private object EventObject_Save = new object();
        private object EventObject_LoadImgToMSpaint = new object();
        private object EventObject_CopyImg = new object();
        private object EventObject_Exit = new object();

        private GlassButton rectTool;
        private GlassButton ellipseTool;
        private GlassButton arrowTool;
        private GlassButton brushTool;
        private GlassButton textTool;
        private GlassButton undoTool;
        private GlassButton saveTool;
        private GlassButton loadImgToMSpaintTool;
        private GlassButton copyImgTool;
        private GlassButton exitShotTool;

        #endregion

        #region Constructor

        public ShotToolBar()
        {
            ShotToolBarIni();
        }

        #endregion

        #region Event

        public event EventHandler RectToolClick
        {
            add { base.Events.AddHandler(EventObject_Rect, value); }
            remove { base.Events.RemoveHandler(EventObject_Rect, value); }
        }

        public event EventHandler EllipseToolClick
        {
            add { base.Events.AddHandler(EventObject_Ellipse, value); }
            remove { base.Events.RemoveHandler(EventObject_Ellipse, value); }
        }

        public event EventHandler ArrowToolClick
        {
            add { base.Events.AddHandler(EventObject_Arrow, value); }
            remove { base.Events.RemoveHandler(EventObject_Arrow, value); }
        }

        public event EventHandler BrushToolClick
        {
            add { base.Events.AddHandler(EventObject_Brush, value); }
            remove { base.Events.RemoveHandler(EventObject_Brush, value); }
        }

        public event EventHandler TextToolClick
        {
            add { base.Events.AddHandler(EventObject_Text, value); }
            remove { base.Events.RemoveHandler(EventObject_Text, value); }
        }

        public event EventHandler UndoToolClick
        {
            add { base.Events.AddHandler(EventObject_Undo, value); }
            remove { base.Events.RemoveHandler(EventObject_Undo, value); }
        }

        public event EventHandler SaveToolClick
        {
            add { base.Events.AddHandler(EventObject_Save, value); }
            remove { base.Events.RemoveHandler(EventObject_Save, value); }
        }

        public event EventHandler LoadImgToMSpaintToolClick
        {
            add { base.Events.AddHandler(EventObject_LoadImgToMSpaint, value); }
            remove { base.Events.RemoveHandler(EventObject_LoadImgToMSpaint, value); }
        }

        public event EventHandler CopyImgToolClick
        {
            add { base.Events.AddHandler(EventObject_CopyImg, value); }
            remove { base.Events.RemoveHandler(EventObject_CopyImg, value); }
        }

        public event EventHandler ExitToolClick
        {
            add { base.Events.AddHandler(EventObject_Exit, value); }
            remove { base.Events.RemoveHandler(EventObject_Exit, value); }
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
            using (Pen pen = new Pen(TOOLBAR_BORDER_COLOR))
            {
                using (SolidBrush sbrush = new SolidBrush(TOOLBAR_BACKGROUND_COLOR))
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

        #endregion

        #region Private

        private void ShotToolBarIni()
        {
            ToolsIni();
            ToolsEventIni();
        }

        private void ToolsIni()
        {
            this.exitShotTool = new ScreenShot.GlassButton();
            this.copyImgTool = new ScreenShot.GlassButton();
            this.loadImgToMSpaintTool = new ScreenShot.GlassButton();
            this.saveTool = new ScreenShot.GlassButton();
            this.undoTool = new ScreenShot.GlassButton();
            this.textTool = new ScreenShot.GlassButton();
            this.brushTool = new ScreenShot.GlassButton();
            this.arrowTool = new ScreenShot.GlassButton();
            this.ellipseTool = new ScreenShot.GlassButton();
            this.rectTool = new ScreenShot.GlassButton();

            this.SuspendLayout();
            // 
            // exitShotTool
            // 
            this.exitShotTool.BackColor = System.Drawing.Color.Transparent;
            this.exitShotTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.exitShotTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.exitShotTool.Location = new System.Drawing.Point(266, 3);
            this.exitShotTool.Name = "exitShotTool";
            this.exitShotTool.Size = new System.Drawing.Size(24, 24);
            this.exitShotTool.TabIndex = 9;
            this.exitShotTool.TabStop = false;
            this.exitShotTool.ToolTipText = "退出截图";
            this.exitShotTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Exit.png");

            // 
            // copyImgTool
            // 
            this.copyImgTool.BackColor = System.Drawing.Color.Transparent;
            this.copyImgTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.copyImgTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.copyImgTool.Location = new System.Drawing.Point(237, 3);
            this.copyImgTool.Name = "copyImgTool";
            this.copyImgTool.Size = new System.Drawing.Size(24, 24);
            this.copyImgTool.TabIndex = 8;
            this.copyImgTool.TabStop = false;
            this.copyImgTool.ToolTipText = "复制到剪切板";
            this.copyImgTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.CopyImg.png");
            // 
            // loadToDrawTool
            // 
            this.loadImgToMSpaintTool.BackColor = System.Drawing.Color.Transparent;
            this.loadImgToMSpaintTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.loadImgToMSpaintTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.loadImgToMSpaintTool.Location = new System.Drawing.Point(208, 3);
            this.loadImgToMSpaintTool.Name = "loadToDrawTool";
            this.loadImgToMSpaintTool.Size = new System.Drawing.Size(24, 24);
            this.loadImgToMSpaintTool.TabIndex = 7;
            this.loadImgToMSpaintTool.TabStop = false;
            this.loadImgToMSpaintTool.ToolTipText = "导入画图工具中编辑";
            this.loadImgToMSpaintTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.LoadToDrawTool.png");
            // 
            // saveTool
            // 
            this.saveTool.BackColor = System.Drawing.Color.Transparent;
            this.saveTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.saveTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.saveTool.Location = new System.Drawing.Point(179, 3);
            this.saveTool.Name = "saveTool";
            this.saveTool.Size = new System.Drawing.Size(24, 24);
            this.saveTool.TabIndex = 6;
            this.saveTool.TabStop = false;
            this.saveTool.ToolTipText = "保存";
            this.saveTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Save.png");
            // 
            // undoTool
            // 
            this.undoTool.BackColor = System.Drawing.Color.Transparent;
            this.undoTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.undoTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.undoTool.Location = new System.Drawing.Point(150, 3);
            this.undoTool.Name = "undoTool";
            this.undoTool.Size = new System.Drawing.Size(24, 24);
            this.undoTool.TabIndex = 5;
            this.undoTool.TabStop = false;
            this.undoTool.ToolTipText = "撤销编辑";
            this.undoTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Undo.png");
            // 
            // textTool
            // 
            this.textTool.BackColor = System.Drawing.Color.Transparent;
            this.textTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.textTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.textTool.Location = new System.Drawing.Point(121, 3);
            this.textTool.Name = "textTool";
            this.textTool.Size = new System.Drawing.Size(24, 24);
            this.textTool.TabIndex = 4;
            this.textTool.TabStop = false;
            this.textTool.ToolTipText = "文字工具";
            this.textTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Text.png");
            // 
            // brushTool
            // 
            this.brushTool.BackColor = System.Drawing.Color.Transparent;
            this.brushTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.brushTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.brushTool.Location = new System.Drawing.Point(92, 3);
            this.brushTool.Name = "brushTool";
            this.brushTool.Size = new System.Drawing.Size(24, 24);
            this.brushTool.TabIndex = 3;
            this.brushTool.TabStop = false;
            this.brushTool.ToolTipText = "画刷工具";
            this.brushTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Brush.png");
            // 
            // arrowTool
            // 
            this.arrowTool.BackColor = System.Drawing.Color.Transparent;
            this.arrowTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.arrowTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.arrowTool.Location = new System.Drawing.Point(63, 3);
            this.arrowTool.Name = "arrowTool";
            this.arrowTool.Size = new System.Drawing.Size(24, 24);
            this.arrowTool.TabIndex = 2;
            this.arrowTool.TabStop = false;
            this.arrowTool.ToolTipText = "箭头工具";
            this.arrowTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Arrow.png");
            // 
            // ellipseTool
            // 
            this.ellipseTool.BackColor = System.Drawing.Color.Transparent;
            this.ellipseTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ellipseTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.ellipseTool.Location = new System.Drawing.Point(34, 3);
            this.ellipseTool.Name = "ellipseTool";
            this.ellipseTool.Size = new System.Drawing.Size(24, 24);
            this.ellipseTool.TabIndex = 1;
            this.ellipseTool.TabStop = false;
            this.ellipseTool.ToolTipText = "椭圆工具";
            this.ellipseTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Ellipse.png");
            // 
            // rectTool
            // 
            this.rectTool.BackColor = System.Drawing.Color.Transparent;
            this.rectTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.rectTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.rectTool.Location = new System.Drawing.Point(5, 3);
            this.rectTool.Name = "rectTool";
            this.rectTool.Size = new System.Drawing.Size(24, 24);
            this.rectTool.TabIndex = 0;
            this.rectTool.TabStop = false;
            this.rectTool.ToolTipText = "矩形工具";
            this.rectTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.Rectangle.png");
            // 
            // ShotToolBar
            // 
            this.ClientSize = new System.Drawing.Size(296, 30);
            this.Controls.Add(this.exitShotTool);
            this.Controls.Add(this.copyImgTool);
            this.Controls.Add(this.loadImgToMSpaintTool);
            this.Controls.Add(this.saveTool);
            this.Controls.Add(this.undoTool);
            this.Controls.Add(this.textTool);
            this.Controls.Add(this.brushTool);
            this.Controls.Add(this.arrowTool);
            this.Controls.Add(this.ellipseTool);
            this.Controls.Add(this.rectTool);
            this.BorderStyle = BorderStyle.None;
            this.Name = "ShotToolBar";
            this.ResumeLayout(false);
        }

        private void ToolsEventIni()
        {
            rectTool.Click += new EventHandler(OnRectToolClick);
            ellipseTool.Click += new EventHandler(OnEllipseToolClick);
            arrowTool.Click += new EventHandler(OnArrowToolClick);
            brushTool.Click += new EventHandler(OnBrushToolClick);
            textTool.Click += new EventHandler(OnTextToolClick);
            undoTool.Click += new EventHandler(OnUndoToolClick);
            saveTool.Click += new EventHandler(OnSaveToolClick);
            loadImgToMSpaintTool.Click += new EventHandler(OnLoadImgToMSpaintToolClick);
            copyImgTool.Click += new EventHandler(OnCopyImgToolClick);
            exitShotTool.Click += new EventHandler(OnExitToolClick);
        }

        private void OnRectToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Rect] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnEllipseToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Ellipse] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnArrowToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Arrow] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnBrushToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Brush] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnTextToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Text] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnUndoToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Undo] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnSaveToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Save] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnLoadImgToMSpaintToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_LoadImgToMSpaint] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        private void OnCopyImgToolClick(object sender, EventArgs e)
        {
            EventHandler hander = base.Events[EventObject_CopyImg] as EventHandler;
            if (hander != null)
                hander(this, e);
        }

        private void OnExitToolClick(object sender, EventArgs e)
        {
            EventHandler handler = base.Events[EventObject_Exit] as EventHandler;
            if (handler != null)
                handler(this, e);
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
