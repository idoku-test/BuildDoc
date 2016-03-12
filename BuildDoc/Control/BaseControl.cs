namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 基础控件
    /// </summary>
    public class BaseControl
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// 控件类型
        /// </summary>
        public ControlType ControlType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public virtual JArray GetDataSource()
        {
            return new JArray();
        }

        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <returns>字段名称</returns>
        public virtual string GetFieldName()
        {
            return string.Empty;
        }
    }
}
