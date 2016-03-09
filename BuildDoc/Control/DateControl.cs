namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 日期控件
    /// </summary>
    public class DateControl : BaseControl
    {
        /// <summary>
        /// 最小日期
        /// </summary>
        public DateTime MinDate
        {
            get;
            set;
        }

        /// <summary>
        /// 最大日期
        /// </summary>
        public DateTime MaxDate
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">标签配置信息</param>
        public DateControl(string labelName, JToken labelConfig)
        {
            JObject control = (JObject)labelConfig["Control"];
            this.LabelName = labelName;
            this.Required = control["Required"].Value<bool>();
            if (!string.IsNullOrEmpty(control["MinDate"].ToString()))
            {
                this.MinDate = control["MinDate"].Value<DateTime>();
            }

            if (!string.IsNullOrEmpty(control["MaxDate"].ToString()))
            {
                this.MaxDate = control["MaxDate"].Value<DateTime>();
            }
        }
    }
}
