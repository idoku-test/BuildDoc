using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities.Forms
{
    public class FormViewModel
    {
        /// <summary>
        /// 实例ID 
        /// </summary>
        public decimal INSTANCE_ID { get; set; }
        /// <summary>
        /// 客户ID 
        /// </summary>
        public decimal CUSTOMER_ID { get; set; }
        /// <summary>
        /// 模板ID 
        /// </summary>
        public decimal TEMPLATE_ID { get; set; }
        /// <summary>
        /// 表单存储ID 
        /// </summary>
        public decimal STORE_ID { get; set; }
        /// <summary>
        /// 表单实例 
        /// </summary>
        public string CONTENTS { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        public IList<FormLabelDTO> LABLELIST { get; set; }
        /// <summary>
        /// 表单提交数据
        /// </summary>
        public string FORMDATA { get; set; }
        /// <summary>
        /// 版本号  
        /// </summary>
        public string VERSION_NO { get; set; }
        /// <summary>
        /// 对象类型ID 
        /// </summary>
        public decimal OBJECT_TYPE_ID { get; set; }
        /// <summary>
        /// 标的物子类型 
        /// </summary>
        public decimal OBJECT_CHILD_TYPE { get; set; }
        /// <summary>
        /// 估价目的
        /// </summary>
        public decimal EVALUATE_PURPOSE { get; set; }
        /// <summary>
        /// 实例名称 
        /// </summary>
        public string INSTANCE_NAME { get; set; }

        /// <summary>
        /// 模板类型id
        /// </summary>
        public decimal TEMPLATE_CLASS_ID { get; set; }
        /// <summary>
        /// 实例类型ID 
        /// </summary>
        public decimal INSTANCE_TYPE_ID { get; set; }

        /// <summary>
        /// 原始表单ID
        /// </summary>
        public decimal INSTANCE_ORI_ID { get; set; }

        public DateTime MODIFIED_TIME { get; set; }

        public Decimal? MODIFIED_BY { get; set; }


        public string USER_NAME { get; set; }

    }
}
