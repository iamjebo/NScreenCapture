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
        private static readonly Color TOOLBAR_BORDER_COLOR = Color.FromArgb(200, 78, 153, 210);
        private static readonly Color TOOLBAR_BACKGROUND_COLOR = Color.FromArgb(200, 222, 238, 255);

        private GlassButton rectTool;
        private GlassButton ellipseTool;
        private GlassButton arrowTool;
        private GlassButton brushTool;
        private GlassButton textTool;
        private GlassButton undoTool;
        private GlassButton saveTool;
        private GlassButton loadToDrawTool;
        private GlassButton copyImgTool;
        private GlassButton exitShotTool;

        public ShotToolBar()
        {
            ShotToolBarIni();
        }

        private void ShotToolBarIni()
        {
            this.exitShotTool = new ScreenShot.GlassButton();
            this.copyImgTool = new ScreenShot.GlassButton();
            this.loadToDrawTool = new ScreenShot.GlassButton();
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
            this.loadToDrawTool.BackColor = System.Drawing.Color.Transparent;
            this.loadToDrawTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.loadToDrawTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.loadToDrawTool.Location = new System.Drawing.Point(208, 3);
            this.loadToDrawTool.Name = "loadToDrawTool";
            this.loadToDrawTool.Size = new System.Drawing.Size(24, 24);
            this.loadToDrawTool.TabIndex = 7;
            this.loadToDrawTool.TabStop = false;
            this.loadToDrawTool.ToolTipText = "导入画图工具中编辑";
            this.loadToDrawTool.Image = MethodHelper.GetImageFormResourceStream("MyControls.ShotToolBar.Res.LoadToDrawTool.png");
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
            this.Controls.Add(this.loadToDrawTool);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(TOOLBAR_BORDER_COLOR))
            {
                using (SolidBrush sbrush = new SolidBrush(TOOLBAR_BACKGROUND_COLOR))
                {
                    e.Graphics.FillRectangle(sbrush, ClientRectangle);
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1));
                }
            }
        }
    }
}
