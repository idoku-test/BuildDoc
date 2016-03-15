namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 列信息
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// 列序号
        /// </summary>
        public int ColumnIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 格式化信息
        /// </summary>
        public FormatInfo FormatInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="columnIndex">列序号</param>
        /// <param name="formatInfo">格式化信息</param>
        public ColumnInfo(string fieldName, int columnIndex, FormatInfo formatInfo)
        {
            this.FieldName = fieldName;
            this.ColumnIndex = columnIndex;
            this.FormatInfo = formatInfo;
        }
    }
}
