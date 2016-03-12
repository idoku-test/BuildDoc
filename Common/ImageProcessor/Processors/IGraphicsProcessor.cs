﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using Common.ImageProcessor;

namespace Common.ImageProcessor.Processors
{
    /// <summary>
    /// Defines properties and methods for ImageProcessor Plugins.
    /// </summary>
    public interface IGraphicsProcessor
    {
        #region Properties
        /// <summary>
        /// Gets the regular expression to search strings for.
        /// </summary>
        Regex RegexPattern { get; }

        /// <summary>
        /// Gets DynamicParameter.
        /// </summary>
        dynamic DynamicParameter { get; }

        /// <summary>
        /// Gets the order in which this processor is to be used in a chain.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Gets or sets any additional settings required by the processor.
        /// </summary>
        Dictionary<string, string> Settings { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// The position in the original string where the first character of the captured substring was found.
        /// </summary>
        /// <param name="queryString">
        /// The query string to search.
        /// </param>
        /// <returns>
        /// The zero-based starting position in the original string where the captured substring was found.
        /// </returns>
        int MatchRegexIndex(string queryString);

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
        Image ProcessImage(ImageFactory factory);
        #endregion
    }
}
