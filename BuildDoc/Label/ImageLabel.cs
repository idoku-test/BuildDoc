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
    /// 图像标签
    /// </summary>
    public class ImageLabel : BaseLabel
    {
        /// <summary>
        /// 图像名称
        /// </summary>
        private string imageName;

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
        public ImageLabel(string labelName, JToken labelConfig, DocTemplateType docTemplateType)
        {
            JObject config = (JObject)labelConfig["Config"];
            string dataSourceName = string.Empty;
            this.LabelName = labelName;

            this.getDataMethod = (GetDataMethod)Enum.Parse(typeof(GetDataMethod), config["GetDataMethod"].Value<string>());
            if (config["DataSourceName"] != null)
            {
                dataSourceName = config["DataSourceName"].Value<string>();
                this.dataSource = docTemplateType.DataSourceList.FirstOrDefault(t => t.DataSourceName == dataSourceName);
            }

            if (config["ImageName"] != null)
            {
                this.imageName = config["ImageName"].Value<string>();
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
        /// <param name="imageName">图像名称</param>
        /// <param name="fileID">文件ID</param>
        public ImageLabel(string labelName, string imageName, decimal fileID)
        {
            this.LabelName = labelName;
            this.imageName = imageName;
            this.fileID = fileID;
        }

        /// <summary>
        /// 根据过滤条件、字段顺序和输出格式化，初始化矩阵数组
        /// </summary>
        /// <returns>矩阵数组</returns>
        private string[,] GetRows()
        {
            var columns = new List<ColumnInfo>();
            columns.Add(new ColumnInfo("文件ID", 0, null));
            return this.dataSource.GetArrayValue(columns, this.filterFieldName, this.filterOperation, this.filterValue, 0, 0);
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
        /// 获取图像流
        /// </summary>
        /// <returns>图像流</returns>
        public Stream GetStream()
        {
            decimal fileID = this.GetFileID();
            return FileServerHelper.GetFileStream(fileID);
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="buildWord">文档操作类</param>
        public override void Execute(IBuildWord buildWord)
        {
            var fileIds = this.GetRows();
            buildWord.InsertImage(this.LabelName, fileIds);
        }

        /// <summary>
        /// 引用替换
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="value">替换使用值</param>
        /// <returns>是否替换成功</returns>
        public override bool Replace(string labelName, string value)
        {
            bool result = false;
            if (this.filterValue.Contains("@") && this.filterValue.Contains("@" + labelName))
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
