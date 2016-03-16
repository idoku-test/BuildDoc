using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    //数据源表     
    public partial class DataSourceDTO
    {
        public IList<string> Fields { get; set; }
    }
}
