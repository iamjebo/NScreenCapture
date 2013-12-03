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
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace NScreenCapture.Helpers
{
    /// <summary>
    /// 图像操作帮助类，封装了常用的方法。
    /// </summary>
    internal class ImageHelper
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
        /// 将图片按照指定的倍数放大
        /// </summary>
        /// <param name="bmpSrc">需要放大的图片</param>
        /// <param name="times">放到的倍数</param>
        public static Bitmap MagnifyImage(Bitmap bmpSrc, int times)
        {
            Bitmap bmpNew = new Bitmap(bmpSrc.Width * times, bmpSrc.Height * times, PixelFormat.Format32bppArgb);
            BitmapData bmpSrcData = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmpNewData = bmpNew.LockBits(new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] bySrcData = new byte[bmpSrcData.Height * bmpSrcData.Stride];
            Marshal.Copy(bmpSrcData.Scan0, bySrcData, 0, bySrcData.Length);
            byte[] byNewData = new byte[bmpNewData.Height * bmpNewData.Stride];
            Marshal.Copy(bmpNewData.Scan0, byNewData, 0, byNewData.Length);

            for (int y = 0, lenY = bmpSrc.Height; y < lenY; y++)
            {
                for (int x = 0, lenX = bmpSrc.Width; x < lenX; x++)
                {
                    for (int cy = 0; cy < times; cy++)
                    {
                        for (int cx = 0; cx < times; cx++)
                        {
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride)] = bySrcData[x * 4 + y * bmpSrcData.Stride];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 1] = bySrcData[x * 4 + y * bmpSrcData.Stride + 1];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 2] = bySrcData[x * 4 + y * bmpSrcData.Stride + 2];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 3] = bySrcData[x * 4 + y * bmpSrcData.Stride + 3];
                        }
                    }
                }
            }
            Marshal.Copy(byNewData, 0, bmpNewData.Scan0, byNewData.Length);
            bmpSrc.UnlockBits(bmpSrcData);
            bmpNew.UnlockBits(bmpNewData);
            return bmpNew;
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
