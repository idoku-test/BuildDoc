using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities.Forms
{
    [DataContract]
    public class DFormViewModel
    {
        [DataMember]
        public IList<FormLabelDTO> FormLabels
        {
            get;
            set;
        }

        /// <summary>
        /// 模板目录
        /// </summary>
        [DataMember]
        public IList<TemplateDirectoryDTO> TemplateDirectories
        {
            get;
            set;
        }

        /// <summary>
        /// 模板参数
        /// </summary>
        [DataMember]
        public IList<TemplateParameterDTO> TemplateParms
        {
            get;
            set;
        }
    }
}
