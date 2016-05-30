using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //模板类型表    
    [DataContract]
    public partial class TemplateTypeDTO
    {
        private decimal _template_type;
        /// <summary>
        /// 模板类型
        /// </summary>		
        [DataMember]
        public decimal TEMPLATE_TYPE
        {
            get { return _template_type; }
            set { _template_type = value; }
        }
        private string _template_type_name;
        /// <summary>
        /// 模板类型名称
        /// </summary>		
        [DataMember]
        public string TEMPLATE_TYPE_NAME
        {
            get { return _template_type_name; }
            set { _template_type_name = value; }
        }
        private string _template_type_desc;
        /// <summary>
        /// 模板类型描述
        /// </summary>		
        [DataMember]
        public string TEMPLATE_TYPE_DESC
        {
            get { return _template_type_desc; }
            set { _template_type_desc = value; }
        }
        private string _sql_content;
        /// <summary>
        /// SQL语句
        /// </summary>		
        [DataMember]
        public string SQL_CONTENT
        {
            get { return _sql_content; }
            set { _sql_content = value; }
        }
        private string _inlay_parameter_set;
        /// <summary>
        /// 内置参数配置
        /// </summary>		
        [DataMember]
        public string INLAY_PARAMETER_SET
        {
            get { return _inlay_parameter_set; }
            set { _inlay_parameter_set = value; }
        }
        private decimal? _is_structure_template;
        /// <summary>
        /// 是否是构建模块
        /// </summary>		
        [DataMember]
        public decimal? IS_STRUCTURE_TEMPLATE
        {
            get { return _is_structure_template; }
            set { _is_structure_template = value; }
        }
        private DateTime? _created_time;
        /// <summary>
        /// 创建时间
        /// </summary>		
        [DataMember]
        public DateTime? CREATED_TIME
        {
            get { return _created_time; }
            set { _created_time = value; }
        }
        private decimal? _created_by;
        /// <summary>
        /// 创建人
        /// </summary>		
        [DataMember]
        public decimal? CREATED_BY
        {
            get { return _created_by; }
            set { _created_by = value; }
        }
        private DateTime? _modified_time;
        /// <summary>
        /// 修改时间
        /// </summary>		
        [DataMember]
        public DateTime? MODIFIED_TIME
        {
            get { return _modified_time; }
            set { _modified_time = value; }
        }
        private decimal? _modified_by;
        /// <summary>
        /// 修改人
        /// </summary>		
        [DataMember]
        public decimal? MODIFIED_BY
        {
            get { return _modified_by; }
            set { _modified_by = value; }
        }
        private decimal? _valid;
        /// <summary>
        /// 是否启用
        /// </summary>		
        [DataMember]
        public decimal? VALID
        {
            get { return _valid; }
            set { _valid = value; }
        }

    }
}
