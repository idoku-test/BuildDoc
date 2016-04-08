using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //数据标签表
    [DataContract]
    public class DataLabelModel
    {
        /// <summary>
        /// 数据标签ID
        /// </summary>
        [DataMember]
        public decimal DATA_LABEL_ID { get; set; }



        /// <summary>
        /// 标签名
        /// </summary>
        [DataMember]
        public string LABEL_NAME { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        [DataMember]
        public string CONFIG_CONTENT { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? CREATED_TIME { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public decimal? CREATED_BY { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime? MODIFIED_TIME { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [DataMember]
        public decimal? MODIFIED_BY { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DATA_SOURCE_NAME { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        [DataMember]
        public decimal CUSTOMER_ID { get; set; }
    }
}
