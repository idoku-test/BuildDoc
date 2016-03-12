using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Common.ImageProcessor.Imaging
{
    public class BorderLayer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public BorderLayer()
        {
            this.Width = 2;
            this.BackroundColor = Color.Transparent;
            this.Top = true;
            this.Bottom = true;
            this.Left = true;
            this.Right = true;
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="width">The width at whcih the border will be done.</param>
        /// <param name="top">Set if top is border</param>
        /// <param name="bottom">Set if bottom is border</param>
        /// <param name="left">Set if left is border</param>
        /// <param name="righ">Set if right is border</param>
        public BorderLayer(int width, bool top, bool bottom, bool left, bool righ)
        {
            this.Width = width;
            this.BackroundColor = Color.Black;
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = righ;
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="width">The width at whcih the border will be done.</param>
        /// <param name="color">The color to set as the background color.</param>
        /// <param name="top">Set if top is border</param>
        /// <param name="bottom">Set if bottom is border</param>
        /// <param name="left">Set if left is border</param>
        /// <param name="righ">Set if right is border</param>
        public BorderLayer(int width, Color color, bool top, bool bottom, bool left, bool righ)
        {
            this.Width = width;
            this.BackroundColor = color;
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = righ;
        
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or sets the width of the border
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Get or sets the background color.
        /// </summary>
        public Color BackroundColor { get; set; }

        /// <summary>
        /// Get or sets a value indicating whether top border are to be added.
        /// </summary>
        public bool Top { get; set; }

        /// <summary>
        /// Get or sets a value indicating whether Bottom border are to be added.
        /// </summary>
        public bool Bottom { get; set; }

        /// <summary>
        /// Get or sets a value indicating whether left border are to be added.
        /// </summary>
        public bool Left { get; set; }

        /// <summary>
        /// Get or sets a value indicating whether right border are to be added.
        /// </summary>
        public bool Right { get; set; }

        #endregion

        /// <summary>
        /// Returns a value that indicates whether the specified object is an 
        /// BorderLayer object that is equivalent to this BorderLayer object.
        /// </summary>
        /// <param name="obj">The object to test</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            BorderLayer border = obj as BorderLayer;
            if (border == null)
                return false;

            return this.Width == border.Width && this.BackroundColor == border.BackroundColor
                && this.Top == border.Top && this.Bottom == border.Bottom
                && this.Left == border.Left && this.Right == border.Right;
        }

        /// <summary>
        /// Returns a hash code value that represents this object.
        /// </summary>
        /// <returns>
        /// A hash code that represents this object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Width.GetHashCode() + this.BackroundColor.GetHashCode() +
                this.Top.GetHashCode() + this.Bottom.GetHashCode() +
                this.Left.GetHashCode() + this.Right.GetHashCode();
        }
    }
}
