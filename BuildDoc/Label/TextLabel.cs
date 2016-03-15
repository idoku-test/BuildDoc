namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Common.Extensions;
    using Newtonsoft.Json.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 文本标签
    /// </summary>
    public class TextLabel : BaseLabel
    {
        /// <summary>
        /// 文档主类
        /// </summary>
        private DocMaster docMaster;

        /// <summary>
        /// 数据源
        /// </summary>
        private DataSource dataSource;

        /// <summary>
        /// 构件ID
        /// </summary>
        private decimal structureID;

        /// <summary>
        /// 动态表单类型
        /// </summary>
        private int formType;

        /// <summary>
        /// 字段ID
        /// </summary>
        private string fieldID;

        /// <summary>
        /// 表ID
        /// </summary>
        private string tableID;

        /// <summary>
        /// 取值方法
        /// </summary>
        public GetDataMethod GetDataMethod
        {
            get;
            set;
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// 原始值
        /// </summary>
        public string InnerValue
        {
            get;
            set;
        }

        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue
        {
            get;
            set;
        }

        /// <summary>
        /// 是否输入
        /// </summary>
        public bool IsInput
        {
            get;
            set;
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// 格式化类
        /// </summary>
        public FormatInfo FormatInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 控件
        /// </summary>
        public BaseControl DocControl
        {
            get;
            set;
        }

        /// <summary>
        /// 关联值
        /// </summary>
        public List<RelateItem> Relate
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
        /// 过滤字段
        /// </summary>
        public string FilterFieldName
        {
            get;
            set;
        }

        /// <summary>
        /// 过滤方式
        /// </summary>
        public string FilterOperation
        {
            get;
            set;
        }

        /// <summary>
        /// 过滤使用值
        /// </summary>
        public string FilterValue
        {
            get;
            set;
        }

        /// <summary>
        /// 是否使用格式化后的值
        /// </summary>
        public bool IsAfterCompute 
        {
            get
            {
                if (FormatInfo == null)
                {
                    return false;
                }
                else
                {
                    return FormatInfo.IsAfterCompute;
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">配置信息</param>
        /// <param name="docMaster">文档主类</param>
        /// <param name="structureID">构件ID</param>
        public TextLabel(string labelName, JToken labelConfig, DocMaster docMaster, decimal structureID)
        {
            this.IsInput = false;
            this.LabelName = labelName;
            this.docMaster = docMaster;
            this.structureID = structureID;
            JObject config = (JObject)labelConfig["Config"];
            string formatString = string.Empty;
            string formatType = string.Empty;
            GetDataMethod = (GetDataMethod)Enum.Parse(typeof(GetDataMethod), config["GetDataMethod"].Value<string>());
            this.Value = string.Empty;
            if (GetDataMethod == GetDataMethod.Const || GetDataMethod == GetDataMethod.Formula)
            {
                this.InnerValue = this.ConfigValue = config["Value"].Value<string>();
                this.dataSource = null;
                this.FieldName = string.Empty;
                this.FormatInfo = null;
            }
            else if (GetDataMethod == GetDataMethod.Source)
            {
                this.DataSourceName = config["DataSourceName"].Value<string>();
                this.dataSource = docMaster.DocTemplateType.DataSourceList.FirstOrDefault(t => t.DataSourceName == this.DataSourceName);
                this.FieldName = config["FieldName"].Value<string>();
                this.FilterFieldName = config["FilterFieldName"].Value<string>();
                this.FilterOperation = config["FilterOperation"].Value<string>();
                this.FilterValue = config["FilterValue"].Value<string>();
            }
            else if (GetDataMethod == GetDataMethod.Dynamic)
            {
                this.formType = config["FormType"].Value<int>();
                this.fieldID = config["FieldID"].Value<string>();
            }
            else if (GetDataMethod == GetDataMethod.MultiSource)
            {
                this.tableID = config["TableID"].Value<string>();
                this.fieldID = config["FieldID"].Value<string>();
            }

            if (config["FormatInfo"] != null)
            {
                FormatInfo = new FormatInfo(config["FormatInfo"], docMaster.DocTemplateType);
            }

            // 控件配置
            if (labelConfig["Relate"] != null)
            {
                string relate = labelConfig["Relate"].ToString();
                if (!string.IsNullOrEmpty(relate))
                {
                    this.Relate = JsonTools.JsonToObject2<List<RelateItem>>(relate);
                }
            }

            this.DocControl = this.CreateControl(labelName, labelConfig, docMaster.DocTemplateType);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="getDataMethod">取值方法</param>
        /// <param name="configValue">配置值</param>
        /// <param name="dataSource">数据源名称</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="formatInfo">格式化类</param>
        public TextLabel(string labelName, GetDataMethod getDataMethod, string configValue, DataSource dataSource, string fieldName, FormatInfo formatInfo)
        {
            this.IsInput = false;
            this.LabelName = labelName;
            this.GetDataMethod = getDataMethod;
            this.ConfigValue = configValue;
            this.dataSource = dataSource;
            this.FieldName = fieldName;
            this.FormatInfo = formatInfo;
        }

        /// <summary>
        /// 界面控件
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">配置信息</param>
        /// <param name="docTemplateType">模板类型</param>
        /// <returns>控件</returns>
        private BaseControl CreateControl(string labelName, JToken labelConfig, DocTemplateType docTemplateType)
        {
            JObject controlConfig = (JObject)labelConfig["Control"];
            if (controlConfig == null)
            {
                return null;
            }

            BaseControl control = null;
            ControlType controlType = (ControlType)Enum.Parse(typeof(ControlType), controlConfig["ControlType"].Value<string>());

            switch (controlType)
            {
                case ControlType.Text:
                    control = new TextControl(labelName, labelConfig);
                    break;
                case ControlType.Date:
                    control = new DateControl(labelName, labelConfig);
                    break;
                case ControlType.Dropdown:
                    FillType fillType = (FillType)Enum.Parse(typeof(FillType), controlConfig["FillType"].Value<string>());

                    if (fillType == FillType.DataSource)
                    {
                        control = new DropdownWithDataSourceControl(labelName, labelConfig, docTemplateType, this.Relate);
                    }
                    else
                    {
                        control = new DropdownWithCustomControl(labelName, labelConfig);
                    }

                    break;
            }

            if (control != null)
            {
                control.ControlType = controlType;
            }

            return control;
        }

        /// <summary>
        /// 格式化后的值
        /// </summary>
        /// <returns>值</returns>
        internal string GetValue()
        {
            if (this.IsInput)
            {
                this.InnerValue = this.Value;
                return this.Value;
            }
            if (!string.IsNullOrEmpty(this.Value))
            {
                return this.Value;
            }

            string strValue = string.Empty;
            try
            {
                switch (GetDataMethod)
                {
                    case GetDataMethod.Const:
                    case GetDataMethod.Formula:
                        strValue = this.InnerValue;
                        break;
                    case GetDataMethod.MultiSource:
                        if (docMaster.GetValueDelegate != null)
                        {
                            if (docMaster.MultiSourceValue == null)
                                docMaster.MultiSourceValue = new Dictionary<string, string>();
                            if (!docMaster.MultiSourceValue.ContainsKey(this.LabelName))
                            {
                                strValue = docMaster.GetValueDelegate(docMaster.InstanceID, this.tableID, this.fieldID, this.structureID, this.LabelName, docMaster.DocTemplateType.ParamsCache);
                                docMaster.MultiSourceValue.Add(this.LabelName, strValue);
                            }
                            else
                                strValue = docMaster.MultiSourceValue[this.LabelName];
                        }
                        break;
                    case GetDataMethod.Dynamic:
                        if (docMaster.DynamicFormData != null)
                        {
                            JArray array = (JArray)docMaster.DynamicFormData[this.formType];
                            var item = array.FirstOrDefault(it => it["NAME"].Value<string>().Equals(this.fieldID));
                            if (item != null)
                            {
                                strValue = item["VALUE"].Value<string>();
                            }
                        }
                        break;
                    case GetDataMethod.Source:
                        strValue = this.dataSource.GetValue(this.FieldName, this.FilterFieldName, this.FilterOperation, this.FilterValue);
                        break;
                }
            }
            catch
            {
                strValue = string.Empty;
            }

            this.InnerValue = strValue;

            // 格式化
            if (FormatInfo != null && !strValue.Contains("@"))
            {
                strValue = this.FormatInfo.ToFormatString(strValue);
            }

            this.Value = strValue;
            return this.Value;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="buildWord">文档操作类</param>
        public override void Execute(IBuildWord buildWord)
        {
            buildWord.InsertText(this.LabelName, this.GetValue());
        }

        /// <summary>
        /// 替换值
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public override bool Replace(string labelName, string value)
        {
            bool result = false;
            switch (GetDataMethod)
            {
                case GetDataMethod.Source:
                    {
                        if (this.FilterValue.Contains("@") &&
                            this.FilterValue.Contains("@" + labelName))
                        {
                            result = true;
                            this.FilterValue = this.FilterValue.Replace("@" + labelName, value);
                        }

                        break;
                    }

                case GetDataMethod.Formula:
                    {
                        //if (this.InnerValue.Contains("@") &&
                        //    this.InnerValue.Contains("@" + labelName + " "))
                        this.InnerValue += " ";
                        if (this.InnerValue.Contains("@") &&
                           this.InnerValue.Contains("@" + labelName + " "))
                        {
                            result = true;
                            //this.InnerValue = this.InnerValue.Replace("@" + labelName + " ", value);
                            this.InnerValue = this.InnerValue.Replace("@" + labelName + " ", value);
                        }

                        if (!this.InnerValue.Contains('@'))
                        {
                            this.InnerValue = this.InnerValue.Eval();
                        }

                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// 替换固定取值中的书名号
        /// </summary>
        /// <param name="inside">书签列表</param>
        public void ReplaceWithConst(List<BaseLabel> inside)
        {
            if (this.GetDataMethod == GetDataMethod.Const)
            {
                Regex r = new Regex(@"《\s*(?<labelName>[\u4e00-\u9fa5\w]+).?\s*》");
                string strValue = this.GetValue();
                if (strValue != null && r.IsMatch(strValue))
                {
                    MatchCollection ms = r.Matches(strValue);
                    foreach (Match m in ms)
                    {
                        var labelName = m.Groups["labelName"].Value;
                        var findLable = inside.FirstOrDefault(i => i.LabelName == labelName);
                        if (findLable != null && findLable is TextLabel)
                        {
                            strValue = Regex.Replace(strValue, "《\\s*" + labelName + "\\s*》", ((TextLabel)findLable).GetValue());
                        }
                    }
                    this.Value = strValue;
                }
            }
        }

        /// <summary>
        /// 关联值
        /// </summary>
        public override string RelateValue
        {
            get
            {
                if (GetDataMethod == GetDataMethod.Source)
                {
                    return this.FilterValue;
                }
                else if (GetDataMethod == GetDataMethod.Formula)
                {
                    return this.InnerValue;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
