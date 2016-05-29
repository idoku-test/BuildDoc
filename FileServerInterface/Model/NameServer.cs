using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileServerInterface.Model
{
    public class NameServer
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the servername.
        /// </summary>
        /// <value>
        /// The servername.
        /// </value>
        public string Servername { get; set; }


        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        /// <value>
        /// The ip.
        /// </value>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the virtual path.
        /// </summary>
        /// <value>
        /// The virtual path.
        /// </value>
        public string VirtualPath { get; set; }
    }
}