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
    /// 数据源下拉框控件
    /// </summary>
    public class DropdownWithDataSourceControl : BaseControl
    {
        /// <summary>
        /// 模板类型
        /// </summary>
        private DocTemplateType docTemplateType;

        /// <summary>
        /// 关联控件
        /// </summary>
        private List<RelateItem> relate;

        /// <summary>
        /// 填充类型
        /// </summary>
        public string FillType
        {
            get;
            set;
        }

        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource
        {
            get;
            set;
        }

        /// <summary>
        /// 值字段
        /// </summary>
        public string ValueField
        {
            get;
            set;
        }

        /// <summary>
        /// 文本字段
        /// </summary>
        public string TextField
        {
            get;
            set;
        }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMultiMode
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">标签配置信息</param>
        /// <param name="docTemplateType">模板类型</param>
        /// <param name="relate">关联控件</param>
        public DropdownWithDataSourceControl(string labelName, JToken labelConfig, DocTemplateType docTemplateType, List<RelateItem> relate)
        {
            this.docTemplateType = docTemplateType;
            this.relate = relate;
            JObject control = (JObject)labelConfig["Control"];
            this.LabelName = labelName;
            this.FillType = BuildDoc.FillType.DataSource.ToString();
            this.Required = control["Required"].Value<bool>();
            this.DataSource = control["DataSource"].Value<string>();
            this.ValueField = control["ValueField"].Value<string>();
            this.TextField = control["TextField"].Value<string>();
            this.IsMultiMode = control["IsMultiMode"].Value<bool>();
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns>数据源</returns>
        public override JArray GetDataSource()
        {
            var dataSource = this.docTemplateType.DataSourceList.Find(it => it.DataSourceName.Equals(this.DataSource));
            JArray newJArray = new JArray();
            if (dataSource != null)
            {
                DataTable dt = dataSource.GetDataTable();
                foreach (DataRow dr in dt.Rows)
                {
                    JObject newJObject = new JObject();
                    newJObject.Add(new JProperty(this.ValueField, dr[this.ValueField].ToString()));
                    newJObject.Add(new JProperty(this.TextField, dr[this.TextField].ToString()));
                    foreach (var relateItem in this.relate)
                    {
                        newJObject.Add(new JProperty(relateItem.FieldName, dr[relateItem.FieldName].ToString()));
                    }

                    newJArray.Add(newJObject);
                }
            }

            return newJArray;
        }

        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <returns>字段名称</returns>
        public override string GetFieldName()
        {
            return this.TextField;
        }
    }
}
