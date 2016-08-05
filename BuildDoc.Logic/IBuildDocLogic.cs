using BuildDoc.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public interface IBuildDocLogic
    {
        #region template
        TemplateTypeDTO GetTemplateType(int templateTypeId);

        Dictionary<string, string> GetTemplateParms(string sql);      
        #endregion

        #region data source
        IList<DataSourceDTO> GetAllDataSource();

        IList<DataSourceDTO> GetDataSource(int templateTypeId);

        IList<DataSourceDTO> GetDataSourceByType(int type);

        DataTable GetDataSource(string dbName, string sql);

        IList<LabelDealWithModel> GetLabelDealSource();
        #endregion

        #region mother set
        IList<MotherSetDTO> GetMotherSetByCustomer(int customerId, int type);

        MotherSetDTO GetMotherSet(int motherId);
        #endregion

        #region structure
        DocumentStructureDTO GetStructure(int structureId);

        IList<DocumentStructureDTO> GetStructuresByCustomer(decimal customerId, decimal stype, decimal dtype);
        #endregion

        #region label
        BaseResult SaveLabel(DataLabelModel model);


        DataLabelModel GetLabel(int customerId, string dataLabelName);
        #endregion

        #region instance
        InstanceDocumentDTO GetInstanceDocument(decimal instanceDocumentID);
        #endregion

        #region garbage
        string GetValue(decimal objectID, string tableID, string fieldID, decimal structureID, string labelName, Dictionary<string, string> parame);
        #endregion
    }
}
