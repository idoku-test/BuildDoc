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
    public partial class TemplateParameterDTO
    {
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
        private decimal? _form_label_id;
        /// <summary>
        /// FORM_LABEL_ID
        /// </summary>		
        [DataMember]
        public decimal? FORM_LABEL_ID
        {
            get { return _form_label_id; }
            set { _form_label_id = value; }
        }
        private decimal? _in_or_out;
        /// <summary>
        /// IN_OR_OUT
        /// </summary>		
        [DataMember]
        public decimal? IN_OR_OUT
        {
            get { return _in_or_out; }
            set { _in_or_out = value; }
        }

        [DataMember]
        public string LABEL_NAME { get; set; }

        [DataMember]
        public string LABEL_NAME_CHS { get; set; }

    }
}
