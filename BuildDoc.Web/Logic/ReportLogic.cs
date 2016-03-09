using BuildDoc;
using DataAccess;
using Newtonsoft.Json.Linq;
using Redas.Entities;
using Redas.Entities.Redas;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redas.Logic
{
    public class ReportLogic : IReportLogic
    {
        private static IReportLogic _instance;
        public static IReportLogic CreateInstance()
        {
            IApplicationContext context = ContextRegistry.GetContext();
            if (_instance == null && context != null)
                _instance = context.GetObject("IReportLogic") as IReportLogic;
            return _instance;
        }

        /// <summary>
        /// 创建DocMaster的实例
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="structureIds"></param>
        /// <returns></returns>
        public DocMaster CreateDocMaster(decimal masterID, decimal objectID, string jsonStructure, decimal instanceDocumentID, int currentUser,int customer_id,bool isBuildDoc,ref string error,bool? isReduction,EntrustItem? businessType,int? objectIndex)
        {          
            string returnJSON = "";
            bool isSearch = true;
            //查询数据库是否已保存示例
            //isReduction为空的情况下是下载   1:预估函重新获取的时候，不需要传值；   2：报告 有实例的情况下：如果重新获取，即要拿预估函的信息；非重新获取，即拿本报告的
            //                                                                               无实例的情况下：拿预估函的实例   
            EntrustItem estimateType = EntrustItem.Estimate;
            if (businessType == EntrustItem.Estimate) {
                if (isReduction != null && Convert.ToBoolean(isReduction))
                    isSearch =false;  //不需要查询
            }
            else if (businessType == EntrustItem.ValuationReport) {
            
                estimateType = EntrustItem.Estimate;
                if (instanceDocumentID > 0 && (isReduction == null || !Convert.ToBoolean(isReduction))) {
                    estimateType = EntrustItem.ValuationReport;
                }
            }

            if (isSearch) 
            {             
                T_INSTANCE_DOCUMENT instanceInfo = GetInstanceDocumentGetByObject(objectID, estimateType);
                if (instanceInfo != null && instanceInfo.MANUAL_EDITING_RETURN != "")
                {
                    returnJSON = instanceInfo.MANUAL_EDITING_RETURN;                 
                }
            }
            string conCode = "", buildingCode = "", houseCode = "", unitCode = "";            
            var formView = new FormLogic().GetFormDataByObjectId(Convert.ToInt32(objectID), 1);
            if (formView.LABLELIST != null)
            {
                List<Redas.Entities.Ompd.FormLabelDTO> lstLabel = formView.LABLELIST.ToList();

                //查询对应的楼盘，楼栋，房号的code
                conCode = GetValFormList(lstLabel, "CONSTRUCTION_CODE");
                buildingCode = GetValFormList(lstLabel, "BUILDING_CODE");
                houseCode = GetValFormList(lstLabel, "HOUSE_CODE");
                unitCode = GetValFormList(lstLabel, "UNIT_CODE");
            }


            try
            {
                Dictionary<string, string> inputParams = new Dictionary<string, string>();
                inputParams.Add("ConstructionCode", conCode == null ? "" : conCode);
                inputParams.Add("BuildingCode", buildingCode == null ? "" : buildingCode);
                inputParams.Add("HouseCode", houseCode == null ? "" : houseCode);
                inputParams.Add("UnitCode", unitCode == null ? "" : unitCode);
                inputParams.Add("ObjectType", formView.OBJECT_TYPE_ID.ToString());
                inputParams.Add("CurrentUser", currentUser.ToString());
                inputParams.Add("CustomerID", customer_id.ToString());
                inputParams.Add("标的物顺序", objectIndex.ToString());
                //IsReduction 重新获取时 预估函:不需要查预估函   报告：需要查预估
                inputParams.Add("IsGetEstimate", isSearch.ToString());
                DocMaster docmaster = new DocMaster(masterID, objectID, returnJSON, jsonStructure, GetValue, inputParams, isBuildDoc);
                return docmaster;
            }
            catch (Exception ex)
            {
                //throw ex;
                error = ex.Message;
                return null;
            }

        }

        //从提交的表单数据中取出指定字段的值
        public string GetValFormList(List<Redas.Entities.Ompd.FormLabelDTO> lst, string Field)
        {
            string val = "";
            var lstLabel = lst.Where(l => l.LABEL_NAME == Field);
            if (lstLabel != null && lstLabel.Count() > 0)
            {
                val = lstLabel.ToList()[0].CONTENT;
            }
            return val;

        }

        /// <summary>
        /// 0:基础数据 1:询价 2:预估函 3:查勘 40031002:资料整理 40031001:资料整理
        /// </summary>
       // string sourceConfig = "[{ Type:40031001 },{ Type:40031002 },{ Type:3 },{ Type:2 },{ Type:1 },{ Type:0 }]";
        string sourceConfig = "[{ Type:40031001 },{ Type:2 },{ Type:3 },{ Type:1 },{ Type:0 }]";
        // 2015.3.8会议决议  1：资料补齐  2:预估函  3:查勘   4：询价  5：基础数据
        string objectCode, objectType = string.Empty;
        private string GetValue(decimal objectID, string tableID, string fieldID, decimal structureID, string labelName, Dictionary<string, string> parame)
        {
            var configList = JArray.Parse(sourceConfig);
            string value = string.Empty;
            var lstFormLabel = GetFormLabelList(fieldID);

            //获得楼盘code 楼栋code 房号code
            //var formView = new FormLogic().GetFormDataByObjectId(Convert.ToInt32(objectID), 1);
            //List<Redas.Entities.Ompd.FormLabelDTO> lstLabel = formView.LABLELIST.ToList();

            ////查询对应的楼盘，楼栋，房号的code
            //string conCode = lstLabel.Single(l => l.LABEL_NAME == "CONSTRUCTION_CODE").CONTENT;
            //string buildingCode = lstLabel.Single(l => l.LABEL_NAME == "BUILDING_CODE").CONTENT;
            //string houseCode = lstLabel.Single(l => l.LABEL_NAME == "HOUSE_CODE").CONTENT;

            string conCode = parame["ConstructionCode"];
            string buildingCode = parame["BuildingCode"];
            string houseCode = parame["HouseCode"];
            string unitCode = parame["UnitCode"];
            string objectType = parame["ObjectType"];
            bool isGetEstimate = Convert.ToBoolean(parame["IsGetEstimate"]);
            foreach (var o in configList)
            {
                int type = o.Value<int>("Type");
                switch (type)
                {
                    case 0:
                        {  
                            //根据objectCode, tableID, fieldID
                            value = GetBasicData(objectID, fieldID, conCode, buildingCode, houseCode, unitCode, objectType);
                        }
                        break;
                    case 1: //询价
                        Dictionary<int, JArray> lstEstForm = GetDynamicFormData(objectID, 1);
                        if (lstEstForm.Count() > 0)
                        {
                            JArray array = (JArray)lstEstForm[1];
                            foreach (var info in lstFormLabel) {
                                var item = array.FirstOrDefault(it => it["NAME"].Value<string>().Equals(info.SELECTVALUE.ToString()));
                                 if (item != null)
                                 { 
                                     value = item["VALUE"].Value<string>();
                                     break;
                                 }
                            }                          
                         
                        }
                        break;
                    case 2: //预估函       
                        if (isGetEstimate)   // 需要查询预估函
                        {
                            T_INSTANCE_DOCUMENT instanceInfo = GetInstanceDocumentGetByObject(objectID, EntrustItem.Estimate);

                            if (instanceInfo != null && instanceInfo.MANUAL_EDITING_RETURN != "")
                            {
                                List<_EditReturnModel> lstEditModel = JsonTools.JsonToObject2<List<_EditReturnModel>>(instanceInfo.MANUAL_EDITING_RETURN);
                                _EditReturnModel labelInfo = lstEditModel.Find(delegate(_EditReturnModel p)
                               {
                                   //return p.ID == structureID && p.LabelName == labelName;
                                   return p.LabelName == labelName;
                               });
                                if (labelInfo == null)
                                    continue;
                                else
                                    value = labelInfo.Value;
                            }
                        }
                        break;
                    case 3:
                        Dictionary<int, JArray> lstReportForm = GetDynamicFormData(objectID, 2);
                        if (lstReportForm.Count() > 0)
                        {
                            JArray array = (JArray)lstReportForm[2];
                            foreach (var info in lstFormLabel)
                            {
                                var item = array.FirstOrDefault(it => it["NAME"].Value<string>().Equals(info.SELECTVALUE.ToString()));
                                if (item != null)
                                {
                                    value = item["VALUE"].Value<string>();
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        value = GetAddInfoItemValue(objectID, tableID, fieldID);
                        break;
                }
                //if (!string.IsNullOrEmpty(value))
                //    break;
                if (value != string.Empty)
                    break;
            }
            return value;
        }
        //根据fieldID查找表单中formLabelID
        private List<SELECTMODEL> GetFormLabelList(string fieldID)
        {
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_field_id", fieldID);
                    var lst = dbHelper.ExecuteListProc<SELECTMODEL>("pkg_redas_build_doc.sp_form_label_get", dic);

                    return lst;

                }
                catch
                {
                    return new List<SELECTMODEL>();
                }
            }
        }
        /// <summary>
        /// 创建DocMerger的实例
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="structureIds"></param>
        /// <returns></returns>
        public BuildDoc.DocMerger CreateDocMerger(decimal mergerInstanceDocumentID, List<decimal> instanceDocumentIDList, List<decimal> objIdList)
        {
           
            //IsReduction 重新获取时 预估函:不需要查预估函   报告：需要查预估
            //inputParams.Add("IsGetEstimate", isSearch.ToString());

            BuildDoc.DocMerger docMerger = new BuildDoc.DocMerger(mergerInstanceDocumentID, instanceDocumentIDList, objIdList);
            return docMerger;
        }
        #region 文档实例表
        /// <summary>
        /// 往实例文档表中添加数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        //public BaseResult AddInstance_Document(T_INSTANCE_DOCUMENT info, enum_DocumentType t)
        //{
        //    BaseResult br = new BaseResult() { Succeeded = true, Errors = new List<string>() };
        //    using (BaseDB dbHelper = new OmpdDBHelper())
        //    {
        //        try
        //        {
        //            Dictionary<string, object> dic = BaseDB.EntityToDictionary(info);
        //            if (info.INSTANCE_DOCUMENT_ID > 0)
        //            {
        //                dbHelper.ExecuteNonQueryProc("pkg_redas_instance_document.sp_instance_document_modify", dic);
        //                br.ResultId = info.INSTANCE_DOCUMENT_ID;
        //            }
        //            else
        //            {

        //                dbHelper.ExecuteNonQueryProc("pkg_redas_instance_document.sp_instance_document_add", dic);
        //                decimal id = Convert.ToDecimal(dic["O_INSTANCE_DOCUMENT_ID"]);
        //                br.ResultId = id;
        //                //更改项目的实例ID
        //                if (info.Project_ID > 0)
        //                {
        //                    UpdateProjectInstanceDocumentID(info.Project_ID, id, t);
        //                }
        //                if (info.Object_ID > 0)
        //                {
        //                    UpdateObjectInstanceDocumentID(info.Object_ID, id, t);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            br.Succeeded = false;
        //            br.Errors.Add("添加不成功");
        //            throw;
        //        }
        //        return br;
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="t">1:预估函 2：报告</param>
        /// <param name="isSingleObject">是否是单标的物</param>
        /// <param name="isDownLoadSort">是否是多标的物下载操作</param>
        /// <returns></returns>
        public BaseResult AddInstance_Document(T_INSTANCE_DOCUMENT info, EntrustItem t, bool isSingleObject, bool isDownLoadSort)
        {
            BaseResult br = new BaseResult() { Succeeded = true, Errors = new List<string>() };
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = BaseDB.EntityToDictionary(info);
                    if (info.INSTANCE_DOCUMENT_ID > 0)
                    {
                        dbHelper.ExecuteNonQueryProc("pkg_redas_instance_document.sp_instance_document_modify", dic);
                        br.ResultId = info.INSTANCE_DOCUMENT_ID;
                    }
                    else
                    {
                        dbHelper.ExecuteNonQueryProc("pkg_redas_instance_document.sp_instance_document_add", dic);
                        decimal id = Convert.ToDecimal(dic["O_INSTANCE_DOCUMENT_ID"]);
                        br.ResultId = id;
                        //更改项目的实例ID 多标的物的实例保存  只要更新object表InstanceID,多标的物的下载保存  需要更新projectID
                        if (!isSingleObject)
                        {                            //UpdateProjectInstanceDocumentID(info.Project_ID, id, t);
                            if (isDownLoadSort)
                                UpdateInstanceDocumentID(0, info.Project_ID, id, t);
                            else
                                UpdateInstanceDocumentID(info.Object_ID, 0, id, t);
                        }
                        else  //单标的物的预估函即要更新object表InstanceID 也要更新project表的InstanceID
                        {
                            //更改object表
                            UpdateInstanceDocumentID(info.Object_ID, 0, id, t);
                            //更改project表
                            UpdateInstanceDocumentID(0, info.Project_ID, id, t);
                        }                      
                    }
                }
                catch (Exception ex)
                {
                    br.Succeeded = false;
                    br.Errors.Add("添加不成功");
                    throw;
                }
                return br;
            }
        }

        public BaseResult UpdateInstance_Structure(T_INSTANCE_DOCUMENT info)
        {
            BaseResult br = new BaseResult() { Succeeded = true, Errors = new List<string>() };
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = BaseDB.EntityToDictionary(info);
                    if (info.INSTANCE_DOCUMENT_ID > 0)
                    {
                        dbHelper.ExecuteNonQueryProc("pkg_redas_instance_document.sp_instance_modifyStructure", dic);
                        br.ResultId = info.INSTANCE_DOCUMENT_ID;
                    }                  
                }
                catch (Exception ex)
                {
                    br.Succeeded = false;
                    br.Errors.Add("修改不成功");
                    throw;
                }
                return br;
            }
        }

        /// <summary>
        /// 根据估价对象ID获得实例信息
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public T_INSTANCE_DOCUMENT GetInstanceDocumentGetByObject(decimal objectId, EntrustItem businessType)
        {
           T_INSTANCE_DOCUMENT result = null;
           using (BaseDB dbHelper = new OmpdDBHelper())
           {
               try
               {
                   Dictionary<string, object> dic = new Dictionary<string, object>();
                   dic.Add("i_OBJECT_ID", objectId);
                   dic.Add("i_businessType", (int)businessType);
                   List<T_INSTANCE_DOCUMENT> lst = dbHelper.ExecuteListProc<T_INSTANCE_DOCUMENT>("pkg_redas_instance_document.sp_instanceDocumentGetByObject", dic);
                   result = lst[0];
               }
               catch
               {

               }
           }
            return result;
        }
        /// <summary>
        /// 根据文档实例ID获得实例信息
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public T_INSTANCE_DOCUMENT GetInstanceDocumentGetById(decimal instanceDocumentID)
        {
            T_INSTANCE_DOCUMENT result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("I_INSTANCE_DOCUMENT_ID", instanceDocumentID);
                    List<T_INSTANCE_DOCUMENT> lst = dbHelper.ExecuteListProc<T_INSTANCE_DOCUMENT>("pkg_redas_build_doc.sp_instance_document_get", dic);
                    result = lst[0];
                }
                catch
                {

                }
            }
            return result;
        }
        #endregion
        #region 操作估价对象表
        /// <summary>
        ///  保存报告字段信息后，需要更新估价对象/项目的报告实例ID
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private void UpdateInstanceDocumentID(decimal objectId, decimal prjId, decimal instance_Document_ID, EntrustItem t)
        {
            BaseResult br = new BaseResult() { Succeeded = true, Errors = new List<string>() };
            using (BaseDB dbHelper = new RedasDBHelper())
            {

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("i_object_id", objectId);
                dic.Add("i_project_id", prjId);
                dic.Add("i_instanceDocument_id", instance_Document_ID);
                dic.Add("i_type", (int)t);
                dbHelper.ExecuteNonQueryProc("PKG_OBJECT.sp_InstanceDocumentID_update", dic);

            }
        }
        /// <summary>
        /// 更改估价对象表的ReportFileID
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="report_file_id"></param>
        public void UpdateReportFileID(decimal objectId, decimal report_file_id, EntrustItem t)
        {
            BaseResult br = new BaseResult() { Succeeded = true, Errors = new List<string>() };
            using (BaseDB dbHelper = new RedasDBHelper())
            {

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("i_object_id", objectId);
                dic.Add("i_report_file_id", report_file_id);
                dic.Add("i_type",(int)t);
                dbHelper.ExecuteNonQueryProc("PKG_OBJECT.sp_ReportFileID_update", dic);

            }
        }
        #endregion
  

        /// <summary>
        /// 获得文档实例信息
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public T_INSTANCE_DOCUMENT GetInstanceInfo(int instanceId)
        {
            T_INSTANCE_DOCUMENT info = new T_INSTANCE_DOCUMENT(); ;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_INSTANCE_DOCUMENT_ID", instanceId);
                    IList<T_INSTANCE_DOCUMENT> infos = dbHelper.ExecuteListProc<T_INSTANCE_DOCUMENT>("pkg_instance_document.sp_instance_document_get", dic);
                    if (infos.Count > 0)
                        info = infos[0];

                }
                catch
                {
                }
            }
            return info;
        }
        /// <summary>
        /// 根据项目ID获得估价对象集合
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IList<ObjectAndHouseModel> GetObjectByProjectId(int projectId)
        {
            IList<ObjectAndHouseModel> result = null;
            using (BaseDB dbHelper = new RedasDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_project_id", projectId);
                    result = dbHelper.ExecuteListProc<ObjectAndHouseModel>("PKG_OBJECT.sp_objectByProjectId_get", dic);
                }
                catch
                {
                    result = new List<ObjectAndHouseModel>();
                }
            }
            return result;
        }

     
        //获得表单数据
        public FormStoreDTO GetFormStore(decimal? store_id)
        {
            FormStoreDTO result = null;
            using (BaseDB dbHelper = new RedasDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_STORE_ID", store_id);
                    List<FormStoreDTO> list = null;
                    list = dbHelper.ExecuteListProc<FormStoreDTO>("pkg_form_store.sp_form_store_get", dic);

                    if (list.Count > 0)
                        result = list[0];
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return result;
        }
        //获得询价信息中的多个表单的信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type">1:询价  2：查勘</param>
        /// <returns></returns>
        public Dictionary<int, JArray> GetDynamicFormData(decimal objectId,int type)
        {
            ObjectDTO info = GetObjectById(objectId);
            Dictionary<int, JArray> lstFormData = new Dictionary<int, JArray>();

            if (type==1 && info.INQUIRY_FORM_ID > 0) //询价表单Id
            {
                FormStoreDTO formInfo = GetFormStore(info.INQUIRY_FORM_ID);
                JArray arry =JArray.Parse(formInfo.CONTENTS);
                lstFormData.Add(1, arry);
            }
            if (type==2 && info.SURVEY_FORM_ID > 0) //查勘表单Id
            {
                FormStoreDTO formInfo = GetFormStore(info.SURVEY_FORM_ID);
                if (!lstFormData.ContainsKey(Convert.ToInt32(formInfo.INSTANCE_TYPE_ID)))
                {
                    JArray arry = JArray.Parse(formInfo.CONTENTS);
                    lstFormData.Add(2, arry);
                }
            }
            return lstFormData;
        }

        //获得object对象
        public ObjectDTO GetObjectById(decimal objectId)
        {

            ObjectDTO result = null;
            using (BaseDB dbHelper = new RedasDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_object_id", objectId);
                    List<ObjectDTO> list = null;
                    list = dbHelper.ExecuteListProc<ObjectDTO>("PKG_OBJECT.sp_object_get", dic);

                    if (list.Count > 0)
                        result = list[0];
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return result;
        }

        //获得补齐资料的值
        private string GetAddInfoItemValue(decimal objectId, string tableId, string fieldId)
        {

            using (BaseDB dbHelper = new RedasDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_object_id", objectId);
                    dic.Add("i_table_id", tableId);
                    dic.Add("i_field_id", fieldId);
                    var obj = dbHelper.ExecuteScalarProc("pkg_object.sp_AddInfoItemValue_get", dic);
                    return obj == null ? string.Empty : obj.ToString();
                }
                catch
                {
                }
            }
            return string.Empty;
        }
        private string GetBasicData(decimal objectId, string fieldId, string conCode, string buildingCode, string houseCode, string unitCode, string objectType)
        {

            var formView = new FormLogic().GetFormDataByObjectId(Convert.ToInt32(objectId), 1);
            List<Redas.Entities.Ompd.FormLabelDTO> lstLabel = formView.LABLELIST.ToList();
            string value = "";
            using (BaseDB dbHelper = new RedasDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_object_id", objectId);
                    dic.Add("i_unit_code", unitCode);
                    dic.Add("i_house_code", houseCode);
                    dic.Add("i_building_code", buildingCode);
                    dic.Add("i_construction_code", conCode);
                    dic.Add("i_new_purpose_id", objectType);
                    dic.Add("i_field_id", fieldId);
                    value = dbHelper.ExecuteScalarProc("pkg_object.sp_redas_basicData_get", dic).ToString();                  
                }
                catch
                {
                }
            }
            return value;
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append("select f.fsq_field as FIELDNAME,t.fsq_table as TABLENAME from t_redas_db_field f ");
            //strSql.Append("   inner join t_redas_db_table t on f.db_table_id=t.db_table_id  ");
            //strSql.Append(" where f.fsq_field_id=:i_fieldId and t.is_virutal=0");

            //Dictionary<string, object> parameters = new Dictionary<string, object>();

            //parameters["i_fieldId"] = fieldId;
            //var obj = new RedasDBHelper().ExecuteList<STRINGMODEL>(strSql.ToString(), parameters);


            //if (obj.Count > 0)
            //{
            //    strSql = new StringBuilder();
            //    strSql.Append("select " + obj[0].FIELDNAME + " from " + obj[0].TABLENAME + "");
            //}
            //else
            //{
            //}
        }
        #region 生成报告是，更改构件排序所用
        /// <summary>
        /// 根据估价对象ID获得实例信息
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public List<_ChangeSortModel> GetObjectInstanceByProjectId(decimal projectId, EntrustItem businessType, decimal customerId)
        {

            List<_ChangeSortModel> result = new List<_ChangeSortModel>();
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_project_id", projectId);
                    dic.Add("i_businessType", (int)businessType);
                    List<_ObjectInstance> lst = dbHelper.ExecuteListProc<_ObjectInstance>("pkg_redas_instance_document.sp_ObjectInstance_Get", dic);
                    string prjStructure = dic["O_PRJ_STRUCTURE"].ToString();

                    //获得构件的排序
                    IList<Document_structureModel> lstStructure = new Document_structureLogic().GetByCustomer(customerId, 0, 0);
                    Dictionary<decimal, decimal> dicStructureAndSort = new Dictionary<decimal, decimal>(); //优先考虑项目是否已经保存排序，为保存则读取构件中的排序
                    List<_ProjectStructrue> lstPrjStructure = new List<_ProjectStructrue>();

                   //估价对象是否更换模板
                    bool isChangeMotherSet = false;
                    if (!string.IsNullOrEmpty(prjStructure))
                    {                        
                        lstPrjStructure = JsonTools.JsonToObject<List<_ProjectStructrue>>(prjStructure);
                        //如果与数据库的模板不匹配  表示已更换模板，所以排序显示的是构件表中的排序 (多标的物)
                        foreach (var info in lst)
                        {
                            //info.MOTHER_SET_ID
                            if (lstPrjStructure.Where(p => p.MotherSetId == info.MOTHER_SET_ID).Count() <= 0)
                            {
                                isChangeMotherSet = true;
                                break;
                            }
                        }
                    }
                    if (isChangeMotherSet || prjStructure=="")
                    {
                        foreach (var info in lstStructure)
                        {
                            dicStructureAndSort.Add(info.STRUCTURE_ID, info.SORT_NO);
                        }
                    }
                    var sort=0;
                    foreach (var info in lst)
                    {
                        sort++;
                        //实例中的所有构件                  
                        Dictionary<BlockType, List<_DocStructrue>> structureCofig = GetStructureDictionary(info.DOCUMENT_STRUCTURE);
                        foreach (var dicConfig in structureCofig)
                        {                           
                            foreach (var structure in dicConfig.Value)
                            {
                                _ChangeSortModel model = new _ChangeSortModel();
                                model.Object_Name = info.OBJECT_NAME;
                                model.StructureName = structure.Name;
                                model.StructureType = ((int)dicConfig.Key).ToString();
                                model.SturctureId = structure.Key;
                                model.Type = structure.Type;                             
                                model.Sort = 0;
                                //单标的物就按数据库中的顺序来
                                if (lst.Count == 1)
                                {
                                    model.Sort = sort;
                                }
                                else
                                {
                                    //并没有更改模板 且 数据库中已保存排序
                                    if (!isChangeMotherSet && lstPrjStructure.Count > 0)
                                    {
                                        for (var i = 0; i < lstPrjStructure.Count(); i++)
                                        {
                                            if (lstPrjStructure[i].Key == structure.Key && lstPrjStructure[i].InstanceId == info.INSTANCE_DOCUMENT_ID)
                                            {
                                                model.Sort = i;
                                                break;
                                            }
                                        }
                                    }
                                    else if (model.Type == "Config") //只有用户上传的构件才有排序
                                    {

                                        model.Sort = dicStructureAndSort[model.SturctureId];
                                    }
                                }
                                model.Instance_document_id = info.INSTANCE_DOCUMENT_ID;
                                model.MotherSetId = info.MOTHER_SET_ID;
                                result.Add(model);
                            }
                        }
                    }

                    result = result.OrderBy(p => p.StructureType).ThenBy(p => p.Type).ThenBy(p => p.Sort).ToList();
                }
                catch
                {

                }
            }
            return result;
        }
       
        //将模板中的配置信息转换成实体对象
        private Dictionary<BlockType, List<_DocStructrue>> GetStructureDictionary(string json)
        {
            Dictionary<BlockType, List<_DocStructrue>> structureDictionary = new Dictionary<BlockType, List<_DocStructrue>>();
          
            JObject j_structureConfig = JObject.Parse(json);
            string ids = string.Empty;
            foreach (var blockItem in j_structureConfig)
            {
                List<_DocStructrue> structureList = new List<_DocStructrue>();
                structureDictionary.Add((BlockType)Enum.Parse(typeof(BlockType), blockItem.Key), structureList);
                JArray j_structureList = blockItem.Value as JArray;

                foreach (var j_structure in j_structureList)
                {
                    _DocStructrue structure = new _DocStructrue();
                    structure.Type = j_structure["Type"].Value<string>();
                    structure.Key = j_structure["Key"].Value<decimal>();
                    structure.Name = j_structure["Name"].Value<string>();
                    structure.Condition = "";                  
                    structureList.Add(structure);
                }
            }
            return structureDictionary;
        }

        /// <summary>
        ///  保存报告字段信息后，需要更新估价对象的报告实例ID
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private void UpdateProjectInstanceDocumentID(decimal projectId, decimal instance_Document_ID, EntrustItem t)
        {
            BaseResult br = new BaseResult() { Succeeded = true, Errors = new List<string>() };
            using (BaseDB dbHelper = new RedasDBHelper())
            {

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("i_project_id", projectId);
                dic.Add("i_instanceDocument_id", instance_Document_ID);
                dic.Add("i_type", (int)t);
                dbHelper.ExecuteNonQueryProc("pkg_project.sp_InstanceDocumentID_update", dic);
            }
        }
        #endregion

    }
    public class STRINGMODEL
    {
        public string TABLENAME { get; set; }    
        public string FIELDNAME { get; set; }
    }
}
