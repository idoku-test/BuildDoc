using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using Common.ImageProcessor.Imaging;
using System.Drawing.Drawing2D;

namespace Common.ImageProcessor.Processors
{
    /// <summary>
    /// Encapsulates methods to add border to an image.
    /// </summary>
    public class Borders : IGraphicsProcessor
    {
        /// <summary>
        /// The regular expression to search strings for.
        /// </summary>
        private static readonly Regex QueryRegex = new Regex(@"borders=(\d+|[^&]*)", RegexOptions.Compiled);

        /// <summary>
        /// The regular expression to search strings for the width attribute.
        /// </summary>
        private static readonly Regex widthRegex = new Regex(@"width-(\d+)", RegexOptions.Compiled);

        /// <summary>
        /// The regular expression to search strings for the color attribute.
        /// </summary>
        private static readonly Regex ColorRegex = new Regex(@"bgcolor-([0-9a-fA-F]{3}){1,2}", RegexOptions.Compiled);

        /// <summary>
        /// The regular expression to search strings for the top  attribute.
        /// </summary>
        private static readonly Regex TopRegex = new Regex(@"top-(true|false)", RegexOptions.Compiled);

        /// <summary>
        /// The regular expression to search strings for the bottom attribute.
        /// </summary>
        private static readonly Regex BottomRegex = new Regex(@"bottom-(true|false)", RegexOptions.Compiled);

        /// <summary>
        /// The regular expression to search strings for the left attribute.
        /// </summary>
        private static readonly Regex LeftRegex = new Regex(@"left-(true|false)", RegexOptions.Compiled);

        /// <summary>
        /// The regular expression to search strings for the  right attribute.
        /// </summary>
        private static readonly Regex RightRegex = new Regex(@"right-(true|false)", RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression to searh string for.
        /// </summary>
        public Regex RegexPattern
        {
            get { return QueryRegex; }
        }

        /// <summary>
        /// Get or sets DynamicParameter.
        /// </summary>
        public dynamic DynamicParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the order in which this processor is to be used in a chain.
        /// </summary>
        public int SortOrder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets any additional settings required by the processor.
        /// </summary>
        public Dictionary<string, string> Settings
        {
            get;
            set;
        }

        /// <summary>
        /// The position in the original string where the first character of the captured substring was found.
        /// </summary>
        /// <param name="queryString">
        /// The query string to search.
        /// </param>
        /// <returns>
        /// The zero-based starting position in the original string where the captured substring was found.
        /// </returns>
        public int MatchRegexIndex(string queryString)
        {
            int index = 0; 
            //set the sort order to max to allow filtering.
            this.SortOrder = int.MaxValue;
            foreach (Match match in this.RegexPattern.Matches(queryString))
            {
                if (match.Success)
                {
                    if (index == 0)
                    {
                        //Set the index on the first instance only.
                        this.SortOrder = match.Index;
                        BorderLayer borderLayer;
                        string toParse = match.Value;
                        if (toParse.Contains("bgcolor"))
                        {
                            borderLayer = new BorderLayer(
                                this.ParseWidth(toParse),
                                this.ParseColor(toParse),
                                this.ParseEdge(TopRegex, toParse), this.ParseEdge(BottomRegex, toParse),
                                this.ParseEdge(LeftRegex, toParse), this.ParseEdge(RightRegex, toParse)
                                );
                        }
                        else
                        {
                            int width;
                            int.TryParse(match.Value.Split('=')[1], out width);
                            borderLayer = new BorderLayer(width,
                                 this.ParseEdge(TopRegex, toParse), this.ParseEdge(BottomRegex, toParse),
                                 this.ParseEdge(LeftRegex, toParse), this.ParseEdge(RightRegex, toParse)
                                 );
                        }
                        this.DynamicParameter = borderLayer;
                    }
                    index += 1;
                }               
            }
            return this.SortOrder;
        }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="factory">
        /// The the current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class containing
        /// the image to process.
        /// </param>
        /// <returns>
        /// The processed image from the current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class.
        /// </returns>
        public Image ProcessImage(ImageFactory factory)
        {
            Bitmap newImage = null;
            Image image = factory.Image;
            try
            {
                BorderLayer borderLayer = this.DynamicParameter;
                int width = borderLayer.Width;
                Color backgroundColor = borderLayer.BackroundColor;
                bool top = borderLayer.Top;
                bool bottom = borderLayer.Bottom;
                bool right = borderLayer.Right;
                bool left = borderLayer.Left;

                //create a border image.
                newImage = this.BorderImage(image, width, backgroundColor, top, bottom, left, right);
                newImage.Tag = image.Tag;
                image.Dispose();
                image = newImage;
            }
            catch
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
            }

            return image;
        }

       

