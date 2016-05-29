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

        IList<LabelDealWithModel> GetLabelDealSource();
        #endregion

        #region mother set
        IList<MotherSetDTO> GetMotherSetByCustomer(int customerId, int type);

        MotherSetDTO GetMotherSet(int motherId);
        #endregion
        #region label 
        BaseResult SaveLabel(DataLabelModel model);
        #endregion
    }
}
