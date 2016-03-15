using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Common
{
    public class ImageShrink
    {

        #region 生成静态缩略图

        public static MemoryStream ShrinkToStream(Image imgSource, int boundWidth, int boundHeight)
        {
            //收缩为静态图片
            ImageFormat iFormat = ImageFormat.Jpeg;
            //透明格式
            if (imgSource.RawFormat.Guid == ImageFormat.Png.Guid
                || imgSource.RawFormat.Guid == ImageFormat.Icon.Guid
                || imgSource.RawFormat.Guid == ImageFormat.Gif.Guid)
            {
                iFormat = ImageFormat.Png;
            }

            Image imgResult = imgSource;
            Bitmap bm = null;
            if (imgSource.Width > boundWidth || imgSource.Height > boundHeight)
            {
                Size sz = CalThumSize(imgSource, boundWidth, boundHeight);
                int width = sz.Width;
                int height = sz.Height;
                Rectangle recResult = new Rectangle(0, 0, width, height);
                Rectangle recSource = new Rectangle(0, 0, imgSource.Width, imgSource.Height);
                bm = GetShrinkBmp(imgSource, sz);
                using (Graphics graphic = GetGraphic(bm))
                {
                    graphic.DrawImage(imgSource, recResult, recSource, GraphicsUnit.Pixel);//生成缩小图像
                }
                imgResult = bm;
            }

            MemoryStream msResult = new MemoryStream();
            imgResult.Save(msResult, iFormat);

            if (bm != null)
            {
                bm.Dispose();
            }

            msResult.Position = 0;
            return msResult;
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="imgSource"></param>
        /// <param name="boundWidth"></param>
        /// <param name="boundHeight"></param>
        /// <returns></returns>
        public static Image ShrinkImage(Image imgSource, int boundWidth, int boundHeight)
        {
            MemoryStream msResult = ShrinkToStream(imgSource, boundWidth, boundHeight);

            Image imgResult = Image.FromStream(msResult); //获取RawFormat为jpg(非透明格式时)的Image   
            msResult.Dispose();
            return imgResult;
        }
        #endregion


        #region 辅助方法

        public static Image Convert(Image imgSource, ImageFormat eImageFormat)
        {
            MemoryStream ms = new MemoryStream();
            imgSource.Save(ms, eImageFormat);
            return Image.FromStream(ms);
        }

        static Graphics GetGraphic(Image img)
        {
            Graphics graphic = Graphics.FromImage(img);
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            return graphic;
        }

        static Bitmap GetShrinkBmp(Image imgSource, Size sz)
        {
            Bitmap bm = new Bitmap(sz.Width, sz.Height, imgSource.PixelFormat);
            bm.SetResolution(imgSource.HorizontalResolution, imgSource.VerticalResolution);

            return bm;
        }



        /// <summary>
        /// 根据尺寸限制，计算缩略图的不失真尺寸
        /// </summary>
        /// <param name="image"></param>
        /// <param name="boundWidth"></param>
        /// <param name="boundHeight"></param>
        /// <returns></returns>
        static Size CalThumSize(Image image, int boundWidth, int boundHeight)
        {
            if (image.Width <= boundWidth && image.Height <= boundHeight)
            {
                return image.Size;
            }

            int resultWidth = boundWidth;
            int resultHeight = resultWidth * image.Height / image.Width;
            if (resultHeight > boundHeight)
            {
                resultHeight = boundHeight;
                resultWidth = resultHeight * image.Width / image.Height;
                if (resultWidth > boundWidth)
                {
                    resultWidth = boundWidth;
                }
            }

            Size rect = new Size(resultWidth, resultHeight);
            return rect;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion

    }
}
