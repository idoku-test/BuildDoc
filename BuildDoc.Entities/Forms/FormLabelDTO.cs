using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities.Forms
{
    //表单标签表
    public partial class FormLabelDTO
    {
        private decimal _form_label_id;
        /// <summary>
        /// 表单标签ID
        /// </summary>		
        [DataMember]
        public decimal FORM_LABEL_ID
        {
            get { return _form_label_id; }
            set { _form_label_id = value; }
        }
        private decimal? _directory_id;
        /// <summary>
        /// 层级ID
        /// </summary>		
        [DataMember]
        public decimal? DIRECTORY_ID
        {
            get { return _directory_id; }
            set { _directory_id = value; }
        }
        private decimal? _fsq_field_id;
        /// <summary>
        /// 字段ID
        /// </summary>		
        [DataMember]
        public decimal? FSQ_FIELD_ID
        {
            get { return _fsq_field_id; }
            set { _fsq_field_id = value; }
        }
        private string _label_name;
        /// <summary>
        /// 标签名称
        /// </summary>		
        [DataMember]
        public string LABEL_NAME
        {
            get { return _label_name; }
            set { _label_name = value; }
        }
        private string _label_alias;
        /// <summary>
        /// 标签别名
        /// </summary>		
        [DataMember]
        public string LABEL_ALIAS
        {
            get { return _label_alias; }
            set { _label_alias = value; }
        }

        private string _label_name_chs;
        /// <summary>
        /// 标签名称
        /// </summary>		
        [DataMember]
        public string LABEL_NAME_CHS
        {
            get { return _label_name_chs; }
            set { _label_name_chs = value; }
        }
        private decimal? _label_type;
        /// <summary>
        /// 标签类型
        /// </summary>		
        [DataMember]
        public decimal? LABEL_TYPE
        {
            get { return _label_type; }
            set { _label_type = value; }
        }
        private decimal? _data_type;
        /// <summary>
        /// 数据类型
        /// </summary>		
        [DataMember]
        public decimal? DATA_TYPE
        {
            get { return _data_type; }
            set { _data_type = value; }
        }
        private decimal? _label_order;
        /// <summary>
        /// 标签排序
        /// </summary>		
        [DataMember]
        public decimal? LABEL_ORDER
        {
            get { return _label_order; }
            set { _label_order = value; }
        }
        private decimal? _dic_type_id;
        /// <summary>
        /// 字典类型ID
        /// </summary>		
        //[DataMember]
        public decimal? DIC_TYPE_ID
        {
            get { return _dic_type_id; }
            set { _dic_type_id = value; }
        }

        private string _api_uri;
        /// <summary>
        /// 接口地址
        /// </summary>		
        [DataMember]
        public string API_URI
        {
            get { return _api_uri; }
            set { _api_uri = value; }
        }
        private decimal? _is_required;
        /// <summary>
        /// 是否必填
        /// </summary>		
        [DataMember]
        public decimal? IS_REQUIRED
        {
            get { return _is_required; }
            set { _is_required = value; }
        }

        private string _label_associate;
        /// <summary>
        /// 关联标签
        /// </summary>		
        [DataMember]

        public string LABEL_ASSOCIATE
        {
            get { return _label_associate; }
            set { _label_associate = value; }
        }

        private decimal? _value_mode;

        /// <summary>
        /// 取值方式（1：单一，2：合并）
        /// </summary>		
        [DataMember]
        public decimal? VALUE_MODE
        {
            get { return _value_mode; }
            set { _value_mode = value; }
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

        private string _content;
        /// <summary>
        /// 标签内容
        /// </summary>		
        [DataMember]
        public string CONTENT
        {
            get { return _content; }
            set { _content = value; }
        }


        public decimal FORM_FIELD_ID { get; set; }
        public string FORM_FIELD_NAME { get; set; }

        public string DIC_TYPE_NAME { get; set; }

        public IList<LabelDataDTO> BindDataList { get; set; }

        public string API_NAME { get; set; }

        public string API_URL { get; set; }
            
        public bool IsSelected { get; set; }

        public decimal SORT { get; set; }
    }
}
