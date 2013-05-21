using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Reflection;

namespace ScreenShot
{
    public class MethodHelper
    {
        /// <summary>
        /// 获取图片指定坐标的颜色
        /// </summary>
        public static Color GetColor(Image scrimg ,int x, int y)
        {
            using (Bitmap bmp = new Bitmap(scrimg))
            {
                Color color = bmp.GetPixel(x,y);
                return color;
            }
        }
    }
}
