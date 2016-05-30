using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //公司文档构建表
    [Serializable]
    [DataContract]
    public partial class DocumentStructureDTO
    {
        private decimal _structure_id;
        /// <summary>
        /// 构建ID
        /// </summary>		
        [DataMember]
        public decimal STRUCTURE_ID
        {
            get { return _structure_id; }
            set { _structure_id = value; }
        }
        private decimal? _customer_id;
        /// <summary>
        /// 所属公司
        /// </summary>		
        [DataMember]
        public decimal? CUSTOMER_ID
        {
            get { return _customer_id; }
            set { _customer_id = value; }
        }
        private string _structure_name;
        /// <summary>
        /// 构建名称
        /// </summary>		
        [DataMember]
        public string STRUCTURE_NAME
        {
            get { return _structure_name; }
            set { _structure_name = value; }
        }
        private decimal? _template_type;
        /// <summary>
        /// 模板类型
        /// </summary>		
        [DataMember]
        public decimal? TEMPLATE_TYPE
        {
            get { return _template_type; }
            set { _template_type = value; }
        }
        private decimal? _file_id;
        /// <summary>
        /// 文档ID
        /// </summary>		
        [DataMember]
        public decimal? FILE_ID
        {
            get { return _file_id; }
            set { _file_id = value; }
        }
        private decimal? _structure_type;
        /// <summary>
        /// 构建类型
        /// </summary>		
        [DataMember]
        public decimal? STRUCTURE_TYPE
        {
            get { return _structure_type; }
            set { _structure_type = value; }
        }
        private string _set_content;
        /// <summary>
        /// 配置内容
        /// </summary>		
        [DataMember]
        public string SET_CONTENT
        {
            get { return _set_content; }
            set { _set_content = value; }
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
        private decimal? _sort;
        /// <summary>
        /// 排序
        /// </summary>		
        [DataMember]
        public decimal? SORT
        {
            get { return _sort; }
            set { _sort = value; }
        }
        private decimal? _document_type;
        /// <summary>
        /// 文档类型
        /// </summary>		
        [DataMember]
        public decimal? DOCUMENT_TYPE
        {
            get { return _document_type; }
            set { _document_type = value; }
        }
        private decimal? _is_new_section;
        /// <summary>
        /// 是否新节点
        /// </summary>		
        [DataMember]
        public decimal? IS_NEW_SECTION
        {
            get { return _is_new_section; }
            set { _is_new_section = value; }
        }

    }
}
