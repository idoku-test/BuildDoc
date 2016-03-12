using BuildDoc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redas.Logic
{
    public interface IReportLogic
    {
        DocMaster CreateDocMaster(decimal masterID, decimal objectID, string jsonStructure, decimal instanceDocumentID, int currentUser, int customer_id, bool isBuildDoc, ref string error, bool? isReduction, EntrustItem? businessType, int? objectIndex);

        BuildDoc.DocMerger CreateDocMerger(decimal mergerInstanceDocumentID, List<decimal> instanceDocumentIDList, List<decimal> objIdList);

        BaseResult AddInstance_Document(T_INSTANCE_DOCUMENT info, EntrustItem t, bool isSingleObject, bool isDownLoadSort);

        BaseResult UpdateInstance_Structure(T_INSTANCE_DOCUMENT info);

        T_INSTANCE_DOCUMENT GetInstanceDocumentGetByObject(decimal objectId, EntrustItem businessType);

        //IList<MotherSetModel> GetMotherSetByCustomer(int customerId, int templateType);

        IList<ObjectAndHouseModel> GetObjectByProjectId(int projectId);


        ObjectDTO GetObjectById(decimal objectId);

        FormStoreDTO GetFormStore(decimal? store_id);

        void UpdateReportFileID(decimal objectId, decimal report_file_id, EntrustItem t);

        List<_ChangeSortModel> GetObjectInstanceByProjectId(decimal projectId, EntrustItem businessType, decimal customerId);

        T_INSTANCE_DOCUMENT GetInstanceInfo(int instanceId);

    }
}
