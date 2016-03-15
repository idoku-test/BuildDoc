using BuildDoc.Entities;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public class BuildDocLogic:IBuildDocLogic
    {
        /// <summary>
        /// 获得数据源
        /// </summary>
        /// <param name="type">-1获得所有的数据源  1：获得除数据转换之外的数据源  </param>
        /// <returns></returns>
        public IList<DataSourceDTO> GetData_Source(int type)
        {
            IList<DataSourceDTO> result = null;
            using (BaseDB dbHelper = new OmpdDBHelper())
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("i_Type", type);
                    result = dbHelper.ExecuteListProc<DataSourceDTO>("PKG_UCS_DataSource.sp_Data_Source_get", dic);

                    // Regex regex = new Regex(@"\([^\,]+\""", RegexOptions.IgnoreCase);                 
                    IList<DataSourceDTO> lstItem;
                    //提取sql中的参数
                    foreach (var info in result)
                    {
                        //MatchCollection mc = regex.Matches(info.SQL_CONTENT.Replace("\r\n", ""));
                        info.SQL_CONTENT = info.SQL_CONTENT.ToLower();
                        int start = info.SQL_CONTENT.IndexOf("select") + 6;
                        int len = info.SQL_CONTENT.IndexOf("from") - start - 1;

                        string betweenStr = info.SQL_CONTENT.Substring(info.SQL_CONTENT.LastIndexOf(" as "), info.SQL_CONTENT.LastIndexOf("from") - info.SQL_CONTENT.LastIndexOf(" as "));
                        if (info.SQL_CONTENT.IndexOf("(select") > 0 && betweenStr.IndexOf("select") == -1)
                        {
                            len = info.SQL_CONTENT.LastIndexOf("from") - start - 1;
                        }
                        string[] lstArry = info.SQL_CONTENT.Substring(start, len).Split(',');
                        //lstItem = new List<SelectItem>();
                        //foreach (var ary in lstArry)
                        //{
                        //    SelectItem item = new SelectItem();
                        //    string[] keyValue = System.Text.RegularExpressions.Regex.Split(ary, " as ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        //    if (keyValue.Length == 2)
                        //    {
                        //        item.SelectText = keyValue[1].Trim().Replace("\"", "");
                        //        item.SelectValue = keyValue[0].Trim().Replace("(", "").Replace(")", "");
                        //        if (lstItem.Where(p => p.SelectText == item.SelectText).Count() <= 0)
                        //            lstItem.Add(item);
                        //    }
                        //}

                        //info.SourceFields = lstItem;
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
