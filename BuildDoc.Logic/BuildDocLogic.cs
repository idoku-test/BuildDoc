using BuildDoc.Entities;
using DataAccess;
using Newtonsoft.Json.Linq;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public class BuildDocLogic : IBuildDocLogic
    {

        private static IBuildDocLogic _instance;
        public static IBuildDocLogic CreateInstance()
        {
            IApplicationContext context = ContextRegistry.GetContext();
            if (_instance == null && context != null)
                _instance = context.GetObject("IBuildDocLogic") as IBuildDocLogic;
            return _instance;
        }

        #region template
        public TemplateTypeDTO GetTemplateType(int templateTypeId)
        {
            TemplateTypeDTO result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dicParms = new Dictionary<string, object>();
                    dicParms.Add("i_template_type", templateTypeId);
                    var templateTypeList = dbHelper.ExecuteListProc<TemplateTypeDTO>("pkg_redas_build_doc.sp_template_type_get", dicParms);
                    if (templateTypeList.Count > 0)
                    {
                        result = templateTypeList[0];
                    }
                }
                catch { 
                
                }
                return result;
            }
        }

        public Dictionary<string, string> GetTemplateParms(string sql)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            using (BaseDB redasHelper = new RedasDBHelper())
            {
                DataTable tmpData = redasHelper.ExecuteDataTable(sql, null);
                if (tmpData != null && tmpData.Rows.Count > 0)
                {
                    foreach (DataColumn col in tmpData.Columns)
                    {
                        dic.Add(col.ColumnName, tmpData.Rows[0][col.ColumnName].ToString());
                    }
                }

                tmpData.Dispose();
            }
            return dic;
        }
        #endregion

        #region data source
        /// <summary>
        /// 获取所有数据源
        /// </summary>
        /// <returns></returns>
        public IList<DataSourceDTO> GetAllDataSource()
        {
            IList<DataSourceDTO> result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_Type", -1);
                    result = dbHelper.ExecuteListProc<DataSourceDTO>("PKG_UCS_DataSource.sp_Data_Source_get", dic);                 
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public IList<DataSourceDTO> GetDataSource(int templateTypeId)
        {
            IList<DataSourceDTO> result = new List<DataSourceDTO>();
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_template_type", templateTypeId);
                    result = dbHelper.ExecuteListProc<DataSourceDTO>("pkg_redas_build_doc.sp_data_source_get", dic);
                    
                }
                catch (Exception ex)
                {
                    result = null;
                }
            }
            return result;
        }

        /// <summary>
        /// 获得数据源
        /// </summary>
        /// <param name="type">-1获得所有的数据源  1：获得除数据转换之外的数据源  </param>
        /// <returns></returns>
        public IList<DataSourceDTO> GetDataSourceByType(int type)
        {
            IList<DataSourceDTO> result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_Type", type);
                    result = dbHelper.ExecuteListProc<DataSourceDTO>("PKG_UCS_DataSource.sp_Data_Source_get", dic);

                    //提取sql中的参数
                    Regex reg = new Regex(@"(?<=as\b).*?(?=,)");

                    foreach (var info in result)
                    {
                        var fields = new List<string>();
                        if (reg.IsMatch(info.SQL_CONTENT))
                        {
                            var matches = reg.Matches(info.SQL_CONTENT);
                            foreach (Match mc in matches)
                            {
                                if (!fields.Contains(mc.Value))
                                    fields.Add(mc.Value.Replace("\"", ""));
                            }
                            info.Fields = fields;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataSource(string dbName, string sql)
        {
            DataTable dt = new DataTable();
            // 根据m_SQL获取
            using (BaseDB dbHelper = CreateDBHelper(dbName))
            {
                try
                {
                    dt = dbHelper.ExecuteDataTable(sql, null);
                }
                catch
                {
                    dt = null;
                }
            }
            return dt;
        }

        /// <summary>
        /// 获得数据处理的数据源
        /// </summary>
        /// <returns></returns>
        public IList<LabelDealWithModel> GetLabelDealSource()
        {
            List<LabelDealWithModel> list = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    list = dbHelper.ExecuteListProc<LabelDealWithModel>("PKG_UCS_DataSource.sp_label_deal_with_get", dic);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return list;
        }

        #endregion

        #region label
        /// <summary>
        /// 保存标签信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult SaveLabel(DataLabelModel model)
        {
            BaseResult result = new BaseResult() { Succeeded = true };
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = BaseDB.EntityToDictionary(model);
                    if (model.DATA_LABEL_ID == 0)
                    {
                        model.CREATED_TIME = DateTime.Now;
                        dbHelper.ExecuteNonQueryProc("PKG_UCS_DATA_LABEL.sp_data_label_add", dic);
                    }
                    else
                    {
                        model.MODIFIED_TIME = DateTime.Now;
                        dbHelper.ExecuteNonQueryProc("PKG_UCS_DATA_LABEL.sp_data_label_modify", dic);
                    }
                }
                catch (Exception ex)
                {
                    result.Succeeded = false;
                    result.Errors.Add(ex.Message);
                    throw;
                }
            }
            return result;
        }

        public DataLabelModel GetLabel(int customerId, string dataLabelName)
        {
            DataLabelModel result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                List<DataLabelModel> list = null;
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_LABEL_NAME", dataLabelName);
                    dic.Add("i_CUSTOMER_ID", customerId);
                    list = dbHelper.ExecuteListProc<DataLabelModel>("PKG_UCS_DATA_LABEL.sp_data_label_getInfoByName", dic);
                }
                catch
                {

                }
                if (list.Count > 0)
                    result = list[0];
            }
            return result;
        }
        #endregion

        #region motherSet
        public IList<MotherSetDTO> GetMotherSetByCustomer(int customerId, int type)
        {
            IList<MotherSetDTO> result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_customer_ID", customerId);
                    dic.Add("i_document_type", type);
                    result = dbHelper.ExecuteListProc<MotherSetDTO>("pkg_redas_mother_set.sp_mother_set_getByCustomer", dic);

                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }

        public MotherSetDTO GetMotherSet(int motherId)
        {
            MotherSetDTO result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_MOTHER_SET_ID", motherId);
                    List<MotherSetDTO> list = null;
                    list = dbHelper.ExecuteListProc<MotherSetDTO>("pkg_redas_mother_set.sp_mother_set_get", dic);
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
        #endregion

        #region structure
        public DocumentStructureDTO GetStructure(int structureId)
        {
            DocumentStructureDTO result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dicParms = new Dictionary<string, object>();
                    dicParms.Add("i_structure_id", structureId);
                    var list = dbHelper.ExecuteListProc<DocumentStructureDTO>("pkg_redas_build_doc.sp_doc_structure_get", dicParms);
                    if (list.Count() > 0)
                    {
                        result = list[0];
                    }
                }
                catch
                {
                    result = null;
                }

            }
            return result;
        }
        #endregion

        #region instance
        /// <summary>
        /// 根据文档实例ID获得实例信息
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public InstanceDocumentDTO GetInstanceDocument(decimal instanceDocumentID)
        {
            InstanceDocumentDTO result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("I_INSTANCE_DOCUMENT_ID", instanceDocumentID);
                    List<InstanceDocumentDTO> lst = dbHelper.ExecuteListProc<InstanceDocumentDTO>("pkg_redas_build_doc.sp_instance_document_get", dic);
                    if (lst.Count() > 0)
                    {
                        result = lst[0];
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }
        #endregion

        #region helper


        /// <summary>
        /// 创建数据访问类
        /// </summary>
        /// <returns>数据访问类</returns>
        private BaseDB CreateDBHelper(string dbName)
        {
            BaseDB baseDB = null;
            switch (dbName)
            {
                case "redas":
                    baseDB = new RedasDBHelper();
                    break;
                case "ompd":
                    baseDB = new OmpdDBHelper();
                    break;
            }
            return baseDB;
        }
        #endregion

        #region garbage     

        /// <summary>
        /// 0:基础数据 1:询价 2:预估函 3:查勘 40031002:资料整理 40031001:资料整理
        /// </summary>
        // string sourceConfig = "[{ Type:40031001 },{ Type:40031002 },{ Type:3 },{ Type:2 },{ Type:1 },{ Type:0 }]";
        string sourceConfig = "[{ Type:40031001 },{ Type:2 },{ Type:3 },{ Type:1 },{ Type:0 }]";
        // 2015.3.8会议决议  1：资料补齐  2:预估函  3:查勘   4：询价  5：基础数据
      
        public string GetValue(decimal objectID, string tableID, string fieldID, decimal structureID, string labelName, Dictionary<string, string> parame)
        {
            var configList = JArray.Parse(sourceConfig);
            string value = string.Empty;
            var formLabelValue = GetFormLabelList(fieldID);

            JArray dynArray = new JArray();
            JToken dynToken;

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
                             dynArray = GetDynamicFormData(objectID, 40026001);
                             dynToken = dynArray.FirstOrDefault(it => it["NAME"].Value<string>().Equals(formLabelValue));
                             if (dynToken != null)
                            {
                                value = dynToken["VALUE"].Value<string>();
                                break;
                            }
                        break;
                    case 2: //预估函       
                        //if (isGetEstimate)   // 需要查询预估函
                        //{
                        //    T_INSTANCE_DOCUMENT instanceInfo = GetInstanceDocumentGetByObject(objectID, EntrustItem.Estimate);

                        //    if (instanceInfo != null && instanceInfo.MANUAL_EDITING_RETURN != "")
                        //    {
                        //        List<_EditReturnModel> lstEditModel = JsonTools.JsonToObject2<List<_EditReturnModel>>(instanceInfo.MANUAL_EDITING_RETURN);
                        //        _EditReturnModel labelInfo = lstEditModel.Find(delegate(_EditReturnModel p)
                        //        {
                        //            //return p.ID == structureID && p.LabelName == labelName;
                        //            return p.LabelName == labelName;
                        //        });
                        //        if (labelInfo == null)
                        //            continue;
                        //        else
                        //            value = labelInfo.Value;
                        //    }
                        //}
                        break;
                    case 3:
                            dynArray = GetDynamicFormData(objectID, 40026002);
                            dynToken = dynArray.FirstOrDefault(it => it["NAME"].Value<string>().Equals(formLabelValue));
                            if (dynToken != null)
                            {
                                value = dynToken["VALUE"].Value<string>();
                                break;
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


        private string GetFormLabelList(string fieldID)
        {
            string value = "";
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_field_id", fieldID);                    
                    value = dbHelper.ExecuteScalarProc("pkg_redas_build_doc.sp_form_label_get", dic).ToString();
                    return value;

                }
                catch
                {
                   
                }
            }
            return value;
        }

        private string GetBasicData(decimal objectId, string fieldId, string conCode, string buildingCode, string houseCode, string unitCode, string objectType)
        {
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
        }

        //
        /// <summary>
        /// 获得询价信息中的多个表单的信息
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type">1:询价  2：查勘</param>
        /// <returns></returns>
        public JArray GetDynamicFormData(decimal objectId, int type)
        {
            JArray jArray = new JArray();
            using (BaseDB dbHelper = new RedasDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_object_id", objectId);
                    dic.Add("i_type", type);
                    var obj = dbHelper.ExecuteScalarProc("pkg_object.sp_redas_formData_get", dic);
                    if (obj != null)
                    {
                        jArray = JArray.Parse(obj.ToString());
                    }
                }
                catch
                {
                    throw;
                }
            }
            return jArray;
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
        #endregion
    }
}



       