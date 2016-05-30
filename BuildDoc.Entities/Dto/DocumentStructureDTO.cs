using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    public partial class DocumentStructureDTO
    {
        /// <summary>
        /// 标的物类型
        /// </summary>
        [DataMember]
        public string OBJECT_TYPE { get; set; }

        /// <summary>
        /// 标的物类型名称
        /// </summary>
        [DataMember]
        public string OBJECT_TYPE_NAME { get; set; }

        /// <summary>
        /// 估价目的
        /// </summary>
        [DataMember]
        public string EVALUATE_PURPOSE { get; set; }

        /// <summary>
        /// 估价目的名称
        /// </summary>
        [DataMember]
        public string EVALUATE_PURPOSE_NAME { get; set; }

        /// <summary>
        /// 单/多标的物
        /// </summary>
        [DataMember]
        public string ONE_MORE_OBJECT { get; set; }


        /// <summary>
        /// 单/多标的物名称
        /// </summary>
        [DataMember]
        public string ONE_MORE_OBJECT_NAME { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        [DataMember]
        public decimal BANK_ID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string REMARK { get; set; }
    }
}
