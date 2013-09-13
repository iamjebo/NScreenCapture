using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ScreenCapture.Helpers
{

    /****************************************************************
    * 
    *             Dcrp：静态方法帮助类，封装了常用的方法。
    *           Author：曹江波
    *             Blog: http://www.cnblogs.com/Keep-Silence-/
    *           E-mail: caojiangbocn@gmail.com             
    *             Date: 2013-5-18
    *
    *****************************************************************/

    internal class MethodHelper
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

        /// <summary>
        /// 利用九宫图绘制图像
        /// </summary>
        /// <param name="g">绘图对象</param>
        /// <param name="img">所需绘制的图片</param>
        /// <param name="targetRect">目标矩形</param>
        /// <param name="srcRect">来源矩形</param>
        public static void DrawImageWithNineRect(Graphics g, Image img, Rectangle targetRect, Rectangle srcRect)
        {
            int offset = 5;
            Rectangle NineRect = new Rectangle(img.Width / 2 - offset, img.Height / 2 - offset, 2 * offset, 2 * offset);
            int x = 0, y = 0, nWidth, nHeight;
            int xSrc = 0, ySrc = 0, nSrcWidth, nSrcHeight;
            int nDestWidth, nDestHeight;
            nDestWidth = targetRect.Width;
            nDestHeight = targetRect.Height;
            // 左上-------------------------------------;
            x = targetRect.Left;
            y = targetRect.Top;
            nWidth = NineRect.Left - srcRect.Left;
            nHeight = NineRect.Top - srcRect.Top;
            xSrc = srcRect.Left;
            ySrc = srcRect.Top;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
            // 上-------------------------------------;
            x = targetRect.Left + NineRect.Left - srcRect.Left;
            nWidth = nDestWidth - nWidth - (srcRect.Right - NineRect.Right);
            xSrc = NineRect.Left;
            nSrcWidth = NineRect.Right - NineRect.Left;
            nSrcHeight = NineRect.Top - srcRect.Top;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);
            // 右上-------------------------------------;
            x = targetRect.Right - (srcRect.Right - NineRect.Right);
            nWidth = srcRect.Right - NineRect.Right;
            xSrc = NineRect.Right;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
            // 左-------------------------------------;
            x = targetRect.Left;
            y = targetRect.Top + NineRect.Top - srcRect.Top;
            nWidth = NineRect.Left - srcRect.Left;
            nHeight = targetRect.Bottom - y - (srcRect.Bottom - NineRect.Bottom);
            xSrc = srcRect.Left;
            ySrc = NineRect.Top;
            nSrcWidth = NineRect.Left - srcRect.Left;
            nSrcHeight = NineRect.Bottom - NineRect.Top;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);
            // 中-------------------------------------;
            x = targetRect.Left + NineRect.Left - srcRect.Left;
            nWidth = nDestWidth - nWidth - (srcRect.Right - NineRect.Right);
            xSrc = NineRect.Left;
            nSrcWidth = NineRect.Right - NineRect.Left;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);

            // 右-------------------------------------;
            x = targetRect.Right - (srcRect.Right - NineRect.Right);
            nWidth = srcRect.Right - NineRect.Right;
            xSrc = NineRect.Right;
            nSrcWidth = srcRect.Right - NineRect.Right;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);

            // 左下-------------------------------------;
            x = targetRect.Left;
            y = targetRect.Bottom - (srcRect.Bottom - NineRect.Bottom);
            nWidth = NineRect.Left - srcRect.Left;
            nHeight = srcRect.Bottom - NineRect.Bottom;
            xSrc = srcRect.Left;
            ySrc = NineRect.Bottom;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
            // 下-------------------------------------;
            x = targetRect.Left + NineRect.Left - srcRect.Left;
            nWidth = nDestWidth - nWidth - (srcRect.Right - NineRect.Right);
            xSrc = NineRect.Left;
            nSrcWidth = NineRect.Right - NineRect.Left;
            nSrcHeight = srcRect.Bottom - NineRect.Bottom;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);
            // 右下-------------------------------------;
            x = targetRect.Right - (srcRect.Right - NineRect.Right);
            nWidth = srcRect.Right - NineRect.Right;
            xSrc = NineRect.Right;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
        }

    }
}
