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
    /// 模板类型
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
        /// 模板类型ID
        /// </summary>
        private decimal templateTypeID;

        /// <summary>
        /// 实例ID
        /// </summary>
        private decimal instanceID;

        /// <summary>
        /// 主键正式表达式
        /// </summary>
        private Regex keyRegex = new Regex(@"@\w+", RegexOptions.Compiled);

        /// <summary>
        /// 数据源列表
        /// </summary>
        public List<DataSource> DataSourceList
        {
            get;
            set;
        }

        /// <summary>
        /// 模板类型名称
        /// </summary>
        public string DocTemplateTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }

        // public bool IsComponent;

        /// <summary>
        /// 参数缓存
        /// </summary>
        public Dictionary<string, string> ParamsCache
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存数据
        /// </summary>
        public DataSet CacheData
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="templateTypeID">模板类型ID</param>
        /// <param name="instanceID">实例ID</param>
        /// <param name="inputParams">编辑参数</param>
        public DocTemplateType(decimal templateTypeID, decimal instanceID, Dictionary<string, string> inputParams)
        {
            this.CacheData = new DataSet();

            // 从数据库获取DocTemplateType相关数据
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
        /// 获取数据源
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
                System.Console.Write("获取数据源报错");
            }
        }

        /// <summary>
        /// 根据m_SQL,获取数据,把列和相应的值生成参数
        /// </summary>
        /// <returns>返回参数</returns>
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