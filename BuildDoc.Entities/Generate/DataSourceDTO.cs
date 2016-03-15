using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //数据源表
    [Serializable]
    [DataContract]
    public partial class DataSourceDTO
    {
        private decimal _data_source_id;
        /// <summary>
        /// 数据源ID
        /// </summary>		
        [DataMember]
        public decimal DATA_SOURCE_ID
        {
            get { return _data_source_id; }
            set { _data_source_id = value; }
        }
        private decimal? _template_type;
        /// <summary>
        /// 所属模板类型
        /// </summary>		
        [DataMember]
        public decimal? TEMPLATE_TYPE
        {
            get { return _template_type; }
            set { _template_type = value; }
        }
        private string _data_source_name;
        /// <summary>
        /// 数据源名称
        /// </summary>		
        [DataMember]
        public string DATA_SOURCE_NAME
        {
            get { return _data_source_name; }
            set { _data_source_name = value; }
        }
        private string _sql_content;
        /// <summary>
        /// SQL语句（加上排序检查）
        /// </summary>		
        [DataMember]
        public string SQL_CONTENT
        {
            get { return _sql_content; }
            set { _sql_content = value; }
        }
        private decimal? _is_multi_select;
        /// <summary>
        /// 是否多选
        /// </summary>		
        [DataMember]
        public decimal? IS_MULTI_SELECT
        {
            get { return _is_multi_select; }
            set { _is_multi_select = value; }
        }
        private string _multi_select_field;
        /// <summary>
        /// 多选过滤条件字段
        /// </summary>		
        [DataMember]
        public string MULTI_SELECT_FIELD
        {
            get { return _multi_select_field; }
            set { _multi_select_field = value; }
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
        private string _fsq_db_name;
        /// <summary>
        /// 数据库名
        /// </summary>		
        [DataMember]
        public string FSQ_DB_NAME
        {
            get { return _fsq_db_name; }
            set { _fsq_db_name = value; }
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
        private decimal? _data_source_type;
        /// <summary>
        /// 数据源类型
        /// </summary>		
        [DataMember]
        public decimal? DATA_SOURCE_TYPE
        {
            get { return _data_source_type; }
            set { _data_source_type = value; }
        }

    }
}
