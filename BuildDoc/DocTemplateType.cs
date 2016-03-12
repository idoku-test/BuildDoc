namespace BuildDoc 
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// ģ������
    /// </summary>
    public class DocTemplateType
    {
        /// <summary>
        /// SQL
        /// </summary>
        private string sql;

        /// <summary>
        /// ģ������ID
        /// </summary>
        private decimal templateTypeID;

        /// <summary>
        /// ʵ��ID
        /// </summary>
        private decimal instanceID;

        /// <summary>
        /// ������ʽ���ʽ
        /// </summary>
        private Regex keyRegex = new Regex(@"@\w+", RegexOptions.Compiled);

        /// <summary>
        /// ����Դ�б�
        /// </summary>
        public List<DataSource> DataSourceList
        {
            get;
            set;
        }

        /// <summary>
        /// ģ����������
        /// </summary>
        public string DocTemplateTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark
        {
            get;
            set;
        }

        // public bool IsComponent;

        /// <summary>
        /// ��������
        /// </summary>
        public Dictionary<string, string> ParamsCache
        {
            get;
            set;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public DataSet CacheData
        {
            get;
            set;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="templateTypeID">ģ������ID</param>
        /// <param name="instanceID">ʵ��ID</param>
        /// <param name="inputParams">�༭����</param>
        public DocTemplateType(decimal templateTypeID, decimal instanceID, Dictionary<string, string> inputParams)
        {
            this.CacheData = new DataSet();

            // �����ݿ��ȡDocTemplateType�������
            this.templateTypeID = templateTypeID;
            this.instanceID = instanceID;
            this.ParamsCache = this.GetParams();
            if (inputParams != null)
            {
                foreach (var item in inputParams)
                {
                    if (!this.ParamsCache.ContainsKey(item.Key))
                    {
                        this.ParamsCache.Add(item.Key, item.Value);
                    }
                }
            }

            this.GetDataSourceList();        
        }

        /// <summary>
        /// ��ȡ����Դ
        /// </summary>
        private void GetDataSourceList()
        {
            try
            {
                //this.DataSourceList = new List<DataSource>();
                //using (BaseDB dbHelper = new OmpdDBHelper())
                //{
                //    Dictionary<string, object> dicParms = new Dictionary<string, object>();
                //    dicParms.Add("i_template_type", this.templateTypeID);
                //    DataTable dt = dbHelper.ExecuteDataTableProc("pkg_redas_build_doc.sp_data_source_get", dicParms);
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        string db_name = string.Empty;
                //        string data_source_name = string.Empty;
                //        string multi_select_field = string.Empty;
                //        string sql_content = string.Empty;
                //        foreach (DataRow dr in dt.Rows)
                //        {
                //            db_name = dr["FSQ_DB_NAME"].ToString();
                //            data_source_name = dr["DATA_SOURCE_NAME"].ToString();
                //            multi_select_field = dr["MULTI_SELECT_FIELD"].ToString();
                //            sql_content = dr["SQL_CONTENT"].ToString();
                //            this.DataSourceList.Add(new DataSource(db_name, data_source_name, data_source_name, false, sql_content, this));
                //        }
                //    }

                //    dt.Dispose();
                //}
            }
            catch {
                System.Console.Write("��ȡ����Դ����");
            }
        }

        /// <summary>
        /// ����m_SQL,��ȡ����,���к���Ӧ��ֵ���ɲ���
        /// </summary>
        /// <returns>���ز���</returns>
        public Dictionary<string, string> GetParams()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            //using (BaseDB dbHelper = new OmpdDBHelper())
            //{
            //    Dictionary<string, object> dicParms = new Dictionary<string, object>();
            //    dicParms.Add("i_template_type", this.templateTypeID);
            //    DataTable dt = dbHelper.ExecuteDataTableProc("pkg_redas_build_doc.sp_template_type_get", dicParms);
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        this.DocTemplateTypeName = dt.Rows[0]["TEMPLATE_TYPE_NAME"].ToString();
            //        this.sql = dt.Rows[0]["SQL_CONTENT"].ToString();
            //        if (this.keyRegex.IsMatch(this.sql))
            //        {
            //            this.sql = this.keyRegex.Replace(this.sql, this.instanceID.ToString());
            //        }

            //        using (BaseDB redasHelper = new RedasDBHelper())
            //        {
            //            DataTable tmpData = redasHelper.ExecuteDataTable(this.sql, null);
            //            if (tmpData != null && tmpData.Rows.Count > 0)
            //            {
            //                foreach (DataColumn col in tmpData.Columns)
            //                {
            //                    dic.Add(col.ColumnName, tmpData.Rows[0][col.ColumnName].ToString());
            //                }
            //            }

            //            tmpData.Dispose();
            //        }
            //    }

            //    dt.Dispose();
            //}

            return dic;
        }
    } // end DocTemplateType
} // end namespace