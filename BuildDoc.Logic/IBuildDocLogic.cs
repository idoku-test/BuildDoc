using BuildDoc.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public interface IBuildDocLogic
    {

        #region data source
        IList<DataSourceDTO> GetDataSource(int type);
        #endregion

    }
}
