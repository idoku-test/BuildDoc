namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 自定义数据源下拉框控件
    /// </summary>
    public class DropdownWithCustomControl : BaseControl
    {
        /// <summary>
        /// 填充方式
        /// </summary>
        public string FillType
        {
            get;
            set;
        }

        /// <summary>
        /// 配置值
        /// </summary>
        public string InputValue
        {
            get;
            set;
        }

        /// <summary>
        /// 分隔符
        /// </summary>
        public string Separtor
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
        public DropdownWithCustomControl(string labelName, JToken labelConfig)
        {
            JObject control = (JObject)labelConfig["Control"];
            this.LabelName = labelName;
            this.FillType = BuildDoc.FillType.Custom.ToString();
            this.Required = control["Required"].Value<bool>();
            this.InputValue = control["InputValue"].Value<string>();
            this.Separtor = control["Separtor"].Value<string>();
            this.IsMultiMode = control["IsMultiMode"].Value<bool>();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public override JArray GetDataSource()
        {
            string json = JsonTools.ObjectToJson2(Regex.Split(this.InputValue, this.Separtor));
            return JArray.Parse(json);
        }
    }
}
