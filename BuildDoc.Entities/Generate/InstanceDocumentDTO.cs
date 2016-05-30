using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //实例文档表    
    [DataContract]
    public partial class InstanceDocumentDTO
    {
        private decimal _instance_document_id;
        /// <summary>
        /// 实例文档ID
        /// </summary>		
        [DataMember]
        public decimal INSTANCE_DOCUMENT_ID
        {
            get { return _instance_document_id; }
            set { _instance_document_id = value; }
        }
        private decimal? _mother_set_id;
        /// <summary>
        /// 所属母版
        /// </summary>		
        [DataMember]
        public decimal? MOTHER_SET_ID
        {
            get { return _mother_set_id; }
            set { _mother_set_id = value; }
        }
        private string _document_structure;
        /// <summary>
        /// 文档构建结构
        /// </summary>		
        [DataMember]
        public string DOCUMENT_STRUCTURE
        {
            get { return _document_structure; }
            set { _document_structure = value; }
        }
        private string _manual_editing_return;
        /// <summary>
        /// 手动编辑返回值
        /// </summary>		
        [DataMember]
        public string MANUAL_EDITING_RETURN
        {
            get { return _manual_editing_return; }
            set { _manual_editing_return = value; }
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

    }
}
