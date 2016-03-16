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
                    var fields = new List<string>();
                    foreach (var info in result)
                    {
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

     
    }
}
