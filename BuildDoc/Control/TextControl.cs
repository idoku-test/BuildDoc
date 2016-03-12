namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 初始化
    /// </summary>
    public class TextControl : BaseControl
    {
        /// <summary>
        /// 验证字符串
        /// </summary>
        public string ValidateString
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">标签配置信息</param>
        public TextControl(string labelName, JToken labelConfig)
        {
            JObject control = (JObject)labelConfig["Control"];
            this.LabelName = labelName;
            this.Required = control["Required"].Value<bool>();
            this.ValidateString = control["ValidateString"].Value<string>();
        }
    }
}
