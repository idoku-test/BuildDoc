using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //标签数据处理表
    [DataContract]
    public class LabelDealWithModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        public decimal LABEL_DEAL_WITH_ID
        {
            get;
            set;
        }
        /// <summary>
        /// 数据源ID
        /// </summary>
        [DataMember]
        public decimal DATA_SOURCE_ID
        {
            get;
            set;
        }
        /// <summary>
        /// 实际值
        /// </summary>
        [DataMember]
        public string ACTUAL_VALUE
        {
            get;
            set;
        }
        /// <summary>
        /// 显示值
        /// </summary>
        [DataMember]
        public string DISPLAY_VALUE
        {
            get;
            set;
        }
        /// <summary>
        /// 数据源名称
        /// </summary>
        [DataMember]
        public string DATA_SOURCE_NAME
        { get; set; }
    }
}
