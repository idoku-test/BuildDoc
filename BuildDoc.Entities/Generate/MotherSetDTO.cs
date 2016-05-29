using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //公司文档母版表
    [DataContract]
    public class MotherSetDTO
    {
        /// <summary>
        /// 母版ID
        /// </summary>
        [DataMember]
        public decimal MOTHER_SET_ID { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public decimal? CUSTOMER_ID { get; set; }

        /// <summary>
        /// 母版名称
        /// </summary>
        [DataMember]
        public string MOTHER_SET_NAME { get; set; }

        /// <summary>
        /// 文档ID
        /// </summary>
        [DataMember]
        public decimal? FILE_ID { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        [DataMember]
        public decimal? TEMPLATE_TYPE { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        [DataMember]
        public string SET_CONTENT { get; set; }

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
        /// 是否启用
        /// </summary>
        [DataMember]
        public decimal? VALID { get; set; }

        /// <summary>
        /// 估价目的
        /// </summary>
        [DataMember]
        public decimal? EVALUATE_PURPOSE { get; set; }

        /// <summary>
        /// 估价目的名称
        /// </summary>
        [DataMember]
        public string EVALUATE_PURPOSENAME { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        public decimal? BUSINESS_TYPE { get; set; }
        /// <summary>
        /// 业务类型名称
        /// </summary>
        [DataMember]
        public string BUSINESS_TYPENAME { get; set; }

        /// <summary>
        /// 模板类型名称
        /// </summary>
        [DataMember]
        public string TEMPLATE_TYPE_NAME { get; set; }

        /// <summary>
        /// 报告类型
        /// </summary>
        [DataMember]
        public decimal? DOCUMENT_TYPE { get; set; }

        public string DOCUMENT_TYPENAME { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        [DataMember]
        public string FILE_NAME { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        [DataMember]
        public decimal? BANK_ID { get; set; }
        /// <summary>
        /// 单多标的物
        /// </summary>
        [DataMember]
        public decimal? ONE_MORE_OBJECT { get; set; }

        /// <summary>
        /// 最小建筑面积
        /// </summary>
        [DataMember]
        public decimal? MIN_SIZE { get; set; }

        /// <summary>
        /// 最大建筑面积
        /// </summary>
        [DataMember]
        public decimal? MAX_SIZE { get; set; }

        private decimal _sort = 100;
        /// <summary>
        /// 排序（附加属性，用于模板自动筛选时）
        /// </summary>
        public decimal Sort
        {
            get
            {
                return _sort;

            }
            set
            {
                _sort = value;
            }
        }

    }
}
