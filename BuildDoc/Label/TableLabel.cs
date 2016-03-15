namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 表格标签
    /// </summary>
    public class TableLabel : BaseLabel
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private DataSource dataSource;

        /// <summary>
        /// 过滤字段名称
        /// </summary>
        private string filterFieldName;

        /// <summary>
        /// 过滤操作符
        /// </summary>
        private string filterOperation;

        /// <summary>
        /// 过滤使用值
        /// </summary>
        private string filterValue;

        /// <summary>
        /// 四种填充方式，先实现只填充值的两种
        /// </summary>
        private TableFillType fillType;

        /// <summary>
        /// 取数据源的第几条开始
        /// </summary>
        private int from;

        /// <summary>
        /// 取数据源的第几条结束
        /// </summary>
        private int to;

        /// <summary>
        /// 列集合
        /// </summary>
        private List<ColumnInfo> columns;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">配置信息</param>
        /// <param name="docTemplateType">模板类型fieldName</param>
        public TableLabel(string labelName, JToken labelConfig, DocTemplateType docTemplateType)
        {
            JObject config = (JObject)labelConfig["Config"];
            string dataSourceName = string.Empty;
            string fieldName = string.Empty;
            FormatInfo formatInfo = null;
            string formatString = string.Empty;
            string formatType = string.Empty;

            this.LabelName = labelName;
            dataSourceName = config["DataSourceName"].Value<string>();
            this.dataSource = docTemplateType.DataSourceList.FirstOrDefault(t => t.DataSourceName == dataSourceName);
            this.filterFieldName = config["FilterFieldName"].Value<string>();
            this.filterOperation = config["FilterOperation"].Value<string>();
            this.filterValue = config["FilterValue"].Value<string>();
            this.fillType = (TableFillType)Enum.Parse(typeof(TableFillType), config["FillType"].Value<string>());
            if (config["From"] != null && !string.IsNullOrEmpty(config["From"].ToString()))
            {
                this.from = config["From"].Value<int>();
                this.from--;
            }

            if (config["From"] != null && !string.IsNullOrEmpty(config["To"].ToString()))
            {
                this.to = config["To"].Value<int>();
            }

            this.columns = new List<ColumnInfo>();
            JArray cols = (JArray)config["ColumnInfo"];
            int columnIndex = 0;
            foreach (var col in cols)
            {
                fieldName = col["FieldName"].Value<string>();
                columnIndex = col["ColumnIndex"].Value<int>();
                formatInfo = new FormatInfo(col["FormatInfo"], docTemplateType);
                this.columns.Add(new ColumnInfo(fieldName, columnIndex, formatInfo));
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="filterOperation">过滤操作符</param>
        /// <param name="filterValue">过滤使用值</param>
        /// <param name="fillType">填充方式</param>
        /// <param name="from">取数据源的第几条开始</param>
        /// <param name="to">取数据源的第几条结束</param>
        /// <param name="columns">列集合</param>
        public TableLabel(string labelName, DataSource dataSource, string filterOperation, string filterValue, TableFillType fillType, int from, int to, List<ColumnInfo> columns)
        {
            this.LabelName = labelName;
            this.dataSource = dataSource;
            this.filterOperation = filterOperation;
            this.filterValue = filterValue;
            this.fillType = fillType;
            this.from = from;
            this.to = to;
            this.columns = columns;
        }

        /// <summary>
        /// 根据过滤条件、字段顺序和输出格式化，初始化矩阵数组
        /// </summary>
        /// <returns>表格数据</returns>
        private string[,] GetRows()
        {
            return this.dataSource.GetArrayValue(this.columns, this.filterFieldName, this.filterOperation, this.filterValue, this.from, this.to);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="buildWord">文档操作类</param>
        public override void Execute(IBuildWord buildWord)
        {
            buildWord.InsertTable(this.LabelName, this.GetRows(), this.fillType);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="value">替换使用值</param>
        /// <returns>是否替换成功</returns>
        public override bool Replace(string labelName, string value)
        {
            bool result = false;
            if (this.filterValue.Contains("@") &&
                this.filterValue.Contains("@" + labelName))
            {
                result = true;
                this.filterValue = this.filterValue.Replace("@" + labelName, value);
            }

            return result;
        }

        /// <summary>
        /// 关联值
        /// </summary>
        public override string RelateValue
        {
            get 
            { 
                return this.filterValue; 
            }
        }
    }
}