        #region Private Methods
        /// <summary>
        /// Adds border to the image
        /// </summary>
        /// <param name="image">The image to add border too</param>
        /// <param name="borderWidth">The width of the border</param>
        /// <param name="backgroundColor">The backround color to fill an image with.</param>
        /// <param name="top">If the top edge will have a border?</param>
        /// <param name="bottom">If the bottom edge will have a border?</param>
        /// <param name="left">If the left edge will have a border?</param>
        /// <param name="right">If the right edge will have a border?</param>
        /// <returns></returns>
        private Bitmap BorderImage(Image image, int borderWidth, Color backgroundColor, bool top, bool bottom, bool left, bool right)
        {
            int width = image.Width;
            int height = image.Height;

            //Create a new empty bitmap to hold border image;
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);           
            // Make a graphics object from the empty bitmap
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
               
                //Reduce the jagged edge.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Contrary to everything I have read bicubic is producing the best results.
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
               
                //Fill the backgroud.
                //graphics.Clear(backgroundColor);

                //draw original image
                graphics.DrawImage(image, 0, 0);
                 
                //Add border                 
                Pen pen = new Pen(backgroundColor, borderWidth);

                //graphics.DrawRectangle(pen, new Rectangle(0, 0, width, height));

                if (top)
                {
                    graphics.DrawLine(pen, 0, 0, width, 0);
                }
                if (bottom)
                {
                    graphics.DrawLine(pen, 0, height, width, height);
                }
                if (left)
                {
                    graphics.DrawLine(pen, 0, 0, 0, height);
                }
                if (right)
                {
                    graphics.DrawLine(pen, width, 0, width, height);
                }
            }
         
            return newImage;
        }

        /// <summary>
        /// Returns the correct containing the width for the given string.
        /// </summary>
        /// <param name="input">The input string containing the value to parse.</param>
        /// <returns></returns>
        private int ParseWidth(string input)
        {
            foreach (Match match in widthRegex.Matches(input))
            {
                //split on width-
                int width;
                int.TryParse(match.Value.Split('-')[1], out width);
                return width;
            }
            //no width - matches the BorderLayer default.
            return 0;
        }

        /// <summary>
        /// Return the correct for the given string.
        /// </summary>
        /// <param name="input">The input string containing the value to parse.</param>
        /// <returns></returns>
        private Color ParseColor(string input)
        {
            foreach (Match match in ColorRegex.Matches(input))
            {
                //split on color-hex
                return ColorTranslator.FromHtml("#" + match.Value.Split('-')[1]);
            }
            return Color.Black;
        }

        /// <summary>
        /// Return a edither ture or false.
        /// </summary>
        /// <param name="edge">The edge</param>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool ParseEdge(Regex edge, string input)
        {
            foreach (Match match in edge.Matches(input))
            {
                //Split on edgeBorder-
                bool edgeBorder;
                bool.TryParse(match.Value.Split('-')[1], out edgeBorder);
                return edgeBorder;
            }
            return true;
        }

        #endregion
    }
}
