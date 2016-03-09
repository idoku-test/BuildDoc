namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 表格填充类型
    /// </summary>
    public enum TableFillType
    {
        /// <summary>
        /// 扩充行
        /// </summary>
        AutoRow,

        /// <summary>
        /// 扩充列
        /// </summary>
        AutoColumn,

        /// <summary>
        /// 仅填充行
        /// </summary>
        OnlyFillByRow,

        /// <summary>
        /// 仅填充列
        /// </summary>
        OnlyFillByColumn
    }

    /// <summary>
    /// 块类型
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// 头
        /// </summary>
        Header,

        /// <summary>
        /// 详细
        /// </summary>
        Detail,

        /// <summary>
        /// 尾
        /// </summary>
        Footer
    }

    /// <summary>
    /// 标签类型
    /// </summary>
    public enum LabelType
    {
        /// <summary>
        /// 条件标签
        /// </summary>
        ConditionLabel,

        /// <summary>
        /// 文档标签
        /// </summary>
        DocLabel,

        /// <summary>
        /// 图像标签
        /// </summary>
        ImageLabel,

        /// <summary>
        /// 表格标签
        /// </summary>
        TableLabel,

        /// <summary>
        /// 文本标签
        /// </summary>
        TextLabel
    }

    /// <summary>
    /// 获取数据方法
    /// </summary>
    public enum GetDataMethod
    {
        /// <summary>
        /// 常量
        /// </summary>
        [Description("常量")]
        Const,

        /// <summary>
        /// 公式
        /// </summary>
        [Description("公式")]
        Formula,

        /// <summary>
        /// 数据源
        /// </summary>
        [Description("数据源")]
        Source,

        /// <summary>
        /// 动态表单
        /// </summary>
        [Description("动态表单")]
        Dynamic,

        /// <summary>
        /// 多数据源
        /// </summary>
        [Description("多数据源")]
        MultiSource
    }

    /// <summary>
    /// 格式化类型
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// 数字
        /// </summary>
        Number,

        /// <summary>
        /// 日期
        /// </summary>
        Date,

        /// <summary>
        /// 值转文本
        /// </summary>
        ValueToText,

        /// <summary>
        /// 转大写1
        /// </summary>
        UpperNumber1,

        /// <summary>
        /// 转大写2
        /// </summary>
        UpperNumber2
    }

    /// <summary>
    /// 控件类型
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,

        /// <summary>
        /// 日期
        /// </summary>
        Date,

        /// <summary>
        /// 下拉框
        /// </summary>
        Dropdown
    }

    /// <summary>
    /// 控件填充类型
    /// </summary>
    public enum FillType
    {
        /// <summary>
        /// 数据源获取
        /// </summary>
        DataSource,

        /// <summary>
        /// 自定义输入
        /// </summary>
        Custom
    }

    /// <summary>
    /// 构件类型
    /// </summary>
    public enum StructureType
    {
        /// <summary>
        /// 后台预先配置构件
        /// </summary>
        Config = 1,

        /// <summary>
        /// 自定义构件
        /// </summary>
        Custom = 2
    }
}
