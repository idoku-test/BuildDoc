using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    [Serializable]
    [DataContract]
    public class SELECTMODEL
    {
        [DataMember]
        public decimal SELECTVALUE { get; set; }
        [DataMember]
        public string SELECTTEXT { get; set; }
    }
}
