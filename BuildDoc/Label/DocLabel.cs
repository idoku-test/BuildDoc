namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 文档标签
    /// </summary>
    public class DocLabel : BaseLabel
    {
        /// <summary>
        /// 文档主类
        /// </summary>
        private DocMaster docMaster;

        /// <summary>
        /// 文档名称
        /// </summary>
        private string docName;

        /// <summary>
        /// 文件ID
        /// </summary>
        private decimal fileID;

        /// <summary>
        /// 获取数据方法
        /// </summary>
        private GetDataMethod getDataMethod;

        /// <summary>
        /// 数据源
        /// </summary>
        private DataSource dataSource;

        /// <summary>
        /// 字段名称
        /// </summary>
        private string fieldName;

        /// <summary>
        /// 过滤字段名称
        /// </summary>
        private string filterFieldName;

        /// <summary>
        /// 过滤字段操作符
        /// </summary>
        private string filterOperation;

        /// <summary>
        /// 过滤字段使用值
        /// </summary>
        private string filterValue;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">配置信息</param>
        /// <param name="docTemplateType">模板类型</param>
        public DocLabel(string labelName, JToken labelConfig, DocMaster docMaster)
        {
            this.docMaster = docMaster;
            JObject config = (JObject)labelConfig["Config"];
            string dataSourceName = string.Empty;

            this.LabelName = labelName;
            this.getDataMethod = (GetDataMethod)Enum.Parse(typeof(GetDataMethod), config["GetDataMethod"].Value<string>());
            if (config["DataSourceName"] != null)
            {
                dataSourceName = config["DataSourceName"].Value<string>();
                this.dataSource = this.docMaster.DocTemplateType.DataSourceList.FirstOrDefault(t => t.DataSourceName == dataSourceName);
            }

            if (config["DocName"] != null)
            {
                this.docName = config["DocName"].Value<string>();
            }

            if (config["FileID"] != null)
            {
                this.fileID = config["FileID"].Value<decimal>();
            }

            if (config["FieldName"] != null)
            {
                this.fieldName = config["FieldName"].Value<string>();
            }

            if (config["FilterFieldName"] != null)
            {
                this.filterFieldName = config["FilterFieldName"].Value<string>();
            }

            if (config["FilterOperation"] != null)
            {
                this.filterOperation = config["FilterOperation"].Value<string>();
            }

            if (config["FilterValue"] != null)
            {
                this.filterValue = config["FilterValue"].Value<string>();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="docName">文档名称</param>
        /// <param name="fileID">文件ID</param>
        /// <param name="getDataMethod">获取数据方法</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="filterFieldName">过滤字段名称</param>
        /// <param name="filterOperation">过滤字段操作符</param>
        /// <param name="filterValue">过滤字段使用值</param>
        public DocLabel(
            string labelName, 
            string docName, 
            decimal fileID, 
            GetDataMethod getDataMethod, 
            DataSource dataSource,
            string fieldName, 
            string filterFieldName, 
            string filterOperation, 
            string filterValue)
        {
            this.LabelName = labelName;
            this.docName = docName;
            this.fileID = fileID;
            this.getDataMethod = getDataMethod;
            this.dataSource = dataSource;
            this.fieldName = fieldName;
            this.filterFieldName = filterFieldName;
            this.filterOperation = filterOperation;
            this.filterValue = filterValue;
        }

        /// <summary>
        /// 获取文件ID
        /// </summary>
        /// <returns>文件ID</returns>
        private decimal GetFileID()
        {
            decimal fileID = 0;
            try
            {
                switch (this.getDataMethod)
                {
                    case GetDataMethod.Const:
                        fileID = this.fileID;
                        break;
                    case GetDataMethod.Source:
                        {
                            string tmpValue = this.dataSource.GetValue(this.fieldName, this.filterFieldName, this.filterOperation, this.filterValue);
                            decimal.TryParse(tmpValue, out fileID);
                            break;
                        }
                }
            }
            catch
            {
                fileID = 0;
            }

            return fileID;
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns>文件流</returns>
        public Stream GetStream()
        {
            Stream docStream = new MemoryStream(); //FileHelper.GetFileStream(this.GetFileID());
            if (docStream == null)
            {
                return null;
            }

            IBuildWord buildWord = new BuildWord(docStream);
            // 替换文档中的所有标签
            foreach (var label in this.docMaster.LabelList)
            {
                if (label is TextLabel)
                {
                    var tl = label as TextLabel;
                    buildWord.ReplaceText(tl.LabelName, tl.GetValue());
                }
            }

            return buildWord.GetStream();
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="buildWord">文档操作类</param>
        public override void Execute(IBuildWord buildWord)
        {
            buildWord.InsertDoc(this.LabelName, this.GetStream(), false);
        }

        /// <summary>
        /// 替换公式引用值
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="value">替换值</param>
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
