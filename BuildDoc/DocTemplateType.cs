namespace BuildDoc 
{
    using BuildDoc.Entities;
    using BuildDoc.Logic;
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
        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }

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
                this.DataSourceList = new List<DataSource>();
                IList<DataSourceDTO> dataSource = BuildWordInstance.GetDataSource((int)this.templateTypeID);
                if (dataSource != null)
                {
                    foreach (var ds in dataSource)
                    {
                        this.DataSourceList.Add(
                            new DataSource(ds.FSQ_DB_NAME, ds.DATA_SOURCE_NAME, ds.DATA_SOURCE_NAME, false,
                                ds.SQL_CONTENT, this)
                            );
                    }
                }
                 
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
            TemplateTypeDTO templateType = BuildWordInstance.GetTemplateType((int)this.templateTypeID);
            if (templateType != null)
            {
                this.DocTemplateTypeName = templateType.TEMPLATE_TYPE_NAME;
                this.sql = templateType.SQL_CONTENT;
                if (this.keyRegex.IsMatch(this.sql))
                    this.sql = this.keyRegex.Replace(this.sql, this.instanceID.ToString());
                dic = BuildWordInstance.GetTemplateParms(this.sql);
            }
            return dic;
        }
    } // end DocTemplateType
} // end namespace