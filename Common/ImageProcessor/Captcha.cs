using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Common
{
    public class Captcha
    {
        const string combination = "23457ACDEFGHJKLMNPQRSTUVWXYZacdefghijkmnprstuvwxyz";
        int m_nHeight = 30;
        int m_nWidth = 100;
       
        public Captcha()
        {

        }
        public Captcha(int nWidth, int nHeight)
        {
            m_nWidth = nWidth;
            m_nHeight = nHeight;

        }

        /// <summary>
        /// 生成验证码字符串
        /// </summary>
        public static string GetString()
        {
            return GetString(6);
        }
        /// <summary>
        /// 生成验证码字符串
        /// </summary>
        /// <param name="nLength">长度</param>     
        public static string GetString(int nLength)
        {
            Random random = new Random();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nLength; i++)
                sb.Append(combination[random.Next(combination.Length)]);
            return sb.ToString();
        }

        /// <summary>
        /// 根据字符串，生成验证码图片
        /// </summary>
        /// <param name="checkcode">验证码</param>
        public Image GetImage(string checkcode)
        {
            Random random = new Random();

            Font font = new Font("Comic Sans MS", 18, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Bitmap bitmap = new Bitmap(m_nWidth, m_nHeight, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                ///画图片的背景噪音线
                for (int i = 0; i < 5; i++)
                {
                    int x1 = random.Next(bitmap.Width);
                    int x2 = random.Next(bitmap.Width);
                    int y1 = random.Next(bitmap.Height);
                    int y2 = random.Next(bitmap.Height);
                    graphics.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                graphics.DrawString(checkcode, font, Brushes.Black, new RectangleF(0f, 0f, (float)bitmap.Width, (float)bitmap.Height), stringFormat);

                ///画图片的前景噪音点
                for (int i = 0; i < 20; i++)
                {
                    int x = random.Next(bitmap.Width);
                    int y = random.Next(bitmap.Height);

                    bitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
            }
            bitmap = WaveDistortion(bitmap);

            return bitmap;
        }

        /// <summary>
        /// 波纹扭曲
        /// </summary>
        private static Bitmap WaveDistortion(Bitmap bitmap)
        {
            Random rnd = new Random();

            var width = bitmap.Width;
            var height = bitmap.Height;

            Bitmap destBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            {
                Color foreColor = Color.FromArgb(rnd.Next(10, 100), rnd.Next(10, 100), rnd.Next(10, 100));
                Color backColor = Color.FromArgb(rnd.Next(200, 250), rnd.Next(200, 250), rnd.Next(200, 250));

                using (Graphics g = Graphics.FromImage(destBmp))
                {
                    g.Clear(backColor);

                    // periods 时间
                    double rand1 = rnd.Next(710000, 1200000) / 10000000.0;
                    double rand2 = rnd.Next(710000, 1200000) / 10000000.0;
                    double rand3 = rnd.Next(710000, 1200000) / 10000000.0;
                    double rand4 = rnd.Next(710000, 1200000) / 10000000.0;

                    // phases  相位
                    double rand5 = rnd.Next(0, 31415926) / 10000000.0;
                    double rand6 = rnd.Next(0, 31415926) / 10000000.0;
                    double rand7 = rnd.Next(0, 31415926) / 10000000.0;
                    double rand8 = rnd.Next(0, 31415926) / 10000000.0;

                    // amplitudes 振幅
                    double rand9 = rnd.Next(330, 420) / 110.0;
                    double rand10 = rnd.Next(330, 450) / 110.0;
                    double amplitudesFactor = rnd.Next(5, 6) / 12.0;//振幅小点防止出界
                    double center = width / 2.0;

                    //wave distortion 波纹扭曲
                    BitmapData destData = destBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, destBmp.PixelFormat);
                    BitmapData srcData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            var sx = x + (Math.Sin(x * rand1 + rand5)
                                        + Math.Sin(y * rand2 + rand6)) * rand9 - width / 2 + center + 1;
                            var sy = y + (Math.Sin(x * rand3 + rand7)
                                        + Math.Sin(y * rand4 + rand8)) * rand10 * amplitudesFactor;

                            int color, color_x, color_y, color_xy;
                            Color overColor = Color.Empty;

                            if (sx < 0 || sy < 0 || sx >= width - 1 || sy >= height - 1)
                            {
                                continue;
                            }
                            else
                            {
                                color = BitmapDataColorAt(srcData, (int)sx, (int)sy).B;
                                color_x = BitmapDataColorAt(srcData, (int)(sx + 1), (int)sy).B;
                                color_y = BitmapDataColorAt(srcData, (int)sx, (int)(sy + 1)).B;
                                color_xy = BitmapDataColorAt(srcData, (int)(sx + 1), (int)(sy + 1)).B;
                            }

                            if (color == 255 && color_x == 255 && color_y == 255 && color_xy == 255)
                            {
                                continue;
                            }
                            else if (color == 0 && color_x == 0 && color_y == 0 && color_xy == 0)
                            {
                                overColor = Color.FromArgb(foreColor.R, foreColor.G, foreColor.B);
                            }

                            else
                            {
                                double frsx = sx - Math.Floor(sx);
                                double frsy = sy - Math.Floor(sy);
                                double frsx1 = 1 - frsx;
                                double frsy1 = 1 - frsy;

                                double newColor =
                                     color * frsx1 * frsy1 +
                                     color_x * frsx * frsy1 +
                                     color_y * frsx1 * frsy +
                                     color_xy * frsx * frsy;

                                if (newColor > 255) newColor = 255;
                                newColor = newColor / 255;
                                double newcolor0 = 1 - newColor;

                                int newred = Math.Min((int)(newcolor0 * foreColor.R + newColor * backColor.R), 255);
                                int newgreen = Math.Min((int)(newcolor0 * foreColor.G + newColor * backColor.G), 255);
                                int newblue = Math.Min((int)(newcolor0 * foreColor.B + newColor * backColor.B), 255);
                                overColor = Color.FromArgb(newred, newgreen, newblue);
                            }
                            BitmapDataColorSet(destData, x, y, overColor);
                        }
                    }
                    destBmp.UnlockBits(destData);
                    bitmap.UnlockBits(srcData);
                }
                if (bitmap != null)
                    bitmap.Dispose();
            }
            return destBmp;
        }

        /// <summary>
        /// 获得 BitmapData 指定坐标的颜色信息
        /// </summary>
        /// <param name="srcData">从图像数据获得颜色 必须为 PixelFormat.Format24bppRgb 格式图像数据</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x,y 坐标的颜色数据</returns>
        /// <remarks>
        /// Format24BppRgb 已知X，Y坐标，像素第一个元素的位置为Scan0+(Y*Stride)+(X*3)。
        /// 这是blue字节的位置，接下来的2个字节分别含有green、red数据。
        /// </remarks>
        private static Color BitmapDataColorAt(BitmapData srcData, int x, int y)
        {
            byte[] rgbValues = new byte[3];
            Marshal.Copy((IntPtr)((int)srcData.Scan0 + ((y * srcData.Stride) + (x * 3))), rgbValues, 0, 3);
            return Color.FromArgb(rgbValues[2], rgbValues[1], rgbValues[0]);
        }

        /// <summary>
        /// 设置 BitmapData 指定坐标的颜色信息
        /// </summary>
        /// <param name="destData">设置图像数据的颜色 必须为 PixelFormat.Format24bppRgb 格式图像数据</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">待设置颜色</param>
        /// <remarks>
        /// Format24BppRgb 已知X，Y坐标，像素第一个元素的位置为Scan0+(Y*Stride)+(X*3)。
        /// 这是blue字节的位置，接下来的2个字节分别含有green、red数据。
        /// </remarks>
        private static void BitmapDataColorSet(BitmapData destData, int x, int y, Color color)
        {
            byte[] rgbValues = new byte[3] { color.B, color.G, color.R };
            Marshal.Copy(rgbValues, 0, (IntPtr)((int)destData.Scan0 + ((y * destData.Stride) + (x * 3))), 3);
        }
    }
}
