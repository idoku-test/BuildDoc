namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// ����Դ��
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// ģ������
        /// </summary>
        private DocTemplateType docTemplateType;

        /// <summary>
        /// SQL���
        /// </summary>
        private string sql;

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string DBName
        {
            get;
            set;
        }

        /// <summary>
        /// ����Դ����
        /// </summary>
        public string DataSourceName
        {
            get;
            set;
        }

        /// <summary>
        /// ����Դ��������
        /// </summary>
        public string DataSourceCNName
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ��ֵ����
        /// </summary>
        public bool IsMultiItem
        {
            get;
            set;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="dbName">���ݿ�����</param>
        /// <param name="dataSourceName">����Դ����</param>
        /// <param name="dataSourceCNName">������������</param>
        /// <param name="isMultiItem">�Ƿ��ֵ����</param>
        /// <param name="sql">SQL���</param>
        /// <param name="docTemplateType">ģ������</param>
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
        /// ��Դ�ͷ�
        /// </summary>
        public virtual void Dispose()
        {
            // ��Դ�ͷ�
        }

        /// <summary>
        /// ����DocTemplateType���ɵĲ���,�滻m_SQL�е�ռλ��
        /// </summary>
        /// <param name="sql">����λ��SQL���</param>
        /// <param name="parames">DocTemplateType���ɵĲ���</param>
        /// <returns>SQL���</returns>
        private string BuildSql(string sql, Dictionary<string, string> parames)
        {
            foreach (var dic in parames)
            {
                sql = Regex.Replace(sql, string.Format("@{0}", dic.Key), dic.Value, RegexOptions.IgnoreCase);
            }

            return sql;
        }

        /// <summary>
        /// ��CacheData��ȡ,�����������SQL��������
        /// </summary>
        /// <returns>����</returns>
        public DataTable GetDataTable()
        {

            DataTable dt = new DataTable();
            try
            {
                string cacheKey = string.Format("{0}_{1}", this.DBName, this.DataSourceName);
                dt = this.docTemplateType.CacheData.Tables[cacheKey];
                if (dt == null && this.sql.IndexOf("@") == -1)
                {
                    // ����m_SQL��ȡ
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
                //System.Console.Write(this.DataSourceName + "������Դȡֵ�쳣");
                //throw;
            }

            return dt;
        }

        /// <summary>
        /// �������ݷ�����
        /// </summary>
        /// <returns>���ݷ�����</returns>
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
        /// ��ȡ�ֶ��б�
        /// </summary>
        /// <returns>�ֶ��б�</returns>
        public List<string> GetFieldList()
        {
            List<string> fieldList = new List<string>();

            // ����DataTable�е���Ϣ��ʼ��
            DataTable data = this.GetDataTable();
            foreach (DataColumn column in data.Columns)
            {
                fieldList.Add(column.ColumnName);
            }

            return fieldList;
        }

        /// <summary>
        /// ���ݴ���Ĳ�����ȡ����
        /// </summary>
        /// <param name="fieldName">�ֶ�����</param>
        /// <param name="filterFieldName">�����ֶ�����</param>
        /// <param name="filterOperation">�����ֶβ�����</param>
        /// <param name="filterValue">�����ֶ�ʹ��ֵ</param>
        /// <returns>ֵ</returns>
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
        /// ���ݴ���Ĳ�����ȡ��ά����
        /// </summary>
        /// <param name="columns">�м���</param>
        /// <param name="filterFieldName">�����ֶ�����</param>
        /// <param name="filterOperation">���˲�����</param>
        /// <param name="filterValue">����ֵ</param>
        /// <param name="from">�����ݼ��ڼ���ʼ</param>
        /// <param name="to">�����ݼ��ڼ���ʼ</param>
        /// <returns>��������</returns>
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
        /// ���ݴ���Ĳ�����ȡ��ֵ��
        /// </summary>
        /// <param name="textField">�ı��ֶ�</param>
        /// <param name="valueField">ֵ�ֶ�</param>
        /// <param name="filterFieldName">�����ֶ�����</param>
        /// <param name="filterOperation">�����ֶβ�����</param>
        /// <param name="filterValue">�����ֶ�ʹ��ֵ</param>
        /// <param name="top">ǰ��</param>
        /// <returns>�ֵ伯��</returns>
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
        /// ��ȡ�ֵ��ı�
        /// </summary>
        /// <param name="textField">�ı��ֶ�</param>
        /// <param name="valueField">ֵ�ֶ�</param>
        /// <param name="value">�ֵ�ֵ</param>
        /// <returns>�����ֵ��ı�</returns>
        public string GetDictionaryText(string textField, string valueField, string value)
        {
            string result = string.Empty;
            int tmpValue = 0;

            // ��ʱ����(������Դ�е���ֵ���е����ı�)
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
                            result = string.Format("{0}��{1}", result, dataRows[0][textField].ToString());
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
                return result.Trim('��');
            }
        }

        /// <summary>
        /// ��ȡ���ݱ�ǩ�б�
        /// </summary>
        /// <returns>���ݱ�ǩ�б�</returns>
        public List<DataLabel> GetDataLabelList()
        {
            return new List<DataLabel>();
        }
    } // end DataSource
} // end namespace