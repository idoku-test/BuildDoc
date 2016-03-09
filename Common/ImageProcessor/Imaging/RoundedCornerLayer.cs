﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Common.ImageProcessor.Imaging
{
    /// <summary>
    /// Encapsulates the properties required to add rounded corners to an image.
    /// </summary>
    public class RoundedCornerLayer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedCornerLayer"/> class.
        /// </summary>
        public RoundedCornerLayer()
        {
            this.Radius = 10;
            this.BackgroundColor = Color.Transparent;
            this.TopLeft = true;
            this.TopRight = true;
            this.BottomLeft = true;
            this.BottomRight = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedCornerLayer"/> class.
        /// </summary>
        /// <param name="radius">
        /// The radius at which the corner will be done.
        /// </param>
        /// <param name="topLeft">
        /// Set if top left is rounded
        /// </param>
        /// <param name="topRight">
        /// Set if top right is rounded
        /// </param>
        /// <param name="bottomLeft">
        /// Set if bottom left is rounded
        /// </param>
        /// <param name="bottomRight">
        /// Set if bottom right is rounded
        /// </param>
        public RoundedCornerLayer(int radius, bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            this.Radius = radius;
            this.BackgroundColor = Color.Transparent;
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomLeft = bottomLeft;
            this.BottomRight = bottomRight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedCornerLayer"/> class.
        /// </summary>
        /// <param name="radius">
        /// The radius at which the corner will be done.
        /// </param>
        /// <param name="backgroundColor">
        /// The <see cref="T:System.Drawing.Color"/> to set as the background color.
        /// <remarks>Used for image formats that do not support transparency</remarks>
        /// </param>
        /// <param name="topLeft">
        /// Set if top left is rounded
        /// </param>
        /// <param name="topRight">
        /// Set if top right is rounded
        /// </param>
        /// <param name="bottomLeft">
        /// Set if bottom left is rounded
        /// </param>
        /// <param name="bottomRight">
        /// Set if bottom right is rounded
        /// </param>
        public RoundedCornerLayer(int radius, Color backgroundColor, bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            this.Radius = radius;
            this.BackgroundColor = backgroundColor;
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomLeft = bottomLeft;
            this.BottomRight = bottomRight;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the radius of the corners.
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether top left corners are to be added.
        /// </summary>
        public bool TopLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether top right corners are to be added.
        /// </summary>
        public bool TopRight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bottom left corners are to be added.
        /// </summary>
        public bool BottomLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bottom right corners are to be added.
        /// </summary>
        public bool BottomRight { get; set; }
        #endregion

        /// <summary>
        /// Returns a value that indicates whether the specified object is an 
        /// <see cref="RoundedCornerLayer"/> object that is equivalent to 
        /// this <see cref="RoundedCornerLayer"/> object.
        /// </summary>
        /// <param name="obj">
        /// The object to test.
        /// </param>
        /// <returns>
        /// True if the given object is an <see cref="RoundedCornerLayer"/> object that is equivalent to 
        /// this <see cref="RoundedCornerLayer"/> object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            RoundedCornerLayer rounded = obj as RoundedCornerLayer;

            if (rounded == null)
            {
                return false;
            }

            return this.Radius == rounded.Radius && this.BackgroundColor == rounded.BackgroundColor
                   && this.TopLeft == rounded.TopLeft && this.TopRight == rounded.TopRight
                   && this.BottomLeft == rounded.BottomLeft && this.BottomRight == rounded.BottomRight;
        }

        /// <summary>
        /// Returns a hash code value that represents this object.
        /// </summary>
        /// <returns>
        /// A hash code that represents this object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Radius.GetHashCode() + this.BackgroundColor.GetHashCode() +
                   this.TopLeft.GetHashCode() + this.TopRight.GetHashCode() +
                   this.BottomLeft.GetHashCode() + this.BottomRight.GetHashCode();
        }
    }
}
