namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 数据源类
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// 模板类型
        /// </summary>
        private DocTemplateType docTemplateType;

        /// <summary>
        /// SQL语句
        /// </summary>
        private string sql;

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName
        {
            get;
            set;
        }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DataSourceName
        {
            get;
            set;
        }

        /// <summary>
        /// 数据源中文名称
        /// </summary>
        public string DataSourceCNName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否多值数据
        /// </summary>
        public bool IsMultiItem
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="dataSourceName">数据源名称</param>
        /// <param name="dataSourceCNName">数据中文名称</param>
        /// <param name="isMultiItem">是否多值数据</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="docTemplateType">模板类型</param>
        public DataSource(string dbName, string dataSourceName, string dataSourceCNName, bool isMultiItem, string sql, DocTemplateType docTemplateType)
        {
            this.DBName = dbName;
            this.DataSourceName = dataSourceName;
            this.DataSourceCNName = dataSourceCNName;
            this.IsMultiItem = isMultiItem;
            this.docTemplateType = docTemplateType;
            this.sql = this.BuildSql(sql, this.docTemplateType.ParamsCache);
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            // 资源释放
        }

        /// <summary>
        /// 根据DocTemplateType生成的参数,替换m_SQL中的占位符
        /// </summary>
        /// <param name="sql">带点位符SQL语句</param>
        /// <param name="parames">DocTemplateType生成的参数</param>
        /// <returns>SQL语句</returns>
        private string BuildSql(string sql, Dictionary<string, string> parames)
        {
            foreach (var dic in parames)
            {
                sql = Regex.Replace(sql, string.Format("@{0}", dic.Key), dic.Value, RegexOptions.IgnoreCase);
            }

            return sql;
        }

        /// <summary>
        /// 从CacheData获取,不存在则根据SQL变量创建
        /// </summary>
        /// <returns>数据</returns>
        public DataTable GetDataTable()
        {

            DataTable dt = new DataTable();
            try
            {
                string cacheKey = string.Format("{0}_{1}", this.DBName, this.DataSourceName);
                dt = this.docTemplateType.CacheData.Tables[cacheKey];
                if (dt == null && this.sql.IndexOf("@") == -1)
                {
                    // 根据m_SQL获取
                    //using (BaseDB dbHelper = this.CreateDBHelper())
                    //{
                    //    dt = dbHelper.ExecuteDataTable(this.sql, null);
                    //    dt.TableName = cacheKey;
                    //    this.docTemplateType.CacheData.Tables.Add(dt);
                    //}
                }
            }
            catch
            {
                //System.Console.Write(this.DataSourceName + "的数据源取值异常");
                //throw;
            }

            return dt;
        }

        /// <summary>
        /// 创建数据访问类
        /// </summary>
        /// <returns>数据访问类</returns>
        //private BaseDB CreateDBHelper()
        //{
        //    BaseDB baseDB = null;
        //    switch (this.DBName)
        //    {
        //        case "redas":
        //            baseDB = new RedasDBHelper();
        //            break;
        //        case "ompd":
        //            baseDB = new OmpdDBHelper();
        //            break;
        //    }

        //    return baseDB;
        //}

        /// <summary>
        /// 获取字段列表
        /// </summary>
        /// <returns>字段列表</returns>
        public List<string> GetFieldList()
        {
            List<string> fieldList = new List<string>();

            // 根据DataTable列的信息初始化
            DataTable data = this.GetDataTable();
            foreach (DataColumn column in data.Columns)
            {
                fieldList.Add(column.ColumnName);
            }

            return fieldList;
        }

        /// <summary>
        /// 根据传入的参数获取数据
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="filterFieldName">过滤字段名称</param>
        /// <param name="filterOperation">过滤字段操作符</param>
        /// <param name="filterValue">过滤字段使用值</param>
        /// <returns>值</returns>
        public string GetValue(string fieldName, string filterFieldName = "", string filterOperation = "", string filterValue = "")
        {
            DataTable data = this.GetDataTable();

            if (data == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(filterFieldName) && !string.IsNullOrEmpty(filterOperation) && !string.IsNullOrEmpty(filterValue))
            {
                var drs = data.Select(string.Format("{0}{1}'{2}'", filterFieldName, filterOperation, filterValue));
                if (drs.Length > 0)
                {
                    return drs[0][fieldName].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (data.Rows.Count == 1)
            {
                return data.Rows[0][fieldName].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据传入的参数获取二维数组
        /// </summary>
        /// <param name="columns">列集合</param>
        /// <param name="filterFieldName">过滤字段名称</param>
        /// <param name="filterOperation">过滤操作符</param>
        /// <param name="filterValue">过滤值</param>
        /// <param name="from">从数据集第几开始</param>
        /// <param name="to">到数据集第几开始</param>
        /// <returns>返回数组</returns>
        public string[,] GetArrayValue(List<ColumnInfo> columns, string filterFieldName, string filterOperation, string filterValue, int from, int to)
        {
            string[,] strAry;
            DataTable data = this.GetDataTable();
            DataRow[] dataRows;

            if (data != null && data.Rows.Count > 0 && columns.Count > 0)
            {
                try
                {
                    if ( !string.IsNullOrEmpty(filterFieldName) &&!string.IsNullOrEmpty(filterOperation) && !string.IsNullOrEmpty(filterValue))
                    {
                        dataRows = data.Select(string.Format("{0}{1}'{2}'", filterFieldName, filterOperation, filterValue));
                    }
                    else
                    {
                        dataRows = data.Select();
                    }

                    if (dataRows == null)
                    {
                        return new string[0, 0];
                    }

                    if (from > 0)
                    {
                        if (to > 0)
                        {
                            data = dataRows.Skip(from).Take(to - from).CopyToDataTable();
                        }
                        else
                        {
                            data = dataRows.Skip(from).CopyToDataTable();
                        }
                    }
                    else
                    {
                        if (to > 0)
                        {
                            data = dataRows.Take(to).CopyToDataTable();
                        }
                        else
                        {
                            data = dataRows.CopyToDataTable();
                        }
                    }

                    strAry = new string[data.Rows.Count, columns.Count];
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        for (int j = 0; j < columns.Count; j++)
                        {
                            if (columns[j].FormatInfo != null && columns[j].FormatInfo.HasValues)
                            {
                                strAry[i, j] = columns[j].FormatInfo.ToFormatString(data.Rows[i][columns[j].FieldName].ToString());
                            }
                            else
                            {
                                strAry[i, j] = data.Rows[i][columns[j].FieldName].ToString();
                            }
                        }
                    }

                    return strAry;
                }
                catch
                {
                }
            }

            return new string[0, 0];
        }

        /// <summary>
        /// 根据传入的参数获取健值对
        /// </summary>
        /// <param name="textField">文本字段</param>
        /// <param name="valueField">值字段</param>
        /// <param name="filterFieldName">过滤字段名称</param>
        /// <param name="filterOperation">过滤字段操作符</param>
        /// <param name="filterValue">过滤字段使用值</param>
        /// <param name="top">前几</param>
        /// <returns>字典集合</returns>
        public Dictionary<string, string> GetDictionary(string textField, string valueField, string filterFieldName, string filterOperation, string filterValue, int top)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DataTable data = this.GetDataTable();
            DataRow[] dataRows;

            if (data != null && data.Rows.Count > 0)
            {
                if ( !string.IsNullOrEmpty(filterFieldName) && !string.IsNullOrEmpty(filterOperation) && !string.IsNullOrEmpty(filterValue))
                {
                    dataRows = data.Select(string.Format("{0}{1}'{2}'", filterFieldName, filterOperation, filterValue));
                }
                else
                {
                    dataRows = data.Select();
                }

                if (dataRows == null)
                {
                    return dic;
                }

                if (top > 0)
                {
                    data = dataRows.Take(top).CopyToDataTable();
                }
                else
                {
                    data = dataRows.CopyToDataTable();
                }

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    dic.Add(data.Rows[i][valueField].ToString(), data.Rows[i][textField].ToString());
                }
            }

            return dic;
        }

        /// <summary>
        /// 获取字典文本
        /// </summary>
        /// <param name="textField">文本字段</param>
        /// <param name="valueField">值字段</param>
        /// <param name="value">字典值</param>
        /// <returns>返回字典文本</returns>
        public string GetDictionaryText(string textField, string valueField, string value)
        {
            string result = string.Empty;
            int tmpValue = 0;

            // 暂时方法(多数据源有的是值，有的是文本)
            string[] values = value.Split(',');
            DataTable data = this.GetDataTable();
            DataRow[] dataRows;
            foreach (string v in values)
            {
                if (int.TryParse(v, out tmpValue))
                {
                    if (data != null && data.Rows.Count > 0)
                    {
                        dataRows = data.Select(string.Format("{0}{1}'{2}'", valueField, "=", v));
                        if (dataRows == null || dataRows.Length == 0)
                        {
                            return value;
                        }

                        if (dataRows.Length > 0)
                        {
                            result = string.Format("{0}，{1}", result, dataRows[0][textField].ToString());
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                return value;
            }
            else
            {
                return result.Trim('，');
            }
        }

        /// <summary>
        /// 获取数据标签列表
        /// </summary>
        /// <returns>数据标签列表</returns>
        public List<DataLabel> GetDataLabelList()
        {
            return new List<DataLabel>();
        }
    } // end DataSource
} // end namespace