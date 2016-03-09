using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ImageProcessor.Imaging
{
    /// <summary>
    /// Enumerated resize modes to apply to resized images.
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// Pads the resized image to fit the bounds of its container.
        /// </summary>
        Pad,

        /// <summary>
        /// 固定大小 Stretches the resized image to fit the bounds of its container.
        /// </summary>
        Stretch,

        /// <summary>
        /// Crops the resized image to fit the bounds of its container.
        /// </summary>
        Crop,

        /// <summary>
        /// Constrains the resized image to fit the bounds of its container.
        /// </summary>
        Max,
        /// <summary>
        /// 等比缩放
        /// </summary>
        Geometric
    }
}
