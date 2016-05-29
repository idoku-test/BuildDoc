using BuildDoc.Entities;
using DataAccess;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public class BuildDocLogic:IBuildDocLogic
    {

        private static IBuildDocLogic _instance;
        public static IBuildDocLogic CreateInstance()
        {
            IApplicationContext context = ContextRegistry.GetContext();
            if (_instance == null && context != null)
                _instance = context.GetObject("IBuildDocLogic") as IBuildDocLogic;
            return _instance;
        }

        #region data source
        /// <summary>
        /// 获得数据源
        /// </summary>
        /// <param name="type">-1获得所有的数据源  1：获得除数据转换之外的数据源  </param>
        /// <returns></returns>
        public IList<DataSourceDTO> GetDataSource(int type)
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
                                if(!fields.Contains(mc.Value))
                                    fields.Add(mc.Value.Replace("\"",""));
                            }
                            info.Fields = fields;
                        }
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }
            return result;
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
                    result = new List<MotherSetDTO>();
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
    }
}
