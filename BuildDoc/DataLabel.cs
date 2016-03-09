namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 数据标签类
    /// </summary>
    public class DataLabel
    {
        /// <summary>
        /// 文本标签
        /// </summary>
        private TextLabel textLabel;

        /// <summary>
        /// 构件ID
        /// </summary>
        private decimal structureID;

        /// <summary>
        /// 客户ID
        /// </summary>
        private decimal customerID;

        /// <summary>
        /// 文档构件
        /// </summary>
        public BaseControl DocControl
        {
            get;
            set;
        }

        /// <summary>
        /// 数据标签名称
        /// </summary>
        public string DataLabelName
        {
            get;
            set;
        }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName 
        { 
            get
            {
                return this.DataLabelName;
            } 
        }

        /// <summary>
        /// 关联值
        /// </summary>
        public List<RelateItem> Relate 
        { 
            get
            { 
                return this.textLabel.Relate;
            } 
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName 
        { 
            get 
            { 
                return this.textLabel.FieldName;
            } 
        }

        /// <summary>
        /// 格式化信息
        /// </summary>
        public FormatInfo FormatInfo 
        { 
            get
            {
                return this.textLabel.FormatInfo;
            } 
        }

        /// <summary>
        /// 获取数据方法
        /// </summary>
        public GetDataMethod GetDataMethod 
        { 
            get 
            { 
                return this.textLabel.GetDataMethod;
            } 
        }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DataSourceName
        {
            get
            {
                return this.textLabel.DataSourceName;
            }
        }

        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue 
        { 
            get 
            { 
                return this.textLabel.ConfigValue;
            } 
        }

        /// <summary>
        /// 过滤字段名称
        /// </summary>
        public string FilterFieldName
        {
            get
            {
                return this.textLabel.FilterFieldName;
            }
        }

        /// <summary>
        /// 过滤操作符
        /// </summary>
        public string FilterOperation
        {
            get
            {
                return this.textLabel.FilterOperation;
            }
        }

        /// <summary>
        /// 过滤使用值
        /// </summary>
        public string FilterValue 
        { 
            get 
            { 
                return this.textLabel.FilterValue; 
            } 
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataLabelName">数据标签名称</param>
        /// <param name="docMaster">文档主类</param>
        /// <param name="structureID">构件ID</param>
        /// <param name="customerID">客户ID</param>
        public DataLabel(string dataLabelName, DocMaster docMaster, decimal structureID, decimal customerID)
        {
            this.structureID = structureID;
            this.customerID = customerID;

            // 没有缓存从数据库取出内容
            // string key = string.Format("{0}_{1}", dataSourctName, dataLabelName);
            // if(!CacheData.ContainsKey(key))
            // {
            //using (BaseDB dbHelper = new OmpdDBHelper())
            //{
            //    Dictionary<string, object> dicParms = new Dictionary<string, object>();
            //    dicParms.Add("I_LABEL_NAME", dataLabelName);
            //    dicParms.Add("I_CUSTOMER_ID", customerID);
            //    DataTable dt = dbHelper.ExecuteDataTableProc("pkg_redas_build_doc.sp_data_label_get", dicParms);
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        JObject config = JObject.Parse(dt.Rows[0]["CONFIG_CONTENT"].ToString());
            //        if (config["LabelType"].Value<string>() == "TextLabel")
            //        {
            //            this.DataLabelName = config["LabelName"].Value<string>();
            //            this.textLabel = new TextLabel(this.DataLabelName, config, docMaster, this.structureID);

            //            // 添加到缓存
            //            // CacheData.Add(key, this);
            //            this.DocControl = this.textLabel.DocControl;
            //        }
            //    }
            //}

            // }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns>返回值</returns>
        public string GetValue()
        {
            return this.textLabel.GetValue();
        }
    }
}
