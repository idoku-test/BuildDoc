using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities.Forms
{
    [Serializable]
    [DataContract]
    public partial class TemplateDirectoryDTO
    {
        private decimal _id;
        /// <summary>
        /// ID
        /// </summary>		
        [DataMember]
        public decimal ID
        {
            get { return _id; }
            set { _id = value; }
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
        private decimal? _template_id;
        /// <summary>
        /// 模板ID
        /// </summary>		
        [DataMember]
        public decimal? TEMPLATE_ID
        {
            get { return _template_id; }
            set { _template_id = value; }
        }
        private decimal? _sort;
        /// <summary>
        /// DIRECTORY_ORDER
        /// </summary>		
        [DataMember]
        public decimal? SORT
        {
            get { return _sort; }
            set { _sort = value; }
        }

    }
}
