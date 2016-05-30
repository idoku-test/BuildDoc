using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FileServerInterface.Model
{
    /// <summary>
    /// 
    /// </summary>
     [DataContract]
    public class StorageFileInfo
    {
        ///// <summary>
        ///// Gets or sets the attributes.
        ///// </summary>
        ///// <value>
        ///// The attributes.
        ///// </value>
        //public IList<FileAttribute> Attributes { get; set; }

        /// <summary>
        /// File_Id
        /// </summary>
        [DataMember]
        public decimal FILE_ID { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [DataMember]
        public string NAME { get; set; }

        /// <summary>
        /// 文件哈希名
        /// </summary>
        [DataMember]
        public string HASH_NAME { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [DataMember]
        public string FILE_PATH { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [DataMember]
        public decimal? FILE_SIZE { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [DataMember]
        public decimal? FILE_TYPE { get; set; }

        /// <summary>
        /// 服务节点
        /// </summary>
        [DataMember]
        public string NAME_SERVER { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [DataMember]
        public decimal? VTIMESTAMP { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? CREATE_TIME { get; set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        [DataMember]
        public DateTime? LAST_ACCESS_TIME { get; set; }


        /// <summary>
        /// 已上传文件大小
        /// </summary>
        [DataMember]
        public decimal? UPLOADED_SIZE { get; set; }
    }
}