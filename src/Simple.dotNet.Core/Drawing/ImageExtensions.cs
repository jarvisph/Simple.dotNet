using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Simple.dotNet.Core.Drawing
{
    public static class ImageExtensions
    {
        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="image"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Image DrawBoundingBox(this Image image, float top, float bottom, float left, float right)
        {
            float _top = image.Height * top / 600;
            float _bottom = image.Height * bottom / 600;
            float _left = image.Width * left / 800;
            float _right = image.Width * right / 800;
            float x = _left;
            float y = _top;
            float width = _right - _left;
            float height = _bottom - _top;
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Pen pen = new Pen(Color.Red, 3.2f);
                graphics.DrawRectangle(pen, x, y, width, height);
            }
            return image;
        }
    }
}
